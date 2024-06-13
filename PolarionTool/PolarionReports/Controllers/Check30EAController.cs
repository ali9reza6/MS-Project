using PolarionReports.Custom_Helpers;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolarionReports.Controllers
{
    public class Check30EAController : Controller
    {
        // GET: Check30EA
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Project(string id)
        {
            string Browser = Request.Browser.Browser;
            Project myP = InitProject.GetProject(id, Browser);
            Check30EAViewModel cv = new Check30EAViewModel();
            Workitem Tempworkitem;
            Workitem LinkedWorkitem;
            bool InList;
            bool WorkitemLinkedWith40SW = false;
            bool InAssociatedAdditionalRequirements = false;
            bool InIncorrectlyAssociatedAdditionalRequirements = false;
            string LinkedDocumentname;

            string Test1 = "";
            string Test2 = "";

            if (id == null)
            {
                return View();
            }

            cv.Document = myP.Documents.FirstOrDefault(d => d.C_id.Contains("30 Element Architecture"));
            if (cv.Document == null)
            {
                // 30 Element Architecture nicht gefunden
                cv.ErrorMsg = "Document with ID: '30 Element Architecture' not found";
                return View(cv);
            }

            Test2 = System.Uri.EscapeDataString(cv.Document.C_id);

            cv.Document.SetBrowserType(myP.Browser); //  for max URL length
            cv.Projectname = myP.C_name;
            cv.PolarionLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/workitem?id=";
            cv.Document.PolarionDocumentLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/wiki/Specification/{cv.Document.C_id}?selection=";

            cv.Document.PolarionTableLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/wiki/Specification/"
                        + System.Uri.EscapeDataString(cv.Document.C_id) + "?query=";

            cv.PolarionTableLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/wiki/Specification/"
                                    + System.Uri.EscapeDataString(cv.Document.C_id) + "?query=";

            cv.Tooltip = new Tooltip30EA();
            cv.TooltipReview = new TooltipReview();

            cv.UnassociatedAdditionalRequirements = new List<Workitem>();
            cv.IncorrectlyAssociatedAdditionalRequirements = new List<Workitem>();
            cv.AssociatedAdditionalRequirements = new List<Workitem>();

            cv.UnlinkedAdditionalInterfaceRequirements = new List<Workitem>();
            cv.LinkedAdditionalRequirementsWithInvalidAllocation = new List<Workitem>();
            cv.LinkedAdditionalInterfaceRequirements = new List<Workitem>();

            cv.UnlinkedAdditionalSoftwareRequirements = new List<Workitem>();
            cv.LinkedAdditionalRequirementsWithInvalidAllocation40SW = new List<Workitem>();
            cv.LinkedAdditionalSoftwareRequirements = new List<Workitem>();

            cv.IncorrectlyLinkedSoftwareRequirements = new List<Workitem>();
            cv.LinkedAdditionalRequirementsWithInvalidAllocation50SW = new List<Workitem>();
            cv.LinkedAdditionalSoftwareRequirements50SW = new List<Workitem>();

            cv.UnlinkedAdditionalHardwareRequirements = new List<Workitem>();
            cv.LinkedAdditionalRequirementsWithInvalidAllocation40HW = new List<Workitem>();
            cv.LinkedAdditionalHardwareRequirements = new List<Workitem>();

            cv.IncorrectlyLinkedHardwareRequirements = new List<Workitem>();
            cv.LinkedAdditionalRequirementsWithInvalidAllocation50HW = new List<Workitem>();
            cv.LinkedAdditionalHardwareRequirements50HW = new List<Workitem>();

            cv.StatusTBD = new List<Workitem>();
            cv.StatusDraft = new List<Workitem>();
            cv.StatusClarificationNeeded = new List<Workitem>();
            cv.StatusDeferred = new List<Workitem>();
            cv.SeverityShouldHave = new List<Workitem>();
            cv.RejectedList = new List<Workitem>();

            cv.AllocMissing = new List<Workitem>();
            cv.InvalidAllocation = new List<Workitem>();
            cv.AllocInterface = new List<Workitem>();
            cv.AllocESE = new List<Workitem>();

            cv.Document.FuncReqHSM = new List<Workitem>();

            cv.ComponentsWithoutLinkedRequirements = new List<Workitem>();
            cv.InterfacesWithoutLinkedRequirements = new List<Workitem>();
            cv.SpecialReportESESW = new List<Workitem>();
            cv.SpecialReportESEHW = new List<Workitem>();

            cv.WorkitemInBin = new List<Workitem>();

            cv.Document.Review = new Review(myP.Users);

            foreach (Workitem w in myP.Workitems.FindAll(x => x.DocumentId == cv.Document.C_pk))
            {
                InAssociatedAdditionalRequirements = false;
                InIncorrectlyAssociatedAdditionalRequirements = false;
                WorkitemLinkedWith40SW = false;

                if (w.Id == "E18010-6252")
                {
                    Debug.WriteLine(w.Id);
                }

                if (w.FillCheckUplink(myP.Uplinks) == 0)
                {
                   // no uplinks
                }

                if (w.InBin)
                {
                    cv.WorkitemInBin.Add(w);
                    continue;
                }

                if (w.Type == "requirement")
                {
                    // Rejected Workitems
                    if (w.Status == "rejected")
                    {
                        cv.RejectedList.Add(w);
                        continue;
                    }

                    #region Linkage to 30 Element Architecture
                    InList = false;
                    bool Reference = false;
                    #region debugging
                    //--------------------------
                    // Only for debugging:
                    if (w.Id == "E17011-2507")
                    {
                        Debug.WriteLine(w.Id);
                    }
                    //--------------------------
                    #endregion

                    foreach (Uplink u in w.Uplinks)
                    {
                        Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                        if (Tempworkitem == null)
                        {
                            // Linked Workitem not Found -> Link to other Project or error ???
                        }
                        else
                        {
                            if (u.Role == "parent")
                            {
                                if (Tempworkitem.DocumentId != w.DocumentId)
                                {
                                    // Parent zeigt auf anderes Document -> diesen Parent ignorieren, kommt aus einer Referenz
                                    continue;
                                }

                                Tempworkitem = myP.GetParentLevel0(Tempworkitem, out Reference);
                                
                                // Check Type of parent
                                if (Tempworkitem.Type != "component" && 
                                    Tempworkitem.Type != "interface" &&
                                    !Reference)
                                {
                                    if (!InList)
                                    {
                                        w.TextError30Interface = "link to wrong type";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else if (w.Interface && Tempworkitem.Type != "interface")
                                {
                                    if (Tempworkitem.Type == "requirement")
                                    {
                                        // Sub-Element eines Requirements -> OK
                                        if (!InList)
                                        {
                                            if (!InAssociatedAdditionalRequirements)
                                            {
                                                cv.AssociatedAdditionalRequirements.Add(w);
                                                InAssociatedAdditionalRequirements = true;
                                                InList = true;
                                            }
                                        }
                                    }
                                    if (!InList)
                                    {
                                        w.TextError30Interface = "not linked to INT";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else if (!w.Interface && Tempworkitem.Type !="component")
                                {
                                    if (Tempworkitem.Type == "requirement")
                                    {
                                        // Sub-Element eines Requirements -> OK
                                        if (!InList)
                                        {
                                            if (!InAssociatedAdditionalRequirements)
                                            {
                                                cv.AssociatedAdditionalRequirements.Add(w);
                                                InAssociatedAdditionalRequirements = true;
                                                InList = true;
                                            }
                                        }
                                    }
                                    if (Tempworkitem.Type == "interface")
                                    {
                                        // Nicht Allociertes Interface zeigt auf Interface -> 
                                        if (!InList)
                                        {
                                            w.TextError30Interface = "non Interface linked to Int";
                                            cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                            InIncorrectlyAssociatedAdditionalRequirements = true;
                                            InList = true;
                                        }
                                    }
                                    if (!InList)
                                    {
                                        w.TextError30Interface = "not linked to COMP";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else
                                {
                                    if (w.IsHSM()) // ohne Allocation nicht in Liste der korrekten Elemente -> kommt in Fehlerliste
                                    {
                                        cv.AssociatedAdditionalRequirements.Add(w);
                                        InAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                            }
                        }
                    }

                    // Unabhängig vom Parent Ergebis, müssen auch die uplinks auf Richtigkeit geprüft werden
                    //---------------------------------------------------------------------------------------
                    InList = false; 
                    foreach (Uplink u in w.Uplinks)
                    {
                        if (u.Role == "relates_to")
                        {
                            // Links in andere Dokumente ignorieren
                            LinkedDocumentname = myP.GetUplinkDocumentname(u);
                            if (LinkedDocumentname != cv.Document.C_id)
                            {
                                // Link ignorieren
                            }
                            else
                            {
                                // Link in gleiches Dokument -> Regeln prüfen
                                // Check Type of parent
                                Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                                if (Tempworkitem == null)
                                {
                                    // Linked Workitem not Found -> Link to other Project or error ???

                                }
                                else
                                {
                                    if (Tempworkitem.Type != "component" &&
                                        Tempworkitem.Type != "interface" &&
                                        Tempworkitem.Type != "requirement")
                                    {
                                        if (!InList)
                                        {
                                            w.TextError30Interface = "link to wrong type";
                                            cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                            InIncorrectlyAssociatedAdditionalRequirements = true;
                                            InList = true;
                                        }
                                    }
                                    else if (w.Interface && Tempworkitem.Type != "interface")
                                    {
                                        if (!InList)
                                        {
                                            w.TextError30Interface = "not linked to INT";
                                            cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                            InIncorrectlyAssociatedAdditionalRequirements = true;
                                            InList = true;
                                        }
                                    }
                                    else if (!w.Interface && Tempworkitem.Type != "component")
                                    {
                                        if (!InList)
                                        {
                                            w.TextError30Interface = "not linked to COMP";
                                            cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                            InIncorrectlyAssociatedAdditionalRequirements = true;
                                            InList = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!InAssociatedAdditionalRequirements)
                                        {
                                            // noch nicht Liste
                                            cv.AssociatedAdditionalRequirements.Add(w);
                                            InAssociatedAdditionalRequirements = true;
                                            InList = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    InList = false;
                   
                    // Uplinks haben kein Ergebnis geliefert => Downlinks überprüfen
                    if (w.FillCheckDownlink(myP.Uplinks) == 0)
                    {
                        // keine Downlinks vorhanden
                    }
                    else
                    {
                        // Downlinks checken
                        foreach(Uplink d in w.Downlinks)
                        {
                            if (d.Role == "relates_to")
                            {
                                Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                                if (Tempworkitem.Type == "feature")
                                {
                                    // Links auf Features werden im 30A Check nicht berücksichtigt
                                    continue;
                                }
                                if (Tempworkitem.Type != "component" &&
                                    Tempworkitem.Type != "interface" &&
                                    Tempworkitem.Type != "requirement")
                                {
                                    if (!InList)
                                    {
                                        w.TextError30Interface = "link to wrong type";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else if (w.Interface && Tempworkitem.Type != "interface")
                                {
                                    if (!InList && !InIncorrectlyAssociatedAdditionalRequirements)
                                    {
                                        w.TextError30Interface = "not linked to INT";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else if (!w.Interface && Tempworkitem.Type != "component")
                                {
                                    if (!InList)
                                    {
                                        w.TextError30Interface = "not linked to COMP";
                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                        InList = true;
                                    }
                                }
                                else
                                {
                                    if (!InAssociatedAdditionalRequirements)
                                    {
                                        cv.AssociatedAdditionalRequirements.Add(w);
                                        InAssociatedAdditionalRequirements = true;
                                        InList = true;
                                        // Das Workitem hat eine richtige Verlinkung
                                        // Falls es einen falschen Parent hatte wird dieser wieder entfernt,
                                        // Da die Verlinkung eine höhere Priorität hat
                                        if (cv.IncorrectlyAssociatedAdditionalRequirements.Contains(w))
                                        {
                                            cv.IncorrectlyAssociatedAdditionalRequirements.Remove(w);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    
                    // Alle Uplinks und Downlinks checked
                    if (!InAssociatedAdditionalRequirements && !InIncorrectlyAssociatedAdditionalRequirements)
                    {
                        cv.UnassociatedAdditionalRequirements.Add(w);
                        InAssociatedAdditionalRequirements = true;
                    }
                    #endregion

                    #region Linkage to 30 Interface Requirements
                    // For every Interface in the Element Architecture additional (HW, SW or Mech) interface
                    // requirements may be defined. Additional hardware/ software interface requirements must be
                    // referenced in 30 Interface Requirements or linked to 30 Interface Requirements.In addition,
                    // every Interface in 30 Element Architecture must be referenced in 30 Interface Requirements.
                    // There may be several Interface Requirement documents based on different 20-docs.In this
                    // case, the Report checks references and links to all Interface Requirement documents; it is not
                    // always possible to check whether a requirement is linked to the correct Interface Requirement document.

                    List<WorkitemReference> wrl = myP.WorkitemReferences.FindAll(x => x.WorkitemID == w.C_pk);

                    if (w.Interface && w.IsHSM())
                    {
                        // Check References in 30 Interface
                        // Das Workitem ist ein Software-Workitem -> alle References zu diesem Element suchen:
                        bool NoRef30Interface = true;
                        foreach (WorkitemReference wr in wrl)
                        {
                            if (wr.DocumentName.Substring(0, 2) == "30" &&
                               (wr.DocumentName.Contains("Interface")))
                            {
                                NoRef30Interface = false;
                                cv.LinkedAdditionalInterfaceRequirements.Add(w);
                            }
                        }
                        if (NoRef30Interface)
                        {
                            // Check if link instead of reference
                            foreach (Uplink u in w.Uplinks)
                            {
                                if (u.Role == "relates_to")
                                {
                                    Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                                    if (Tempworkitem == null)
                                    {
                                        // Linked Workitem not Found -> Link to other Project or error ???
                                    }
                                    else
                                    {
                                        LinkedDocumentname = myP.GetUplinkDocumentname(u);
                                        if (LinkedDocumentname.Substring(0, 2) == "30" &&
                                            LinkedDocumentname.Contains("Interface"))
                                        {
                                            // Link in 30 Interface vorhanden
                                            NoRef30Interface = false;
                                            cv.LinkedAdditionalInterfaceRequirements.Add(w);
                                        }
                                    }
                                }
                            }
                        }
                        if (NoRef30Interface)
                        {
                            // Workitem in Fehlerliste
                            cv.UnlinkedAdditionalInterfaceRequirements.Add(w);
                        }
                    }

                    if (!w.Interface)
                    {
                        // Ein nicht interface darf nicht in ein 30 Interface Document referenced oder linked sein
                        InList = false;
                        foreach (WorkitemReference wr in wrl)
                        {
                            if (wr.DocumentName.Substring(0, 2) == "30" &&
                               (wr.DocumentName.Contains("Interface")))
                            {
                                w.TextError30Interface = "Invalid Reference to 30 Interface";
                                cv.LinkedAdditionalRequirementsWithInvalidAllocation.Add(w);
                                InList = true;
                                break;
                            }
                        }
                        if (!InList)
                        {
                            // Workitem noch auf falsche Links prüfen
                            foreach (Uplink u in w.Uplinks)
                            {
                                if (u.Role == "relates_to")
                                {
                                    Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                                    if (Tempworkitem == null)
                                    {
                                        // Linked Workitem not Found -> Link to other Project or error ???
                                    }
                                    else
                                    {
                                        LinkedDocumentname = myP.GetUplinkDocumentname(u);
                                        if (LinkedDocumentname.Substring(0, 2) == "30" &&
                                            LinkedDocumentname.Contains("Interface"))
                                        {
                                            // Link in 30 Interface vorhanden
                                            w.TextError30Interface = "Invalid Link to 30 Interface";
                                            cv.LinkedAdditionalRequirementsWithInvalidAllocation.Add(w);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region 4.1.3 Linkage to 40 Software Requirements
                    // For every Component or Interface (element) in the Element Architecture additional (HW, SW or
                    // Mech) requirements may be defined. Additional software requirements must be referenced in
                    // 40 Software Requirements or linked to 40 Software Requirements.
                    // There may be several Software Requirement documents based on different 20 - docs.In this
                    // case, the Report checks references and links to all 40 Software Requirement documents; it is
                    // not always possible to check whether a requirement is linked to the correct Software
                    // Requirement document.
                    
                    if (w.Software)
                    {
                        // Software requirement
                        InList = false;
                        // Check reference to 40 SW
                        foreach (WorkitemReference wr in wrl)
                        {
                            if (wr.DocumentName.Substring(0, 2) == "40" &&
                               (wr.DocumentName.Contains("SW")))
                            {
                                cv.LinkedAdditionalSoftwareRequirements.Add(w);
                                WorkitemLinkedWith40SW = true;
                                InList = true;
                                break;
                            }
                        }
                        if (!InList)
                        {
                            // Check Link to 40 SW
                            LinkedWorkitem = w.CheckLinkTo(myP, "40", "SW", "relates_to", "requirement");
                            if (LinkedWorkitem != null)
                            {
                                if (LinkedWorkitem.Software)
                                {
                                    cv.LinkedAdditionalSoftwareRequirements.Add(w);
                                    WorkitemLinkedWith40SW = true;
                                }
                                else
                                {
                                    cv.LinkedAdditionalRequirementsWithInvalidAllocation40SW.Add(w);
                                }
                                InList = true;
                            }
                        }
                        if (!InList)
                        {
                            // no reference or link found
                            cv.UnlinkedAdditionalSoftwareRequirements.Add(w);
                        }
                    }
                    #endregion

                    #region 4.1.4 Linkage to 50 SW Architectural Design
                    // For every Component or Interface (element) in the Element Architecture additional (HW, SW or
                    // Mech) requirements may be defined. Every additional software requirement must be related to
                    // a Component or Interface of the Software Architectural Design.
                    // There may be several SW Architectural Design documents based on different 20-docs. In this
                    // case, the Report checks references and links to all 50 SW Architectural Design documents; it is
                    // not always possible to check whether a requirement is linked to the correct Software
                    // Architectural Design document.
                    if (w.Software)
                    {
                        if (w.Interface)
                        {
                            // Check Link to 50 SW
                            LinkedWorkitem = w.CheckLinkTo(myP, "50", "SW", "relates_to", "interface");
                        }
                        else
                        {
                            // Check Link to 50 SW
                            LinkedWorkitem = w.CheckLinkTo(myP, "50", "SW", "relates_to", "component");
                        }

                        if (LinkedWorkitem != null)
                        {
                            if (w.Interface)
                            {
                                if (LinkedWorkitem.Type == "interface")
                                {
                                    cv.LinkedAdditionalSoftwareRequirements50SW.Add(w);
                                }
                                else
                                {
                                    w.TextError50SW = "Interface linked to Component";
                                    cv.IncorrectlyLinkedSoftwareRequirements.Add(w);
                                }
                            }
                            else
                            {
                                // no interface
                                if (LinkedWorkitem.Type == "component")
                                {
                                    cv.LinkedAdditionalSoftwareRequirements50SW.Add(w);
                                }
                                else
                                {
                                    w.TextError50SW = "non-Interface linked to Interface";
                                    cv.IncorrectlyLinkedSoftwareRequirements.Add(w);
                                }
                            }
                        }
                        else
                        {
                            /* 2018-07-19 Diese Prüfung/Eintrag entfert weiss nicht mehr warum der hier entstanden ist
                             * das ein Workitem in die Liste "AssociatedAdditionalRequirements" eingetragen wird ????
                            if (!WorkitemLinkedWith40SW)
                            {
                                if (!InAssociatedAdditionalRequirements)
                                {
                                    cv.AssociatedAdditionalRequirements.Add(w);
                                }
                            }
                            2018-07-19 */
                        }

                    }
                    else
                    {
                        // no Software -> no link to 50 SW allowed
                        if (w.CheckLinkExists(myP,"50","SW","relates_to"))
                        {
                            cv.LinkedAdditionalRequirementsWithInvalidAllocation.Add(w);
                        }
                    }
                    #endregion

                    #region 4.1.5 Linkage to 40 Hardware Requirements
                    // For every Component or Interface (element) in the Element Architecture additional (HW, SW or
                    // Mech) requirements may be defined. Additional hardware requirements must be referenced in
                    // 40 Hardware Requirements or linked to 40 Hardware Requirements.
                    if (w.Hardware)
                    {
                        // Hardware requirement
                        InList = false;
                        // Check reference to 40 HW
                        foreach (WorkitemReference wr in wrl)
                        {
                            if (wr.DocumentName.Substring(0, 2) == "40" &&
                               (wr.DocumentName.Contains("HW")))
                            {
                                cv.LinkedAdditionalHardwareRequirements.Add(w);
                                InList = true;
                                break;
                            }
                        }
                        if (!InList)
                        {
                            // Check Link to 40 HW
                            LinkedWorkitem = w.CheckLinkTo(myP, "40", "HW", "relates_to", "requirement");
                            if (LinkedWorkitem != null)
                            {
                                if (LinkedWorkitem.Hardware)
                                {
                                    cv.LinkedAdditionalHardwareRequirements.Add(w);
                                }
                                else
                                {
                                    cv.LinkedAdditionalRequirementsWithInvalidAllocation40HW.Add(w);
                                }
                                InList = true;
                            }
                        }
                        if (!InList)
                        {
                            // no reference or link found
                            cv.UnlinkedAdditionalHardwareRequirements.Add(w);
                        }
                    }
                    #endregion

                    #region 4.1.6 Linkage to 50 HW Architectural Design (if applicable)
                    // As mentioned in Rules for multiple documents, usually, the Hardware Architectural Design is
                    // included in 30 Element Architecture. Alternatively, separate 50 HW Architectural Design
                    // documents may be used for better clarity (equivalent to the Software Architectural Design
                    // document(s)).
                    // For every Component or Interface (element) in the Element Architecture additional (HW, SW or
                    // Mech) requirements may be defined. Every additional hardware requirement defined in 30
                    // Element Architecture (in this case, 30 Element Architecture only contains the context diagram)
                    // must be related to a Component or Interface of the Hardware Architectural Design (in this case,
                    // 50 HW Architectural Design document).
                    
                    if (myP.DocumentExits("50","HW"))
                    {
                        // 50 HW exists in this Project -> perform checks
                        if (w.Hardware)
                        {
                            if (w.Interface)
                            {
                                // Check Link to 50 SW
                                LinkedWorkitem = w.CheckLinkTo(myP, "50", "HW", "relates_to", "interface");
                            }
                            else
                            {
                                // Check Link to 50 SW
                                LinkedWorkitem = w.CheckLinkTo(myP, "50", "HW", "relates_to", "component");
                            }
                            if (LinkedWorkitem != null)
                            {
                                if (w.Interface)
                                {
                                    if (LinkedWorkitem.Type == "interface")
                                    {
                                        cv.LinkedAdditionalHardwareRequirements50HW.Add(w);
                                    }
                                    else
                                    {
                                        w.TextError50SW = "Interface linked to Component";
                                        cv.IncorrectlyLinkedHardwareRequirements.Add(w);
                                    }
                                }
                                else
                                {
                                    // no interface
                                    if (LinkedWorkitem.Type == "component")
                                    {
                                        cv.LinkedAdditionalHardwareRequirements50HW.Add(w);
                                    }
                                    else
                                    {
                                        w.TextError50SW = "non-Interface linked to Interface";
                                        cv.IncorrectlyLinkedHardwareRequirements.Add(w);
                                    }
                                }
                            }
                            else
                            {
                                cv.UnlinkedAdditionalHardwareRequirements.Add(w);
                            }
                        }
                        else
                        {
                            // no Hardware -> no link to 50 HW allowed
                            if (w.CheckLinkExists(myP, "50", "HW", "relates_to"))
                            {
                                cv.LinkedAdditionalRequirementsWithInvalidAllocation50HW.Add(w);
                            }
                        }
                    }

                    #endregion

                    #region 4.2.1 Requirements Status
                    // The Report lists requirements with suspect Status and requirements that should be checked as to their Status.
                    // Workitems with TBD and Status <> In Clarification
                    if (w.Title.Contains("TBD") && w.Status != "clarificationNeeded")
                    {
                        cv.StatusTBD.Add(w);
                    }

                    // Workitems with Status=Draft
                    if (w.Status == "draft")
                    {
                        cv.StatusDraft.Add(w);
                    }

                    // WorkItems in Review -> eigener Report

                    // WorkItems in Clarification
                    if (w.Status == "clarificationNeeded")
                    {
                        cv.StatusClarificationNeeded.Add(w);
                    }

                    // Deferred Workitems
                    if (w.Status == "deferred")
                    {
                        cv.StatusDeferred.Add(w);
                    }

                    // Workitems with Prioritization = 'Should Have'
                    if (w.Severity == "should_have")
                    {
                        cv.SeverityShouldHave.Add(w);
                    }
                    #endregion

                    #region 4.2.2 Requirement Allocation
                    // Workitems without Allocation
                    if (myP.WorkitemRequirementAllocations.Count(x => x.WorkitemID == w.C_pk) == 0)
                    {
                        cv.AllocMissing.Add(w);
                    }
                    else
                    {
                        // Workitems with invalid Allocation Requirement Allocation must be "HW | SW | Mech
                        // nur prüfen wenn Allocation vorhanden!
                        if (!w.IsHSM())
                        {
                            cv.InvalidAllocation.Add(w);
                        }
                    }

                    // Interfaces and not HW, SW, Mech (Valid Interface Requirement Allocation is "Interface"+"HW |"SW | "Mech)
                    if (w.Interface && !w.IsHSM())
                    {
                        cv.AllocInterface.Add(w);
                    }

                    // ESE and not HW, SW, Mech ("ESE" requirements must be allocated to HW, SW and/or Mechanics)
                    if (w.ESE && !w.IsHSM())
                    {
                        cv.AllocESE.Add(w);
                    }
                    #endregion

                    #region 5.2.3 Functional Requirement
                    // "Functional requirement" checked for != HW, SW, Mech oder
                    // "Functional requirement" und Allocation Project
                    if ((w.FunctionalRequirement && !w.IsHSM()) || (w.FunctionalRequirement && w.Project))
                    {
                        if (!cv.AllocMissing.Contains(w))
                        {
                            // Wenn das Workitem nicht in der Liste AllocMissing ist
                            cv.Document.FuncReqHSM.Add(w);
                        }
                    }
                    #endregion

                    #region 4.2.3 Special reports

                    // Workitems with ESE and SW (lists all software requirements relevant to the Software Architectural Design)
                    if (w.ESE && w.Software)
                    {
                        cv.SpecialReportESESW.Add(w);
                        if (Test1.Length > 2) Test1 += " || ";
                        Test1 += "id=" + w.Id;
                    }

                    

                    // Workitems with ESE and HW (lists all hardware requirements relevant to the Hardware Architectural Design)
                    if (w.ESE && w.Hardware)
                    {
                        cv.SpecialReportESEHW.Add(w);
                    }
                    #endregion

                    // Review Report:
                    w.FillComments(myP.Comments, myP.Users); // Approval Comments zu Workitem verknüpfen
                    w.FillApprovals(myP.WorkitemApprovals, myP.Users);
                    cv.Document.Review.CheckWorkitem(w);
                }

                // Components without linked requirements (Additional requirements may be required)
                if (w.Type == "component")
                {
                    if (w.Id == "RT-2641")
                    {
                        Debug.WriteLine(w.Id);
                    }
                    // Check if additional requirements exits
                    bool RequirementFound = false;

                    if (w.FillCheckDownlink(myP.Uplinks) == 0)
                    {
                        // keine Downlinks vorhanden -> Uplinks mit Requirements im gleichen Dokument checken:
                        Tempworkitem = w.CheckLinkFrom(myP, "30", "Element Architecture", "relates_to", "requirement");
                        if (Tempworkitem != null)
                        {
                            RequirementFound = true;
                        }
                        else
                        {
                            cv.ComponentsWithoutLinkedRequirements.Add(w);
                        }
                    }
                    else
                    {
                        foreach (Uplink d in w.Downlinks)
                        {
                            if (d.Role == "parent" || d.Role == "relates_to")
                            {
                                Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                                if (Tempworkitem.Type == "requirement")
                                {
                                    RequirementFound = true;
                                    break;
                                }
                            }
                        }
                        if (!RequirementFound)
                        {
                            cv.ComponentsWithoutLinkedRequirements.Add(w);
                        }
                    }
                }

                // Interfaces without linked requirements (Additional requirements may be required)
                if (w.Type == "interface")
                {
                    bool RequirementFound = false;
                    // Check if additional requirements exits
                    if (w.Id == "RT-2641")
                    {
                        Debug.WriteLine(w.Id);
                    }
                    if (w.FillCheckDownlink(myP.Uplinks) == 0)
                    {
                        // keine Downlinks vorhanden -> Uplinks mit Requirements im gleichen Dokument checken:
                        Tempworkitem = w.CheckLinkFrom(myP, "30", "Element Architecture", "relates_to", "requirement");
                        if (Tempworkitem != null)
                        {
                            RequirementFound = true;
                        }
                        else
                        {
                            cv.InterfacesWithoutLinkedRequirements.Add(w);
                        }
                    }
                    else
                    {
                        foreach (Uplink d in w.Downlinks)
                        {
                            if (d.Role == "parent" || d.Role == "relates_to")
                            {
                                Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                                if (Tempworkitem.Type == "requirement")
                                {
                                    RequirementFound = true;
                                    break;
                                }
                            }
                        }
                        if (!RequirementFound)
                        {
                            cv.InterfacesWithoutLinkedRequirements.Add(w);
                        }
                    }
                }
            }
            // cv.PolarionTableLink += System.Uri.EscapeDataString(Test1) + "&tab=table";
            // cv.PolarionTableLink = cv.PolarionTableLink.Replace("%3D", "%3A");
            return View(cv);
        }
    }
}