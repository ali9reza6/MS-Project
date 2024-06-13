using PolarionReports.Custom_Helpers;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic
{
    public class Check30NBL
    {
        public void Check30NDocument(DatareaderP dr, Project myP, Check30NListsViewModel cv)
        {
            bool InList;
            bool InAssociatedAdditionalRequirements = false;
            bool InIncorrectlyAssociatedAdditionalRequirements = false;
            Workitem Tempworkitem;
            Workitem LinkedWorkitem;
            string LinkedDocumentname;
            CheckVerification checkVerification = new CheckVerification();

            cv.Document.PolarionDocumentLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/{cv.Document.C_id}?selection=";

            cv.Document.PolarionTableLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{myP.C_id}/wiki/Specification/"
                        + System.Uri.EscapeDataString(cv.Document.C_id) + "?query=";

            cv.PolarionTableLink = "http://" + Topol.PolarionServer + "/polarion/#/project/" + myP.C_id + "/wiki/Specification/"
                                    + System.Uri.EscapeDataString(cv.Document.C_id) + "?query=";

            cv.Tooltip = new Tooltip30EA();
            cv.TooltipReview = new TooltipReview();

            cv.UnassociatedAdditionalRequirements = new List<Workitem>();
            cv.IncorrectlyAssociatedAdditionalRequirements = new List<Workitem>();
            cv.AssociatedAdditionalRequirements = new List<Workitem>();

            cv.LinkedAdditionalRequirementsWithInvalidAllocation = new List<Workitem>();
            cv.LinkedAdditionalInterfaceRequirements = new List<Workitem>();

            cv.UnreferencedInterfaces = new List<Workitem>();

            // cv.LinkedAdditionalRequirementsWithInvalidAllocation40SW = new List<Workitem>();
            // cv.LinkedAdditionalSoftwareRequirements = new List<Workitem>();

            cv.UnlinkedAdditionalSoftwareRequirements = new List<Workitem>();
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

            cv.InterfacesWithoutConnectedComponents = new List<Workitem>();
            cv.ComponentsWithoutLinkedRequirements = new List<Workitem>();
            cv.InterfacesWithoutLinkedRequirements = new List<Workitem>();
            cv.SpecialReportESESW = new List<Workitem>();
            cv.SpecialReportESEHW = new List<Workitem>();
            cv.SafetyRelevant = new List<Workitem>();
            cv.CybersecurityRelevant = new List<Workitem>();

            cv.WorkitemInBin = new List<Workitem>();

            cv.HighLevelRequirementsWithoutAnyBreakDownRequirements = new List<Workitem>();

            cv.Document.UnlinkedElementRequirements = new List<Models.TableRows.RequTestcaseUnlinked>();
            cv.Document.IncorrectlyLinkedElementRequirementsToTestcases = new List<Models.TableRows.RequTestcaseError>();
            cv.Document.LinkedElementRequirementsToTestcases = new List<Models.TableRows.RequTestcase>();
            cv.Document.ElementRequirementsWithMissingVerificationProperties = new List<Models.TableRows.RequTestcase>();

            cv.Document.DocSignatureStatus = new DocSignatureStatusBL();
            cv.Document.DocSignatureStatus.DocSignatureStatusList = dr.GetDocSignatureStatus(cv.Document.C_pk, out string errorGetDocSignatureStatus);
            cv.Document.DocSignatureStatus.MakeView(cv.Document);

            cv.Document.Review = new Review(myP.Users);

            foreach (Workitem w in myP.Workitems.FindAll(x => x.DocumentId == cv.Document.C_pk))
            {
                w.DocPrefix = cv.Document.DocumentLevel + cv.Document.DucumentIndex;
                InAssociatedAdditionalRequirements = false;
                InIncorrectlyAssociatedAdditionalRequirements = false;

                if (w.Id == "RT-2640")
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

                    #region 4.2.2 Requirement Allocation
                    // Workitems without Allocation
                    if (myP.WorkitemRequirementAllocations.Count(x => x.WorkitemID == w.C_pk) == 0)
                    {
                        cv.AllocMissing.Add(w);
                        continue; // bei fehlender Allocierung keine weiteren Checks
                    }
                    else
                    {
                        // Interfaces and not HW, SW, Mech (Valid Interface Requirement Allocation is "Interface"+"HW |"SW | "Mech)
                        if (w.Interface && !w.IsHSM())
                        {
                            w.TextError = "Interface Alloc";
                            cv.InvalidAllocation.Add(w);
                            continue;
                        }

                        if (w.ProjectOnly())
                        {
                            w.TextError = "Invalid Project alloc";
                            cv.InvalidAllocation.Add(w);
                            continue;
                        }

                        // ESE and not HW, SW, Mech ("ESE" requirements must be allocated to HW, SW and/or Mechanics)
                        //if (w.ESE && !w.IsHSM())
                        //{
                        //    w.TextError = "ARCH Alloc";
                        //    cv.InvalidAllocation.Add(w);
                        //    continue;
                        //} alte Version geändert auf gleichen check wie im 20

                        if (w.ESE)
                        {
                            if (!w.Hardware && !w.Software)
                            {
                                //w.TextErrorESE = "not HW,SW";
                                w.TextError = "Incorrect ARCH allocation";
                                cv.InvalidAllocation.Add(w);
                            }
                            else if (w.Project)
                            {
                                //w.TextErrorESE = "Project";
                                w.TextError = "ARCH and Project";
                                cv.InvalidAllocation.Add(w);
                            }
                            else if ((w.Hardware) && !w.SE)
                            {
                                w.TextError = "Incorrect ARCH allocation";
                                cv.InvalidAllocation.Add(w);
                            }
                        }

                        // Workitems with invalid Allocation Requirement Allocation must be "HW | SW | Mech
                        // nur prüfen wenn Allocation vorhanden!
                        if (!w.IsHSM())
                        {
                            w.TextError = "not HW|SW|Mech";
                            cv.InvalidAllocation.Add(w);
                            continue;
                        }
                    }

                    #endregion

                    #region Linkage to 30 Element Architecture
                    InList = false;
                    bool Reference = false;
                    #region debugging
                    //--------------------------
                    // Only for debugging:
                    if (w.Id == "E18008-13898")
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

                                // Das Child Requirement erbt die Verlinkungen des Parents (rekursiv)
                                // ToDo: Funktion die überprüft ob Parent in der Kette richtig verlinkt ist
                                //==========================================================================

                                // Reference => not found => no more checks
                                if (!Reference)
                                {
                                    // Check Type of parent
                                    if (Tempworkitem.Type != "component" &&
                                        Tempworkitem.Type != "interface")
                                    {
                                        if (Tempworkitem.Type == "requirement")
                                        {
                                            Workitem w2 = Tempworkitem.CheckLinkTo(myP, "30", "", "relates_to", "component");
                                            if (w2 == null)
                                            {
                                                // Check Interface:
                                                Workitem w3 = Tempworkitem.CheckLinkTo(myP, "30", "", "relates_to", "interface");
                                                if (w3 == null)
                                                {
                                                    if (!InList)
                                                    {
                                                        w.TextError30Interface = "not associated";
                                                        cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                                        InIncorrectlyAssociatedAdditionalRequirements = true;
                                                        InList = true;
                                                    }
                                                }
                                                else
                                                {
                                                    // interface am obersten Parent gefunden:
                                                    cv.AssociatedAdditionalRequirements.Add(w);
                                                    InAssociatedAdditionalRequirements = true;
                                                    InList = true;
                                                }
                                            }
                                            else
                                            {
                                                // Component am obersten Parent gefunden:
                                                cv.AssociatedAdditionalRequirements.Add(w);
                                                InAssociatedAdditionalRequirements = true;
                                                InList = true;
                                            }
                                        }
                                        else
                                        {
                                            if (!InList)
                                            {
                                                w.TextError30Interface = "not associated";
                                                cv.IncorrectlyAssociatedAdditionalRequirements.Add(w);
                                                InIncorrectlyAssociatedAdditionalRequirements = true;
                                                InList = true;
                                            }
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
                                    else if (!w.Interface && Tempworkitem.Type != "component")
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
                        foreach (Uplink d in w.Downlinks)
                        {
                            if (d.Role == "relates_to" || d.Role == "specifies")
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

                    #region 4.1.3 Linkage to 40 Software Requirements
                    // Im 30N Check nicht mehr erforderlich
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
                        if (w.Id == "RT-2869")
                        {
                            Debug.WriteLine(w.Id);
                        }

                        if (w.Interface)
                        {
                            // Check Link to 50 SW
                            LinkedWorkitem = w.CheckLinkTo(myP, "50", "Software", "relates_to", "interface");
                        }
                        else
                        {
                            // Check Link to 50 SW
                            LinkedWorkitem = w.CheckLinkTo(myP, "50", "Software", "relates_to", "component");
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
                            cv.UnlinkedAdditionalSoftwareRequirements.Add(w);
                        }

                    }
                    else
                    {
                        // no Software -> no link to 50 SW allowed
                        if (w.CheckLinkExists(myP, "50", "SW", "relates_to"))
                        {
                            cv.LinkedAdditionalRequirementsWithInvalidAllocation.Add(w);
                        }
                    }
                    #endregion

                    #region 4.1.5 Linkage to 40 Hardware Requirements
                    // Im 30N Check nicht mehr erforderlich
                    #endregion

                    #region 4.1.6 Linkage to 50 HW Architectural Design (if applicable)
                    // Im 30N Check nicht mehr erforderlich
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
                    }

                    // Workitems with ESE and HW (lists all hardware requirements relevant to the Hardware Architectural Design)
                    if (w.ESE && w.Hardware)
                    {
                        cv.SpecialReportESEHW.Add(w);
                    }

                    // Safety-relevat Workitems
                    if (w.Asil != null)
                    {
                        if (w.Asil.ToUpper() != "QM" && w.Asil != "notrelevant")   // QM || qm || notrelevant
                        {
                            cv.SafetyRelevant.Add(w);
                        }
                    }

                    // Cybersecurity-relevant Workitems
                    if (w.Cs != null)
                    {
                        if (w.Cs == "csRelevant")
                        {
                            cv.CybersecurityRelevant.Add(w);
                        }
                    }
                    #endregion

                    // Breakdown
                    if (IsHighLevelRequirementsWithoutAnyBreakDownRequirements(myP, cv, w))
                    {
                        cv.HighLevelRequirementsWithoutAnyBreakDownRequirements.Add(w);
                    }

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


                    #region Linkage to 30 Interface Requirements
                    // With the New approach for generating Element Requirements, external interfaces are specified
                    // in the Element Requirements, internal interfaces are described in the 30 Interface Requirements documents.
                    // If an Interface is to be further specified in an Interface Requirements Specification, 
                    // it may be referenced.The Report generates a list of unreferenced Interfaces, 
                    // indicating that specification of an Interface may be missing

                    List<WorkitemReference> wrl = myP.WorkitemReferences.FindAll(x => x.WorkitemID == w.C_pk);

                    // Check References in 30 Interface
                    // Das Workitem ist ein Software-Workitem -> alle References zu diesem Element suchen:
                    bool NoRef30Interface = true;
                    foreach (WorkitemReference wr in wrl)
                    {
                        if ((wr.DocumentName.Substring(0, 2) == "20") ||
                           ((wr.DocumentName.Substring(0, 2) == "30") &&
                            (wr.DocumentName.Contains("Interface Requirements"))))
                        {
                            NoRef30Interface = false;
                        }
                    }
                    if (NoRef30Interface)
                    {
                        // Workitem in Fehlerliste -- 2018-12-03 -> Änderung auf Unreferenced Interfaces
                        cv.UnreferencedInterfaces.Add(w);
                    }
                    #endregion


                    bool RequirementFound = false;
                    // Check if additional requirements exits
                    if (w.Id == "E18010-5150")
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

                    // Check Interfaces without connected Components:
                    bool ComponentFound = false;
                    foreach (Uplink d in w.Uplinks)
                    {
                        if (d.Role == "connects")
                        {
                            // Check Type of Workitem:
                            Tempworkitem = myP.Workitems.FirstOrDefault(x => x.C_pk == d.UplinkId);
                            if (Tempworkitem != null)
                            {
                                if (Tempworkitem.Type == "component")
                                {
                                    ComponentFound = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!ComponentFound)
                    {
                        // no Component connect with Interface
                        cv.InterfacesWithoutConnectedComponents.Add(w);
                    }
                }

                // Verification Checks: 2019-11-25 Verification checks nur für Hard- oder Software und nicht Project
                if ((w.Hardware || w.Software) && !w.Project)
                {
                    checkVerification.Check(dr, myP, cv.Document, w);
                }
            }
        }

        #region bool IsHighLevelRequirementsWithoutAnyBreakDownRequirements(Project p, Check30NListsViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down" = "" (to be specified by ZKW) without any break down requirements by technical engineers.
        /// Required action: Specify break down requirements and link to 10 Additional Requirements
        /// 
        /// HighLevelRequirementsWithoutAnyBreakDownRequirements
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelRequirementsWithoutAnyBreakDownRequirements(Project p, Check30NListsViewModel vm, Workitem w)
        {
            List<Workitem> Childs;

            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if (!w.LowLevel)
                {
                    // Get Child Workitems
                    Childs = w.GetAllChilds(p, w);

                    if (Childs.Count == 0)
                    {
                        // keine Childs gefunden -> warning
                        return true;
                    }

                    // Check Check Child with LowLevel 
                    foreach (Workitem child in Childs)
                    {
                        if (child.Type == "requirement")
                        {
                            // Check valid allocation
                            if (CheckRequirementAllocation(p, child))
                            {
                                if (child.LowLevel)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    return true; // keine Downlink mit "Low Level" gefunden
                }
            }
            return false;
        }
        #endregion

        /// <summary>
        /// check requirement the allocation 
        /// </summary>
        /// <param name="myP">Project Class</param>
        /// <param name="w">Workitem to check</param>
        /// <returns>
        /// true ... Allocation valid
        /// false .. Allocation invalid
        /// </returns>
        private bool CheckRequirementAllocation(Project myP, Workitem w)
        {
            if (myP.WorkitemRequirementAllocations.Count(x => x.WorkitemID == w.C_pk) == 0)
            {
                return false; // bei fehlender Allocierung keine weiteren Checks
            }
            else
            {
                // Interfaces and not HW, SW, Mech (Valid Interface Requirement Allocation is "Interface"+"HW |"SW | "Mech)
                if (w.Interface && !w.IsHSM())
                {
                    return false;
                }

                // ESE and not HW, SW, Mech ("ESE" requirements must be allocated to HW, SW and/or Mechanics)
                if (w.ESE && !w.IsHSM())
                {
                    return false;
                }

                // Workitems with invalid Allocation Requirement Allocation must be "HW | SW | Mech
                // nur prüfen wenn Allocation vorhanden!
                if (!w.IsHSM())
                {
                    return false;
                }
            }
            return true;
        }
    }
}