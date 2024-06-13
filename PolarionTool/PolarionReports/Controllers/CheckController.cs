using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolarionReports.Models;
using System.Diagnostics;

using PolarionReports.BusinessLogic;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class CheckController : Controller
    {
        // GET: Check
        public ActionResult Index()
        {
            string Error;
            DatareaderP dr;
            Project myP;
            List<Document> dl;
            List<Workitem> wl;
            List<Uplink> ul = new List<Uplink>();

            dr = new DatareaderP();
            myP = dr.GetProject("E17019", out Error);
            dl = dr.GetDocuments(myP.C_pk, out Error);
            wl = dr.GetWorkitems(myP.C_pk, out Error);
            
            CheckViewModel cv = new CheckViewModel();
            cv.Projectname = myP.C_name;
            cv.Errorname = "Missing Uplinks:";
            cv.PolarionLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/workitem?id=";
            cv.Documents = new List<Document>();

            // 20 Dokument suchen
            foreach(Document d in dl)
            {
                d.SetBrowserType(myP.Browser);  // for max URL length
                if (d.C_id.Substring(0,2) == "20" && d.C_modulefolder == "Specification" )
                {
                    cv.Documents.Add(d);
                    foreach(Workitem w in wl)
                    {
                        if (w.DocumentId == d.C_pk)
                        {
                            // Workitem gehört zum Dokument
                            if (w.FillCheckUplink(ul) == 0)
                            {
                                // Missing Uplink => Workitem zur Fehlerliste hinzufügen
                                d.ErrorList.Add(w);
                            }
                            else
                            {
                                d.UplinkCounter++;
                            }
                        }
                    }
                }
            }
            return View(cv);
        }

        public ActionResult Project(string id)
        {
            string ErrorMsg;
            string Documentname;
            Project myP;
            DocumentName DocNameToCheck;
            CheckNames checkNames = new CheckNames();
            List<Uplink> TempUplinks= new List<Uplink>();
            Workitem Tempworkitem;

            string Browser = Request.Browser.Browser;
            myP = InitProject.GetProject(id, Browser);
            ViewBag.Error = myP.DatabaseErrorMessage;

            CheckViewModel cv = new CheckViewModel
            {
                Projectname = myP.C_name,
                Errorname = "Missing Uplinks:",
                PolarionLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/workitem?id=",
                Documents = new List<Document>(),
                Tooltip20 = new Tooltip20(),
                TooltipReview = new TooltipReview()
            };

            // 20 Dokument suchen
            // dl.Cast<Document>().Where(x => x.C_id.s)
            foreach (Document d in myP.Documents)
            {
                if (d.C_id.Substring(0, 2) == "20" && d.C_modulefolder == "Specification")
                {
                    cv.Documents.Add(d);
                    d.LinkedRequirementList = new List<Workitem>();
                    d.ErrorList = new List<Workitem>();
                    d.RejectedList = new List<Workitem>();
                    d.WrongDocumentLinkList = new List<Workitem>();
                    d.MissingRefSW = new List<Workitem>();
                    d.RefSW = new List<Workitem>();
                    d.MissingRefHW = new List<Workitem>();
                    d.RefHW = new List<Workitem>();
                    d.MissingRefInterface = new List<Workitem>();
                    d.RefInterface = new List<Workitem>();
                    d.Error30Interface = new List<Workitem>();
                    d.Missing30Links = new List<Workitem>();
                    d.Links30 = new List<Workitem>();
                    d.StatusDraft = new List<Workitem>();
                    d.StatusClarificationNeeded = new List<Workitem>();
                    d.StatusReviewNeeded = new List<Workitem>();
                    d.StatusDeferred = new List<Workitem>();
                    d.StatusTBD = new List<Workitem>();
                    d.SeverityShouldHave = new List<Workitem>();
                    d.AllocMissing = new List<Workitem>();
                    d.AllocInterface = new List<Workitem>();
                    d.AllocESE = new List<Workitem>();
                    d.FuncReqHSM = new List<Workitem>();
                    d.MissingHyperlink = new List<Workitem>();
                    d.Hyperlinks = new List<Workitem>();
                    d.SpecialReportCustomerAction = new List<Workitem>();
                    d.SpecialReportSupplierAction = new List<Workitem>();
                    d.SpecialReportESESW = new List<Workitem>();
                    d.SpecialReportESEHW = new List<Workitem>();
                    d.WorkitemsInBin = new List<Workitem>();

                    d.Review = new Review(myP.Users);

                    d.PolarionDocumentLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/wiki/Specification/{d.C_id}?selection=";

                    d.PolarionTableLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/wiki/Specification/"
                        + System.Uri.EscapeDataString(d.C_id) + "?query=";

                    // auch x.type == 'customer requirement' -> mit Hinweismeldung
                    foreach (Workitem w in myP.Workitems.FindAll(x => x.DocumentId == d.C_pk && x.Type == "requirement"))
                    {
                        if (w.Id == "E18010-6252")
                        {
                            Debug.WriteLine(w.Id);
                        }

                        w.FillComments(myP.Comments, myP.Users); // Approval Comments zu Workitem verknüpfen

                        if (w.Status == "rejected")
                        {
                            d.RejectedList.Add(w);
                            w.FillApprovals(myP.WorkitemApprovals, myP.Users);
                            d.Review.CheckWorkitem(w);
                            continue;
                        }
                       
                        // Workitem gehört zum Dokument:
                        #region Check Uplink in 10er Document
                        bool InErrorList = false;
                        bool InWrongDocumentList = false;
                        bool LinkToCustomerRequirement = false;
                        if (w.FillCheckUplink(myP.Uplinks) == 0)
                        {
                            if (w.InBin)
                            {
                                // gelöschtes Workitem -> keine weiteren Prüfungen
                                d.WorkitemsInBin.Add(w);
                                continue;
                            }

                            // Missing Uplink => Workitem zur Fehlerliste hinzufügen - Requirement 
                            d.ErrorList.Add(w);
                            InErrorList = true;
                        }
                        else
                        {
                            if (w.InBin)
                            {
                                // gelöschtes Workitem mit Uplinks -> keine weiteren Prüfungen
                                d.WorkitemsInBin.Add(w);
                                continue;
                            }
                            // Workitem hat einen oder mehrere Links -> Prüfung ob richtiges Document
                            foreach (Uplink u in w.Uplinks)
                            {
                                if (myP.IsUplinkCustomerRequirement(u))
                                {
                                    LinkToCustomerRequirement = true;
                                    DocNameToCheck = new DocumentName(myP.GetUplinkDocumentname(u));
                                    if (!checkNames.CheckDocumentRelation(d.DocName, DocNameToCheck, out ErrorMsg))
                                    {
                                        u.UplinkToWrongDocument = true;
                                        u.ErrorMessage = ErrorMsg;
                                        if (!InWrongDocumentList)
                                        {
                                            d.WrongDocumentLinkList.Add(w);
                                            InWrongDocumentList = true;
                                        }
                                        // break;
                                    }
                                    Debug.WriteLine(w.Title + " -> " + DocNameToCheck.OriginalDocumentname);
                                }
                                else
                                {
                                    // kein CustomerRequirement:
                                    if (u.Role == "relates_to")
                                    {
                                        // Uplinks nur in 10 und 20 document erlaubt
                                        // oder Uplinks zu ChangeRequest
                                        Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                                        if (Tempworkitem == null)
                                        {
                                            // Fehler oder anderes Dokument
                                        }
                                        else
                                        { 
                                            if (Tempworkitem.Type == "requirement")
                                            {
                                                DocNameToCheck = new DocumentName(myP.GetUplinkDocumentname(u));
                                                if (DocNameToCheck.Level != "10" && DocNameToCheck.Level != "20")
                                                {
                                                    u.UplinkToWrongDocument = true;
                                                    u.ErrorMessage = "Uplink to " + DocNameToCheck.Level;
                                                    if (!InWrongDocumentList)
                                                    {
                                                        // um doppelte Eintraäge zu verhindern
                                                        d.WrongDocumentLinkList.Add(w);
                                                        InWrongDocumentList = true;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                // Behandlung von uplinks zu nicht "requirements"
                                            }
                                        }
                                    }

                                }

                            }
                            d.UplinkCounter += w.Uplinks.Count;
                            
                        }
                        //if (!InErrorList && !InWrongDocumentList && LinkToCustomerRequirement)
                        if (LinkToCustomerRequirement)
                        {
                            d.LinkedRequirementList.Add(w);
                        }
                        else
                        {
                            if (!InErrorList)
                            {
                                d.ErrorList.Add(w); 
                            }
                        }
                        #endregion check Uplink

                        #region check downlink in 30 Document
                        // ToDo: auf leeres Document überprüfen
                        // 1. Check
                        // Alle Workitems die ein Interface sind müssen einen Downlink in das 30 Element Architecture haben
                        // und dieses Workitem muss ein "Interface" sein

                        // 2. Check
                        // Alle Workitems (HW, SW, M) die kein Interface sind müssen einen Downlink in das 30 Element Architecture haben
                        // und dieses Workitems muss ein "Component" sein

                        if (w.IsHSM())
                        {
                            bool NotFound = true;

                            if (w.Id == "E17015-3390")
                            {
                                Debug.WriteLine(w.Id);
                            }

                            TempUplinks = myP.Uplinks.FindAll(x => x.UplinkId == w.C_pk);
                            if (w.Interface)
                            {
                                // 1. Check
                                foreach(Uplink u in TempUplinks)
                                {
                                    Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.WorkitemId);
                                    if (Tempworkitem == null)
                                    {
                                        // Uplink auf workitem "heading"
                                        continue;
                                    }
                                    Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                                    if (Documentname.Length < 2)
                                    {
                                        continue;
                                    }
                                    if (u.Role == "parent")
                                    {
                                        continue; // uplinks zu parent ignorieren
                                    }
                                    if (Documentname.Substring(0,2) == "30" && Documentname.Contains("Element"))
                                    {
                                        if (Tempworkitem.Type != "interface")
                                        {
                                            if (Tempworkitem.Type != "requirement")
                                            {
                                                // Fehler -> Workitem in Fehlerliste - alle Requirements ignorieren
                                                w.TextError30Interface = "not linked to INT";
                                                d.Error30Interface.Add(w);
                                                NotFound = false;
                                                break; // Nach dem 1. Fehler dieses Workitem nicht weiter prüfen
                                            }
                                        }
                                        else
                                        {
                                            // richtigen Link gefunden
                                            NotFound = false;
                                            break;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                // 2. Check kein Interface
                                foreach (Uplink u in TempUplinks)
                                {
                                    Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.WorkitemId);
                                    Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                                    if (Documentname.Length < 2)
                                    {
                                        continue;
                                    }
                                    if (u.Role == "parent")
                                    {
                                        continue; // uplinks zu parent ignorieren
                                    }
                                    if (Documentname.Substring(0, 2) == "30" && Documentname.Contains("Element"))
                                    {
                                        if (Tempworkitem.Type != "component")
                                        {
                                            // Fehler -> Workitem in Fehlerliste
                                            if (Tempworkitem.Type != "requirement")
                                            {
                                                w.TextError30Interface = "not linked to COMP";
                                                d.Error30Interface.Add(w);
                                                NotFound = false;
                                                break; // Nach dem 1. Fehler dieses Workitem nicht weiter prüfen
                                            }
                                        }
                                        else
                                        {
                                            // richtigen Link gefunden
                                            NotFound = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (NotFound)
                            {
                                if (!w.Project)
                                {
                                    // Fehlender Link in 30 Element Architecture 
                                    // 2018 -08-17: keine Workitems mit Allocation "Project"
                                    d.Missing30Links.Add(w);
                                }
                            }
                            else
                            {
                                // Link gefunden in die Liste 
                                d.Links30.Add(w);
                            }
                        }
                        #endregion

                        #region check reference in 40 Document
                        if (w.Software)
                        {
                            // Das Workitem ist ein Software-Workitem -> alle References zu diesem Element suchen:
                            List<WorkitemReference> wrl = myP.WorkitemReferences.FindAll(x => x.WorkitemID == w.C_pk);
                            bool NoRefSoftware40 = true;
                            foreach (WorkitemReference wr in wrl)
                            {
                                if (wr.DocumentName.Substring(0,2) == "40" && 
                                    (wr.DocumentName.Contains("SW") || wr.DocumentName.Contains("Software")))
                                {
                                    NoRefSoftware40 = false;
                                    d.RefSW.Add(w);
                                }
                            }
                            if (NoRefSoftware40)
                            {
                                // Überprüfen ob statt der Reference ein Link existiert
                                if (w.CheckLinkTo(myP, "40", "Software", "relates_to", "requirement") != null)
                                {
                                    d.RefSW.Add(w);
                                }
                                else
                                {
                                    // Workitem in Fehlerliste
                                    d.MissingRefSW.Add(w);
                                }
                            }
                        }

                        if (w.Hardware)
                        {
                            // Das Workitem ist ein Software-Workitem -> alle References zu diesem Element suchen:
                            List<WorkitemReference> wrl = myP.WorkitemReferences.FindAll(x => x.WorkitemID == w.C_pk);
                            bool NoRefHardware40 = true;
                            foreach (WorkitemReference wr in wrl)
                            {
                                if (wr.DocumentName.Substring(0, 2) == "40" &&
                                    (wr.DocumentName.Contains("HW") || wr.DocumentName.Contains("Hardware")))
                                {
                                    NoRefHardware40 = false;
                                    d.RefHW.Add(w);
                                }
                            }
                            if (NoRefHardware40)
                            {
                                // Überprüfen ob statt der Reference ein Link existiert
                                if (w.CheckLinkTo(myP, "40", "Hardware", "relates_to", "requirement") != null)
                                {
                                    d.RefHW.Add(w);
                                }
                                else
                                {
                                    // Workitem in Fehlerliste
                                    d.MissingRefHW.Add(w);
                                }
                            }
                        }
                        #endregion

                        #region check reference in 30 Document
                        if ((w.Hardware || w.Software) && w.Interface && !w.Mechanic)
                        {
                            // Das Workitem ist ein Software oder Hardware-Workitem mit Interface-> alle References zu diesem Element suchen:
                            List<WorkitemReference> wrl = myP.WorkitemReferences.FindAll(x => x.WorkitemID == w.C_pk);
                            bool NoRefInterface30 = true;
                            foreach (WorkitemReference wr in wrl)
                            {
                                if (wr.DocumentName.Substring(0, 2) == "30" &&
                                    wr.DocumentName.Contains("Interface"))
                                {
                                    NoRefInterface30 = false;
                                    d.RefInterface.Add(w);
                                }
                            }
                            if (NoRefInterface30)
                            {
                                // Überprüfen ob statt der Reference ein Link existiert
                                if (w.CheckLinkTo(myP, "30", "Interface", "relates_to", "requirement") != null)
                                {
                                    // Link statt Reference
                                    d.RefInterface.Add(w);
                                }
                                else
                                {
                                    // Workitem in Fehlerliste
                                    d.MissingRefInterface.Add(w);
                                }
                            }
                        }
                        #endregion

                        #region Requirement Status
                        //Requirement Status:
                        // Liste Status = draft
                        if (w.Status == "draft")
                        {
                            d.StatusDraft.Add(w);
                        }
                        // Liste Status = in clarification
                        if (w.Status == "clarificationNeeded")
                        {
                            d.StatusClarificationNeeded.Add(w);
                        }
                        // Liste Review Needed
                        if (w.Status == "reviewNeeded")
                        {
                            d.StatusReviewNeeded.Add(w);
                        }
                        // Liste Status Deffered
                        if (w.Status == "deferred")
                        {
                            d.StatusDeferred.Add(w);
                        }
                        // Liste TBD
                        if (w.Title.Contains("TBD") && w.Status != "clarificationNeeded")
                        {
                            d.StatusTBD.Add(w);
                        }
                        #endregion

                        #region Requirement Prioritization
                        if (w.Severity == "should_have")
                        {
                            d.SeverityShouldHave.Add(w);
                        }
                        #endregion

                        #region Requirement Allocation
                        // Allocation missing
                        if (myP.WorkitemRequirementAllocations.Count(x => x.WorkitemID == w.C_pk) == 0)
                        {
                            d.AllocMissing.Add(w);
                        }
                            
                        // Allocation = INT + other than “HW, SW, Mech”
                        if (w.Interface && !w.IsHSM())
                        {
                            d.AllocInterface.Add(w);
                        }

                        // Allocation = “ESE” + other than “HW, SW, Mech”
                        if (w.ESE && !w.IsHSM())
                        {
                            d.AllocESE.Add(w);
                        }
                        #endregion

                        #region Functional Requirement
                        // "Functional requirement" checked for != HW, SW, Mech oder 
                        // "Functional requirement" mit Allocation Project
                        if ((w.FunctionalRequirement && !w.IsHSM()) || (w.FunctionalRequirement && w.Project))
                        {
                            if (!d.AllocMissing.Contains(w))
                            {
                                // Wenn das Workitem nicht in der Liste AllocMissing ist
                                d.FuncReqHSM.Add(w);
                            }
                        }
                        #endregion

                        #region Missing
                        if (w.ReferredDoc != null && myP.Hyperlinks != null)
                        {
                            // Reffered Doc search Hyperlink
                            Hyperlink h = myP.Hyperlinks.FirstOrDefault(x => x.WorkitemID == w.C_pk);
                            if (h == null)
                            {
                                d.MissingHyperlink.Add(w);
                            }
                            else
                            {
                                if (w.Hyperlinks == null)
                                {
                                    w.Hyperlinks = new List<Hyperlink>();
                                }
                                w.Hyperlinks.Add(h);
                                d.Hyperlinks.Add(w);
                            }
                        }
                        #endregion

                        #region Special reports
                        if (w.CustomerAction != null && w.CustomerAction.Length > 4)
                        {
                            d.SpecialReportCustomerAction.Add(w);
                        }

                        if (w.SupplierAction != null && w.SupplierAction.Length > 4)
                        {
                            d.SpecialReportSupplierAction.Add(w);
                        }

                        // Allocation = “ESE”+ SW
                        if (w.ESE && w.Software)
                        {
                            d.SpecialReportESESW.Add(w);
                        }

                        // Allocation = “ESE” + HW (20)
                        if (w.ESE && w.Hardware)
                        {
                            d.SpecialReportESEHW.Add(w);
                        }
                        #endregion

                        // Review Report:
                        w.FillApprovals(myP.WorkitemApprovals, myP.Users);
                        d.Review.CheckWorkitem(w);
                    }
                }
            }

            return View(cv);
        }
    }
}