using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PolarionReports.BusinessLogic;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using System.Configuration;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class Check20Controller : Controller
    {
        // GET: Check20
        public ActionResult Index()
        {
            return View();
        }

        #region public ActionResult Project(string id)
        /// <summary>
        /// Neue Implemtierung des 20-Checks
        /// </summary>
        /// <remarks>
        /// History:
        /// 2018-09-27 Start Implementierung
        /// </remarks>
        /// <param name="id">Id des Projektes zB: E18008</param>
        /// <returns></returns>
        public ActionResult Project(string id, string docFilter)
        {
            string ErrorMsg;
            string Documentname = "";
            string Browser;

            DatareaderP dr = new DatareaderP();
            Project myP;
            DocumentName DocNameToCheck;
            CheckNames checkNames = new CheckNames();
            List<Uplink> TempUplinks = new List<Uplink>();
            Workitem Tempworkitem = new Workitem();

            Check20BL check20BL = new Check20BL();
            Check30Review check30Review = new Check30Review();
            CheckVerification checkVerification = new CheckVerification();

            Browser = Request.Browser.Browser;

            myP = InitProject.GetProject(id, Browser);
            ViewBag.Error = myP.DatabaseErrorMessage;

            Check20ViewModel cv = new Check20ViewModel
            {
                Projectname = myP.C_name,
                Errorname = "Missing Uplinks:",
                PolarionLink = "http://" + Topol.PolarionServer + "/polarion/#/project/" + myP.C_id + "/workitem?id=",
                Documents = new List<Document>(),
                Tooltip20 = new Tooltip20(),
                TooltipReview = new TooltipReview()
            };

            cv.TableLinkError = new TableLinkError();
            cv.TableLink      = new TableLink();
            cv.RequirementList          = new RequViewModel();
            cv.RequApprovalList         = new RequApprovalViewModel();
            cv.RequirementErrorList     = new RequErrorViewModel();
            cv.RequTestcaseList         = new RequTestcaseListViewModel();
            cv.RequTestcaseErrorList    = new RequTestcaseErrorListViewModel();
            cv.RequTestcaseUnlinkedList = new RequTestcaseUnlinkedListViewModel();

            // 20 Dokument suchen
            foreach (Document d in myP.Documents)
            {
                if (docFilter != null)
                { 
                    if (!d.DocName.OriginalDocumentname.Contains(docFilter))
                    {
                        continue;
                    }
                }
                d.SetBrowserType(myP.Browser); // for max URL length
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

                    d.IncorrectlyLinkedElementRequirements = new List<WorkitemLinkError>();

                    d.Missing30Links = new List<Workitem>();

                    d.Links30 = new List<Workitem>(); // ersetzt durch folgende Liste
                    d.LinkedElementRequirements = new List<WorkitemLinks>();

                    d.StatusDraft = new List<Workitem>();
                    d.StatusClarificationNeeded = new List<Workitem>();
                    d.StatusReviewNeeded = new List<Workitem>();
                    d.StatusDeferred = new List<Workitem>();
                    d.StatusTBD = new List<Workitem>();
                    d.SeverityShouldHave = new List<Workitem>();
                    d.AllocMissing = new List<Workitem>();
                    d.AllocInvalid = new List<Workitem>();  // Ersetzt AllocInterface und AllocESE
                    d.AllocInterface = new List<Workitem>();
                    d.AllocESE = new List<Workitem>();
                    d.FuncReqHSM = new List<Workitem>();
                    d.MissingHyperlink = new List<Workitem>();
                    d.Hyperlinks = new List<Workitem>();
                    d.SpecialReportCustomerAction = new List<Workitem>();
                    d.SpecialReportSupplierAction = new List<Workitem>();
                    d.SpecialReportESESW = new List<Workitem>();    // 2019-03-29
                    d.SpecialReportESEHW = new List<Workitem>();    // 2019-03-29
                    d.SafetyRelevant = new List<Workitem>();
                    d.CybersecurityRelevant = new List<Workitem>();
                    d.WorkitemsInBin = new List<Workitem>();

                    // Listen im neuem Check20

                    d.HighLevelElementRequirementsWithoutBreakDown = new List<Workitem>();
                    d.BreakDown30InterfaceRequirements = new List<Workitem>();
                    d.BreakDown40SWRequirements = new List<Workitem>();
                    d.BreakDown40HWRequirements = new List<Workitem>();

                    d.UnlinkedSoftwareElementRequirements = new List<Workitem>();
                    d.IncorrectlyLinkedSoftwareElementRequirements = new List<WorkitemLinkError>();
                    d.LinkedSoftwareElementRequirementsWithInvalidAllocation = new List<WorkitemLinks>();
                    d.LinkedSoftwareElementRequirements = new List<WorkitemLinks>();

                    d.UnlinkedHardwareElementRequirements = new List<Workitem>();
                    d.IncorrectlyLinkedHardwareElementRequirements = new List<WorkitemLinkError>();
                    d.LinkedHardwareElementRequirementsWithInvalidAllocation = new List<WorkitemLinkError>();
                    d.LinkedHardwareElementRequirements = new List<WorkitemLinkError>();

                    d.LinkedElementRequirementsWithInvalidAllocation = new List<WorkitemLinks>();

                    d.HighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW = new List<Workitem>();
                    d.HighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements = new List<Workitem>();
                    d.HighLevelHWRequirementsToBeSpecifiedInSWRequirements = new List<Workitem>();
                    d.HighLevelSWRequirementsToBeSpecifiedInHWRequirements = new List<Workitem>();
                    d.HighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation = new List<Workitem>();
                    d.HighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation = new List<Workitem>();
                    d.HighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation = new List<Workitem>();
                    d.HighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements = new List<Workitem>();
                    d.HighLevelElementRequirementsWithBreakDownRequirementsByCustomer = new List<Workitem>();
                    d.HighLevelElementRequirementsWithBreakDownRequirementsByZKW = new List<Workitem>();

                    d.HighLevelRequirementsWithoutAnyBreakDownRequirements = new List<Workitem>();

                    d.HwEseRequirementsNotLinkedToChecklist = new List<Workitem>();
                    d.HwEseRequirementsLinkedToChecklist = new List<WorkitemLinkError>();

                    // 2019-08-09 Neue Listen Verification
                    d.UnlinkedElementRequirements = new List<Models.TableRows.RequTestcaseUnlinked>();
                    d.IncorrectlyLinkedElementRequirementsToTestcases = new List<Models.TableRows.RequTestcaseError>();
                    d.LinkedElementRequirementsToTestcases = new List<Models.TableRows.RequTestcase>();
                    d.ElementRequirementsWithMissingVerificationProperties = new List<Models.TableRows.RequTestcase>();

                    // 2019-11-13
                    // 2020-01-23 New implementation:
                    d.DocSignatureStatus = new DocSignatureStatusBL();
                    d.DocSignatureStatus.DocSignatureStatusList = dr.GetDocSignatureStatus(d.C_pk, out string errorGetDocSignatureStatus);
                    d.DocSignatureStatus.MakeView(d);

                    d.Review = new Review(myP.Users);

                    d.PolarionDocumentLink = $"http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/{d.C_id}?selection=";

                    d.PolarionTableLink = $"http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/"
                        + System.Uri.EscapeDataString(d.C_id) + "?query=";

                    // auch x.type == 'customer requirement' -> mit Hinweismeldung
                    foreach (Workitem w in myP.Workitems.FindAll(x => x.DocumentId == d.C_pk && x.Type == "requirement"))
                    {
                        if (w.Id == "E19006-11552")
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
                                            LinkToCustomerRequirement = false;
                                            InErrorList = true;
                                        }
                                        // break;
                                    }
                                    // Debug.WriteLine(w.Title + " -> " + DocNameToCheck.OriginalDocumentname);
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
                                                        InErrorList = true;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                // Behandlung von uplinks zu nicht "requirements"
                                                // 2018-11-19 Wolfgang "Links zu nicht Requirements einfach ignorieren!!"
                                                /*
                                                DocNameToCheck = new DocumentName(myP.GetUplinkDocumentname(u));
                                                if (DocNameToCheck.Level != "10" && DocNameToCheck.Level != "20")
                                                {
                                                    u.UplinkToWrongDocument = true;
                                                    u.ErrorMessage = "Incorrect link dir " + DocNameToCheck.Level;
                                                    if (!InWrongDocumentList)
                                                    {
                                                        // um doppelte Eintraäge zu verhindern
                                                        d.WrongDocumentLinkList.Add(w);
                                                        InWrongDocumentList = true;
                                                    }
                                                }
                                                */
                                            }
                                        }
                                    }

                                }

                            } // Ende foreach uplink
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

                        bool In30ErrorList = false;

                        if (check20BL.IsLinkedElementRequirementsWithInvalidAllocation(myP, cv, w, out Workitem LinkedWorkitem))
                        {
                            Documentname = myP.GetDocumentname(LinkedWorkitem.DocumentId);
                            WorkitemLinks workitemLinks = new WorkitemLinks();
                            workitemLinks.Workitem = w;

                            WorkitemViewLink workitemViewLink = new WorkitemViewLink
                            {
                                Id = LinkedWorkitem.Id,
                                Title = LinkedWorkitem.Title,
                                Type = LinkedWorkitem.Type,
                                LinkDisplay = Documentname.Substring(0, 3) + ":" + LinkedWorkitem.Type.Substring(0, 1).ToUpper() + ":" + LinkedWorkitem.Id,
                                PolarionDocumentLink = myP.GetPolarionLink(Documentname, LinkedWorkitem.Id)
                            };

                            workitemLinks.LinkedWorkitems.Add(workitemViewLink);
                            d.LinkedElementRequirementsWithInvalidAllocation.Add(workitemLinks);
                            In30ErrorList = true;
                        }

                        if (w.IsHSM())
                        {
                            bool NotFound = true;
                            WorkitemLinks wl = new WorkitemLinks();
                            wl.LinkedWorkitems = new List<WorkitemViewLink>();
                            wl.Workitem = w;

                            TempUplinks = myP.Uplinks.FindAll(x => x.UplinkId == w.C_pk);
                            if (w.Interface)
                            {
                                // 1. Check
                                foreach (Uplink u in TempUplinks)
                                {
                                    Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.WorkitemId);
                                    if (Tempworkitem == null)
                                    {
                                        continue;   // Uplink auf workitem "heading"
                                    }
                                    Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                                    if (Documentname.Length < 2)
                                    {
                                        continue;   // Uplink auf nicht vorhandenes Document
                                    }
                                    if (u.Role == "parent")
                                    {
                                        continue; // uplinks zu parent ignorieren
                                    }
                                    if (Documentname.Substring(0, 2) == "30" && Documentname.Contains("Element"))
                                    {
                                        if (Tempworkitem.Type != "interface")
                                        {
                                            if (Tempworkitem.Type != "requirement" && Tempworkitem.Type != "feature")
                                            {
                                                // Fehler -> Workitem in Fehlerliste - alle Requirements ignorieren
                                                WorkitemLinkError workitemLinkError = new WorkitemLinkError();
                                                workitemLinkError.Workitem = w;
                                                workitemLinkError.LinkedWorkitem = Tempworkitem;
                                                workitemLinkError.IdLinkedWorkitem = Tempworkitem.Id;
                                                workitemLinkError.ErrorMsg = "not linked to INT";
                                                workitemLinkError.PolarionLink = myP.GetPolarionLink(Documentname, Tempworkitem.Id);

                                                d.IncorrectlyLinkedElementRequirements.Add(workitemLinkError);
                                                In30ErrorList = true;
                                                NotFound = false;
                                                break; // Nach dem 1. Fehler dieses Workitem nicht weiter prüfen
                                            }
                                        }
                                        else
                                        {
                                            // richtigen Link gefunden
                                            if (Tempworkitem.Type == "interface")
                                            {
                                                WorkitemViewLink wvl = new WorkitemViewLink();
                                                wvl.Id = Tempworkitem.Id;
                                                wvl.Title = Tempworkitem.Title;
                                                wvl.Type = Tempworkitem.Type;
                                                wvl.PolarionDocumentLink = myP.GetPolarionLink(Documentname, Tempworkitem.Id);
                                                wvl.LinkDisplay = Documentname.Substring(0, 3) + ":" + Tempworkitem.Type.Substring(0, 1).ToUpper() + ":" + Tempworkitem.Id;
                                                wl.LinkedWorkitems.Add(wvl);
                                                NotFound = false;
                                            }
                                            // 2018-10-25 alle Links überprüfen -- break;
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
                                            if (Tempworkitem.Type != "requirement" && Tempworkitem.Type != "feature")
                                            {
                                                WorkitemLinkError workitemLinkError = new WorkitemLinkError();
                                                workitemLinkError.Workitem = w;
                                                if (Tempworkitem == null)
                                                {
                                                    // Link auf Header:
                                                    Tempworkitem = new Workitem();
                                                    Tempworkitem.Id = "Header";
                                                    Tempworkitem.Type = "Header";
                                                }
                                                workitemLinkError.LinkedWorkitem = Tempworkitem;
                                                workitemLinkError.IdLinkedWorkitem = Tempworkitem.Id;
                                                workitemLinkError.ErrorMsg = "not linked to COMP";
                                                workitemLinkError.PolarionLink = myP.GetPolarionLink(Documentname, Tempworkitem.Id);

                                                d.IncorrectlyLinkedElementRequirements.Add(workitemLinkError);
                                                In30ErrorList = true;
                                                NotFound = false;
                                                break; // Nach dem 1. Fehler dieses Workitem nicht weiter prüfen
                                            }
                                        }
                                        else
                                        {
                                            // richtigen Link gefunden
                                            if (Tempworkitem.Type == "component")
                                            {
                                                WorkitemViewLink wvl = new WorkitemViewLink();
                                                wvl.Id = Tempworkitem.Id;
                                                wvl.Title = Tempworkitem.Title;
                                                wvl.Type = Tempworkitem.Type;
                                                wvl.PolarionDocumentLink = myP.GetPolarionLink(Documentname, Tempworkitem.Id);
                                                wvl.LinkDisplay = Documentname.Substring(0, 3) + ":" + Tempworkitem.Type.Substring(0, 1).ToUpper() + ":" + Tempworkitem.Id;
                                                wl.LinkedWorkitems.Add(wvl);
                                                NotFound = false;
                                            }
                                            // 2018-10-25 Tests nicht beenden -> alle Links müssen untersuchet werden -- break;
                                        }
                                    }
                                }
                            }
                            if (NotFound)
                            {
                                if (!w.Project)
                                {
                                    // Fehlender Link in 30 Element Architecture 
                                    // 2018-08-17: keine Workitems mit Allocation "Project"
                                    d.Missing30Links.Add(w);
                                }
                            }
                            else
                            {
                                // Link gefunden in die Liste eintragen
                                if (wl.LinkedWorkitems.Count > 0)
                                {
                                    d.LinkedElementRequirements.Add(wl);
                                    // d.Links30.Add(w);
                                }   
                            }
                        }

                        #endregion

                        #region check reference in 40 Document
                        // ist im neuem Check20 nicht mehr notwendig
                        #endregion

                        #region check reference in 30 Document
                        // ist im neuem Check20 nicht mehr notwendig
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
                            w.TextError = "Incorrect Interface Allocation";
                            d.AllocInvalid.Add(w);
                        }

                        // Workitem auf "Nur Project" überprüfen
                        if (w.ProjectOnly())
                        {
                            w.TextError = "Invalid Procject alloc";
                            d.AllocInvalid.Add(w);
                        }

                        // Allocation = “ESE” + other than “HW, SW, Mech”
                        if (w.ESE)
                        {
                            if (!w.Hardware && !w.Software)
                            {
                                //w.TextErrorESE = "not HW,SW";
                                w.TextError = "Incorrect ARCH allocation";
                                d.AllocInvalid.Add(w);
                            }
                            else if (w.Project)
                            {
                                //w.TextErrorESE = "Project";
                                w.TextError = "ARCH and Project";
                                d.AllocInvalid.Add(w);
                            }
                            else if ((w.Hardware) && !w.SE)
                            {
                                w.TextError = "Incorrect ARCH allocation";
                                d.AllocInvalid.Add(w);
                            }
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
                        if (w.Id == "E19006-10943")
                        {
                            Debug.WriteLine(w.Id);
                        }
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

                        if (w.ReferredDoc != null)
                        {
                            HyperlinKHelper hlh = new HyperlinKHelper();
                            List<Hyperlink> hl = hlh.GetHyperlinksFromWorkitemDescription(w);
                            if (hl.Count > 0)
                            {
                                if (w.Hyperlinks == null)
                                {
                                    w.Hyperlinks = hl;
                                }
                                else
                                {
                                    w.Hyperlinks.AddRange(hl);
                                }
                                if (!d.Hyperlinks.Contains(w))
                                {
                                    d.Hyperlinks.Add(w);
                                }
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
                            if (!d.AllocESE.Contains(w))
                            {
                                d.SpecialReportESEHW.Add(w);
                                check20BL.LinkageToHWChecklist(myP, d, cv, w);
                            }
                        }

                        // Safety-relevat Workitems
                        if (w.Asil != null)
                        {
                            if (w.Asil.ToUpper() != "QM" && w.Asil != "notrelevant")   // QM || qm || notrelevant
                            {
                                d.SafetyRelevant.Add(w);
                            }
                        }

                        // Cybersecurity-relevant Workitems
                        if (w.Cs != null)
                        {
                            if (w.Cs == "csRelevant")
                            {
                                d.CybersecurityRelevant.Add(w);
                            }
                        }


                        #endregion Special reports

                        // Break Down Analyse:
                        check20BL.BreakDownAnalyse(myP, d, cv, w);

                        // Linkage to 50 SW Architectural Design
                        check20BL.LinkageTo50SW(myP, d, cv, w);

                        // Linkage to 50 HW Architectural Design
                        // Neues Dokumentenformat Links von 50 in 20 werden nicht mehr überprüft!
                        // 2018-12-03 email von Wolfgang
                        // check20BL.LinkageTo50HW(myP, d, cv, w);

                        // Review Report:
                        if (w.Id == "E19006-10000")
                        {
                            Debug.WriteLine(w.Id);
                        }
                        w.FillApprovals(myP.WorkitemApprovals, myP.Users);
                        d.Review.CheckWorkitem(w);
                        d.Review.WorkitemPerClarificationList.OrderBy(x => x.SortOrder);

                        // Verification Checks: 2019-11-25 Verification checks nur für Hard- oder Software und nicht Project
                        if ((w.Hardware || w.Software) && !w.Project)
                        {
                            checkVerification.Check(dr, myP, d, w);
                        }
                    }
                } // Ende im richtigen Document

                // Checklist Report - für jedes 20 Document den Report aufrufen
                check30Review.Check(myP, d);

            } // Ende foreach Document

            return View(cv);
        }
        #endregion
    }
}