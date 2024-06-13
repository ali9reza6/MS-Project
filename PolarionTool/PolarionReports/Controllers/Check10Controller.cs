using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PolarionReports.BusinessLogic;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using PolarionReports.Models.Tooltip;
using System.Diagnostics;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class Check10Controller : Controller
    {
        // GET: Check10
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Project(string id, string docFilter)
        {
            string Documentname = "";
            string Browser;

            Project myP;
            DocumentName DocNameToCheck;
            CheckNames checkNames = new CheckNames();
            List<Uplink> TempUplinks = new List<Uplink>();
            Workitem Tempworkitem = new Workitem();
            WorkitemLinkError wle;

            Check10BL check10BL = new Check10BL();

            Browser = Request.Browser.Browser;

            myP = InitProject.GetProject(id, Browser);
            ViewBag.Error = myP.DatabaseErrorMessage;

            Check10ViewModel cv = new Check10ViewModel
            {
                Projectname = myP.C_name,
                Errorname = "Missing Uplinks:",
                PolarionLink = "http://" + Topol.PolarionServer + "/polarion/#/project/" + myP.C_id + "/workitem?id=",
                Documents = new List<Document>(),
                Tooltip10 = new Tooltip10(),
                TooltipReview = new TooltipReview()
            };

            // 20 Dokument suchen
            foreach (Document d in myP.Documents)
            {
                d.SetBrowserType(myP.Browser); // for max URL length
                if (d.C_id.Substring(0, 2) == "10" && d.C_modulefolder == "Specification")
                {
                    if (docFilter != null)
                    {
                        if (!d.DocName.OriginalDocumentname.Contains(docFilter))
                        {
                            continue;
                        }
                    }
                    d.check10Lists = new Check10Lists(cv);
                    d.Review = new Review(myP.Users);
                    cv.Documents.Add(d);

                    d.PolarionDocumentLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/{d.C_id}?selection=";

                    d.PolarionTableLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/"
                        + System.Uri.EscapeDataString(d.C_id) + "?query=";

                    foreach (Workitem w in myP.Workitems.FindAll(x => x.DocumentId == d.C_pk && x.Type == "customerrequirement"))
                    {
                        w.FillComments(myP.Comments, myP.Users); // Approval Comments zu Workitem verknüpfen

                        if (w.Id == "E19008-4116")
                        {
                            Debug.WriteLine(w.Id);
                        }
                        if (w.FillCheckUplink(myP.Uplinks) == 0)
                        {
                            if (w.InBin)
                            {
                                // gelöschtes Workitem -> keine weiteren Prüfungen
                                Debug.WriteLine(w.Id + " In Bin");
                                d.check10Lists.DeletedCustomerRequirements.Table.Add(w);
                                continue;
                            }
                        }


                        if (w.Status == "rejected")
                        {
                            if (w.InternalComments == null || w.InternalComments.Length == 0)
                            {
                                d.check10Lists.RejectedCustomerRequirementsWithoutComment.Table.Add(w);
                            }
                            else
                            {
                                d.check10Lists.RejectedCustomerRequirements.Table.Add(w);
                            }
                            continue;
                        }

                        if (w.Status == "norequirement" || w.Status == "noRequirement")
                        {
                            d.check10Lists.CustomerRequirementsWithStatusNo.Table.Add(w);
                            continue;
                        }

                        if (w.FillCheckDownlink(myP.Uplinks) == 0)
                        {
                            // kein Downlinks
                            // Debug.WriteLine("kein Downlink " + w.Id);
                            if (w.Status != "rejected" && w.Status != "noRequirement")
                            {
                                d.check10Lists.UnlinkedCustomerRequirements.Table.Add(w);
                            }
                        }
                        else
                        {
                            foreach (Uplink dl in w.Downlinks)
                            {
                                Tempworkitem = myP.Workitems.Find(x => x.C_pk == dl.WorkitemId);
                                if (Tempworkitem == null)
                                {
                                    continue; // eventuell in Fehlerliste
                                }
                                Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                                DocNameToCheck = new DocumentName(Documentname);
                                if (Documentname.Length < 2)
                                {
                                    continue;
                                }

                                if (Documentname.Substring(0, 2) == "20" && DocNameToCheck.Subindex == null)
                                {
                                    // kein Subindex alle 10 Links werden akteptiert
                                    if (Tempworkitem.Type == "requirement")
                                    {
                                        if (!d.check10Lists.LinkedCustomerRequirements.Table.Contains(w))
                                        {
                                            d.check10Lists.LinkedCustomerRequirements.Table.Add(w);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Documentname.Substring(0, 2) == "20" && DocNameToCheck.Subindex != null && DocNameToCheck.Subindex.Contains(d.DocName.Prefix))
                                    {
                                        if (Tempworkitem.Type == "requirement")
                                        {
                                            if (!d.check10Lists.LinkedCustomerRequirements.Table.Contains(w))
                                            {
                                                d.check10Lists.LinkedCustomerRequirements.Table.Add(w);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Documentname.Substring(0, 2) != "10") // Downlinks im 10 Document ignorieren
                                        {
                                            // Falsche Verknüpfung
                                            wle = new WorkitemLinkError();
                                            wle.Workitem = w;
                                            wle.LinkedWorkitem = Tempworkitem;
                                            w.TextError = DocNameToCheck.Level + DocNameToCheck.Prefix;
                                            if (DocNameToCheck.Subindex != null && DocNameToCheck.Subindex.Length > 0) w.TextError += "_" + DocNameToCheck.Subindex;
                                            wle.ErrorMsg = "Wrong Doc " + w.TextError;
                                            wle.IdLinkedWorkitem = Tempworkitem.Id;
                                            wle.PolarionLink = myP.GetPolarionLink(Documentname, Tempworkitem.Id);
                                            d.check10Lists.IncorrectlyLinkedCustomerRequirements.Table.Add(wle);
                                        }
                                    }
                                }
                                
                            }
                        } // Check Downlinks
                        
                        if (w.Status == "partlyaccepted" || w.Status == "partlyAccepted")
                        {
                            if (w.InternalComments == null || w.InternalComments.Length == 0)
                            {
                                d.check10Lists.PartlyAcceptedCustomerRequirementsWithoutComment.Table.Add(w);
                            }
                            else
                            {
                                d.check10Lists.PartlyAcceptedCustomerRequirements.Table.Add(w);
                            }
                        }

                        // Customer Requirements with Status = Open
                        if (w.Status == "open")
                        {
                            d.check10Lists.CustomerRequirementsWithStatusOpen.Table.Add(w);
                        }

                        if (w.Status == "maybeAccepted")
                        {
                            d.check10Lists.CustomerRequirementsWithStatusMaybeAccepted.Table.Add(w);
                        }

                        if (w.Status == "accepted")
                        {
                            d.check10Lists.AcceptedCustomerRequirements.Table.Add(w);
                        }

                        // Special Reports
                        if (w.CustomerAction != null && w.CustomerAction.Length > 0)
                        {
                            d.check10Lists.CustomerRequirementsWithCustomerAction.Table.Add(w);
                        }

                        if (w.SupplierAction != null && w.SupplierAction.Length > 0)
                        {
                            d.check10Lists.CustomerRequirementsWithSupplierAction.Table.Add(w);
                        }

                        if (w.Cs != null && w.Cs == "csRelevant")
                        {
                            d.check10Lists.CyberSecurityRelatedCustomerRequirements.Table.Add(w);
                        }

                        if (w.Asil != null && w.Asil.Length > 4 && w.Asil.Substring(0,4) == "asil")
                        {
                            d.check10Lists.SafetyRelatedCustomerRequirements.Table.Add(w);
                            Debug.WriteLine(w.Asil + " " + w.TextCS);
                        }

                        // Customer Requirements in Clarification
                        // Review Report:
                        w.FillApprovals(myP.WorkitemApprovals, myP.Users);
                        d.Review.CheckWorkitem(w);
                        d.Review.WorkitemPerClarificationList.OrderBy(x => x.SortOrder);

                    } // foreach Workitem
                }
            }

            return View(cv);
        }
    }
}