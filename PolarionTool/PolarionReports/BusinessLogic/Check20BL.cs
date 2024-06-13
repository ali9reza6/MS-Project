using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic
{
    public class Check20BL
    {
        #region void BreakDownAnalyse(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Breakdown Analyse
        /// </summary>
        /// <param name="p">Project</param>
        /// <param name="d">Document</param>
        /// <param name="vm">ViewModel</param>
        /// <param name="w">perform checks with this Workitem</param>
        public void BreakDownAnalyse(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            bool InErrorList = false;

            if (w.Id == "RT-2784")
            {
                Debug.WriteLine(w.Id);
            }

            /* 2019-07-22 Breakdown wird derzeit nicht verwendet Listen ausgeblendet
            if (IsHighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW(p, vm, w))
            {
                d.HighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW.Add(w);
                InErrorList = true;
            }
            

            if (IsHighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements(p, vm, w))
            {
                d.HighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements.Add(w);
                InErrorList = true;
            }
            */

            /* 2018-10-11 Diese 2 Listen werden derzeit nicht angezeigt
            if (IsHighLevelHWRequirementsToBeSpecifiedInSWRequirements(p, vm, w))
            {
                d.HighLevelHWRequirementsToBeSpecifiedInSWRequirements.Add(w);
                InErrorList = true;
            }

            if (IsHighLevelSWRequirementsToBeSpecifiedInHWRequirements(p, vm, w))
            {
                d.HighLevelSWRequirementsToBeSpecifiedInHWRequirements.Add(w);
                InErrorList = true;
            }
            */

            /*
            if (IsHighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation(p, vm, w))
            {
                d.HighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation.Add(w);
                InErrorList = true;
            }
            */

            /*
            if (IsHighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation(p, vm, w))
            {
                d.HighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation.Add(w);
                InErrorList = true;
            }
            */

            /*
            if (IsHighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation(p, vm, w))
            {
                d.HighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation.Add(w);
                InErrorList = true;
            }
            */

            /*
            if (IsHighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements(p, vm, w))
            {
                d.HighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements.Add(w); // orange keine Fehlerliste
            }
            */

            if (IsHighLevelRequirementsWithoutAnyBreakDownRequirements(p,vm,w))
            {
                d.HighLevelRequirementsWithoutAnyBreakDownRequirements.Add(w);
            }


            if (!InErrorList)
            {
                /*
                if (IsBreakDown30InterfaceRequirements(p, vm, w))
                {
                    d.BreakDown30InterfaceRequirements.Add(w);
                }
                */

                /*
                if (IsBreakDown40SWRequirements(p, vm, w))
                {
                    d.BreakDown40SWRequirements.Add(w);
                }
                */

                /*
                if (IsBreakDown40HWRequirements(p, vm, w))
                {
                    d.BreakDown40HWRequirements.Add(w);
                }
                */

                /*
                if (IsHighLevelElementRequirementsWithBreakDownRequirementsByCustomer(p, vm, w))
                {
                    d.HighLevelElementRequirementsWithBreakDownRequirementsByCustomer.Add(w);
                }
                */

                /*
                if (IsHighLevelElementRequirementsWithBreakDownRequirementsByZKW(p, vm, w))
                {
                    d.HighLevelElementRequirementsWithBreakDownRequirementsByZKW.Add(w);
                }
                */
            }
        }
        #endregion

        #region void LinkageTo50SW(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Check Linkage to 50 Software
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        public void LinkageTo50SW(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            WorkitemLinkError wle;
            WorkitemLinks wl;

            if (IsUnlinkedSoftwareElementRequirements(p, d, vm, w))
            {
                d.UnlinkedSoftwareElementRequirements.Add(w);
            }
            else if (IsIncorrectlyLinkedSoftwareElementRequirements(p, d, vm, w, out wle))
            {
                
                d.IncorrectlyLinkedSoftwareElementRequirements.Add(wle);
            }

            if (IsLinkedSoftwareElementRequirementsWithInvalidAllocation(p, d, vm, w, out wl))
            {
                d.LinkedSoftwareElementRequirementsWithInvalidAllocation.Add(wl);
            }

            if (IsLinkedSoftwareElementRequirements(p, d, vm, w, out wl))
            {
                d.LinkedSoftwareElementRequirements.Add(wl);
            }
        }
        #endregion

        #region 2018-12-03 obsolet:  void LinkageTo50HW(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Check Linkage to 50 Hardware
        /// 2018-12-03 obsolet:
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        public void LinkageTo50HW(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            WorkitemLinkError wle;
            // Check ob ein 50 HW Document bereits existiert:
            if (p.DocumentExits("50", "Hardware Architectural"))
            {
                // 50 Hardware Architectual Design Document exists => check linkage

                if (IsUnlinkedHardwareElementRequirements(p, d, vm, w))
                {
                    d.UnlinkedHardwareElementRequirements.Add(w);
                }
                else if (IsIncorrectlyLinkedHardwareElementRequirements(p, d, vm, w, out wle))
                {
                    d.IncorrectlyLinkedHardwareElementRequirements.Add(wle);
                }

                if (IsLinkedHardwareElementRequirementsWithInvalidAllocation(p, d, vm, w, out wle))
                {
                    d.LinkedHardwareElementRequirementsWithInvalidAllocation.Add(wle);
                }

                if (IsLinkedHardwareElementRequirements(p, d, vm, w, out wle))
                {
                    d.LinkedHardwareElementRequirements.Add(wle);
                }

                // CleanUp Linkage to 30 Elemet Architecture:
                // Wenn in 50 OK Liste dann aus 30 Fehlerliste entfernen
                // 
                //d.Missing30Links
                //d.LinkedHardwareElementRequirements
                
                foreach(WorkitemLinkError wle1 in d.LinkedHardwareElementRequirements)
                {
                    if (d.Missing30Links.Contains(wle1.Workitem))
                    {
                        d.Missing30Links.Remove(wle1.Workitem);
                    }
                }

                // CleanUp Linkage to 50 HW Architecture:
                // Wenn in 30 OK Liste dann aus 50 Fehlerliste entfernen
                // 
                //d.LinkedElementRequirements
                //d.UnlinkedHardwareElementRequirements
                foreach(WorkitemLinks wle1 in d.LinkedElementRequirements)
                {
                    if (d.UnlinkedHardwareElementRequirements.Contains(wle1.Workitem))
                    {
                        d.UnlinkedHardwareElementRequirements.Remove(wle1.Workitem);
                    }
                }
            }

        }
        #endregion

        #region void LinkageToHWChecklist(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Logik: Requirements mit Allocation “HW+ESE” (wie bisherige Liste)
        /// not linked to Checkliste(Folder “Verification”, Filename: “30 Review Architectural Design, 
        /// mit oder ohne Prefix, der Link geht vom Requirement zu einem Testcase in der Checklist)
        /// Tooltip: „Architecturally significant HW Element Requirements not linked to Checklist”
        /// Required actions: 
        ///    1) Include corresponding test cases in 30 Review Architectural Design(folder: Verification), if required
        ///    2) Link requirements to corresponding test case in 30 Review Architectural Design”
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        public void LinkageToHWChecklist(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            Workitem LinkedWorkitem;
            Workitem WorkitemError;
            WorkitemLinkError LinkedWorkitemElement;
            string Documentname;

            if (w.Id == "E18008-3749")
            {
                Debug.WriteLine(w.Id);
            }

            if (w.ESE && w.Hardware)
            {
                LinkedWorkitem = w.CheckLinkFromDoc(p, "30", "Review Architectural", "relates_to", "testcase", out WorkitemError);
                if (LinkedWorkitem != null)
                {
                    LinkedWorkitemElement = new WorkitemLinkError();
                    LinkedWorkitemElement.Workitem = w;
                    LinkedWorkitemElement.LinkedWorkitem = LinkedWorkitem;
                    LinkedWorkitemElement.IdLinkedWorkitem = LinkedWorkitem.Id;
                    LinkedWorkitemElement.ErrorMsg = "linked to Checklist";
                    Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                    LinkedWorkitemElement.PolarionLink = p.GetPolarionLink("Verification", Documentname, LinkedWorkitem.Id);
                    d.HwEseRequirementsLinkedToChecklist.Add(LinkedWorkitemElement);
                    return;
                }
                else
                {
                    d.HwEseRequirementsNotLinkedToChecklist.Add(w);
                }
                return;
            }
        }
        #endregion

        #region bool IsLinkedElementRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// überprüft ob das Workitem unerlaubte Links ins 30 Element Architecture Document hat
        /// </summary>
        /// <param name="p">Project mit allen Daten aus der Datenbank</param>
        /// <param name="d">Aktuelles Document</param>
        /// <param name="vm">Viewmodel für Check20</param>
        /// <param name="w">zu überprüfendes Workitem</param>
        /// <returns>true ... Fehler, false kein Link ins Element Architectue</returns>
        public bool IsLinkedElementRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w, out Workitem LinkedWorkitem)
        {
            List<Workitem> wl = new List<Workitem>();
            List<Uplink> TempUplinks = new List<Uplink>();
            Workitem Tempworkitem;
            string Documentname;

            if (p.WorkitemRequirementAllocations.Count(x => x.WorkitemID == w.C_pk) == 0)
            {
                // Workitems ohne Allocation sollen in die Liste without Allocation
                LinkedWorkitem = null;
                return false;
            }

            if (!w.IsHSM() || (w.IsHSM() && w.Project))
            {
                TempUplinks = p.Uplinks.FindAll(x => x.UplinkId == w.C_pk);
                foreach (Uplink u in TempUplinks)
                {
                    Tempworkitem = p.Workitems.Find(x => x.C_pk == u.WorkitemId);
                    if (Tempworkitem == null)
                    {
                        // Uplink auf workitem "heading"
                        continue;
                    }
                    Documentname = p.GetDocumentname(Tempworkitem.DocumentId);
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
                        // Link from 30 Element Architecture gefunden -> sollte nicht sein
                        LinkedWorkitem = Tempworkitem;
                        return true;
                    }
                }
            }
            LinkedWorkitem = null;
            return false;
        }
        #endregion

        #region bool IsHighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down = 20" (to be specified by Customer) but with Child Requirements by ZKW.
        /// Required action: Set "Break down" to " " (to be specified by ZKW) or resolve conflict otherwise
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW(Project p, Check20ViewModel vm, Workitem w)
        {
            Workitem LinkedWorkitem;
            List<Workitem> wl;

            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if ((!w.LowLevel) && w.BreakDownIn == Workitem.BreakDownType.BD20)
                {
                    // Get Child Workitems
                    wl = w.GetAllChilds(p, w);
                    // Check Uplink to 10x Additional Requirements
                    foreach(Workitem child in wl)
                    {
                        LinkedWorkitem = child.CheckLinkFrom(p, "10", "Additional", "relates_to","customerrequirement");
                        if (LinkedWorkitem != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down = 20" (to be specified Customer) without any Child Requirements.
        /// Required action: Set "Break down" to " " (to be specified by ZKW) or indent existing child requirements, if applicable
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            List<Workitem> Childs;
            Workitem Tempworkitem;


            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if ((!w.LowLevel) && w.BreakDownIn == Workitem.BreakDownType.BD20)
                {
                    // Get Child Workitems
                    Childs = w.GetAllChilds(p, w);
                    // Check Uplink to 10x Additional Requirements
                    foreach (Workitem child in Childs)
                    {
                        Tempworkitem = child.CheckLinkFrom(p, "10", "Additional", "relates_to", "customerrequirement");
                        if (Tempworkitem != null)
                        {
                            // Requirement child gefunden
                            return false; // Workitem OK
                        }
                    }
                    return true; // kein Downlink gefunden
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelHWRequirementsToBeSpecifiedInSWRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Hardware Element Requirements with "Break down = 40 SW" (to be specified in 40 SW Requirements).
        /// Required action: Set "Break down" to "40 HW " (to be specified in 40 HW Requirements) or modify Allocation
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelHWRequirementsToBeSpecifiedInSWRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((w.Hardware || (w.Hardware && w.Interface)) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40SW )
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelSWRequirementsToBeSpecifiedInHWRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Software Element Requirements with "Break down = 40 HW" (to be specified in 40 HW Requirements).
        /// Required action: Set "Break down" to "40 SW " (to be specified in 40 SW Requirements)  or modify Allocation
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelSWRequirementsToBeSpecifiedInHWRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((w.Software || (w.Software && w.Interface)) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40HW)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level HW/SW or Mech Element Requirements with "Break down = 30I" (to be specified in the Interface Requirements) with invalid Allocation.
        /// Required action: modify Allocation or redefine "Break down"
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((!w.IsHSM() && !w.Interface) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD30I)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level SW Element Requirements with "Break down = 40 SW" (to be specified in the Software Requirements) with invalid Allocation.
        /// Required action: modify Allocation or redefine "Break down"
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((!w.Software || (!w.Software && w.Interface)) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40SW)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level HW Element Requirements with "Break down = 40 HW" (to be specified in the Hardware Requirements) with invalid Allocation.
        /// Required action: modify Allocation or redefine "Break down"
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((!w.Hardware || (w.Hardware && w.Interface)) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40HW)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down" = "" (to be specified by ZKW) without any break down requirements by technical engineers.
        /// Required action: Specify break down requirements and link to 10 Additional Requirements
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            List<Workitem> Childs;
            Workitem Tempworkitem;

            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if (w.FunctionalRequirement && !w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.Empty)
                {
                    // Get Child Workitems
                    Childs = w.GetAllChilds(p, w);
                    // Check Uplink to 10x Additional Requirements
                    foreach (Workitem child in Childs)
                    {
                        Tempworkitem = child.CheckLinkFrom(p, "10", "Additional", "relates_to", "customerrequirement");
                        if (Tempworkitem != null)
                        {
                            return false; 
                        }
                    }
                    return true; // kein Downlink gefunden
                }
            }
            return false;
        }
        #endregion

        #region bool IsBreakDown30InterfaceRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements further specified in 30 Interface Requirements
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsBreakDown30InterfaceRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD30I)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsBreakDown40SWRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements further specified in 40 SW Requirements
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsBreakDown40SWRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            if (w.Software || (w.Software && w.Interface && !w.Project))
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40SW)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsBreakDown40HWRequirements(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements further specified in 40 HW Requirements
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsBreakDown40HWRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            if (w.Hardware || (w.Hardware && w.Interface && !w.Project))
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD40HW)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelElementRequirementsWithBreakDownRequirementsByCustomer(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down = 20" and break down requirements correctly specified by Customer.
        /// Required action: Verify that detailed specification is complete
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelElementRequirementsWithBreakDownRequirementsByCustomer(Project p, Check20ViewModel vm, Workitem w)
        {
            List<Workitem> Childs;
            Workitem Tempworkitem;

            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.BD20)
                {
                    // Get Child Workitems
                    Childs = w.GetAllChilds(p, w);
 
                    // Check Uplink to 10x Additional Requirements
                    foreach (Workitem child in Childs)
                    {
                        Tempworkitem = w.CheckLinkFrom(p, "10", "Additional", "relates_to", "customerrequirement");
                        if (Tempworkitem != null)
                        {
                            return false; // müsste eigentlich schon in Fehlerliste sein
                        }
                    }
                    return true; // keine Downlinks in "Additional"
                }
            }
            return false;
        }
        #endregion

        #region bool IsHighLevelElementRequirementsWithBreakDownRequirementsByZKW(Project p, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// High-level Element Requirements with "Break down = " "" and break down requirements correctly specified ZKW.
        /// Required action: Verify that detailed specification is complete
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsHighLevelElementRequirementsWithBreakDownRequirementsByZKW(Project p, Check20ViewModel vm, Workitem w)
        {
            List<Workitem> Childs;
            Workitem Tempworkitem;

            if ((w.IsHSM() || w.Interface) && !w.Project)
            {
                if (!w.LowLevel && w.BreakDownIn == Workitem.BreakDownType.Empty)
                {
                    // Get Child Workitems
                    Childs = w.GetAllChilds(p, w);

                    // Check Uplink to 10x Additional Requirements
                    foreach (Workitem child in Childs)
                    {
                        Tempworkitem = child.CheckLinkFrom(p, "10", "Additional", "relates_to", "customerrequirement");
                        if (Tempworkitem != null)
                        {
                            return true; // Link ins 10 gefunden   
                        }
                    }
                    return false; // nur Downlinks in != "Additional"
                }
            }
            return false;
        }
        #endregion

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
        private bool IsHighLevelRequirementsWithoutAnyBreakDownRequirements(Project p, Check20ViewModel vm, Workitem w)
        {
            List<Workitem> Childs;

            if (w.Id == "RT-2898")
            {
                Debug.WriteLine(w.Id);
            }

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



        #region bool IsUnlinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Unlinked Software Element Requirements
        /// Software Element Requirements not linked to SW Architectural Design. 
        /// Required action: link SW requirements to components/interfaces in the SW Architectural Design
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsUnlinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            Workitem Tempworkitem;
            string documentname;

            if (w.Software && !w.Project)
            {

                if (w.Downlinks == null)
                {
                    w.FillCheckDownlink(p.Uplinks);
                }
                foreach (Uplink u in w.Downlinks)
                {
                    Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                    if (Tempworkitem != null)
                    {
                        documentname = p.GetDocumentname(Tempworkitem.DocumentId);

                        if (documentname != null && documentname.Length > 2)
                        {
                            // auch auf korrekten Subindex prüfen 
                            DocumentName FromDocument = new DocumentName(documentname);
                            if (documentname.Substring(0, 2) == "50" && documentname.Contains("Software Architectural Design"))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Fehler: ");
                    }
                }
                return true; // kein link gefunden
            }
            return false;
        }
        #endregion

        #region bool IsIncorrectlyLinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Incorrectly linked Software Element Requirements
        /// Interface SW Element Requirements linked to Components or non-Interface SW Element Requirements linked to Interfaces. 
        /// Required action: "correct invalid links"  
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsIncorrectlyLinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinkError wle)
        {
            Workitem LinkedWorkitem;
            Workitem WorkitemError;
            string Documentname;
            DocumentName FromDoc;
            WorkitemLinkError workitemLinkError = new WorkitemLinkError();
            //if (w.Id == "E18010-5735")
            //{
            //    Debug.WriteLine(w.Id);
            //}

            if (w.Software && !w.Project)
            {
                if (w.Interface)
                {
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Software", "relates_to", "interface", out WorkitemError);
                    if (LinkedWorkitem == null)
                    {
                        w.TextError50SW = "not linked to INT";
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = WorkitemError;
                        workitemLinkError.ErrorMsg = "not linked to INT";
                        workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                        Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                        //workitemLinkError.LinkedWorkitem = LinkedWorkitem
                        wle = workitemLinkError;
                        return true;
                    }
                    else
                    {
                        // Check Präfix in Linked Workitem
                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        if (Documentname.Length < 2)
                        {
                            FromDoc = new DocumentName(Documentname);
                            if (d.DocName.Prefix.Length > 0)
                            {
                                if (d.DocName.Prefix == FromDoc.Prefix)
                                {
                                    wle = workitemLinkError;
                                    return false;
                                }
                                else
                                {
                                    w.TextError50SW = FromDoc.Level + FromDoc.Prefix + ":" + LinkedWorkitem.Id;
                                    workitemLinkError.Workitem = w;
                                    workitemLinkError.LinkedWorkitem = WorkitemError;
                                    workitemLinkError.ErrorMsg = FromDoc.Level + FromDoc.Prefix + ":" + LinkedWorkitem.Id;
                                    workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                                    Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                                    workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                                    wle = workitemLinkError;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Software", "relates_to", "component", out WorkitemError);
                    if (LinkedWorkitem == null)
                    {
                        w.TextError50SW = "not linked to COMP";
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = WorkitemError;
                        workitemLinkError.ErrorMsg = "not linked to COMP";
                        workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                        Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                        wle = workitemLinkError;
                        return true;
                    }
                    else
                    {
                        // Check Präfix in Linked Workitem
                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        if (Documentname.Length > 2)
                        {
                            FromDoc = new DocumentName(Documentname);
                            if (FromDoc.Prefix == null)
                            {
                                // 50 Software hat keinen Präfix => kein Präfix check => Link gefunden OK
                                wle = workitemLinkError;
                                return false;
                            }
                            else
                            {
                                if (d.DocName.Prefix.Length > 0)
                                {
                                    if (d.DocName.Prefix == FromDoc.Prefix)
                                    {
                                        wle = workitemLinkError;
                                        return false;
                                    }
                                    else
                                    {
                                        w.TextError50SW = FromDoc.Level + FromDoc.Prefix + ": " + LinkedWorkitem.Id;
                                        workitemLinkError.Workitem = w;
                                        workitemLinkError.LinkedWorkitem = WorkitemError;
                                        workitemLinkError.ErrorMsg = FromDoc.Level + FromDoc.Prefix + ":" + LinkedWorkitem.Id;
                                        workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                                        Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                                        wle = workitemLinkError;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            wle = workitemLinkError;
            return false;
        }
        #endregion

        #region bool IsLinkedSoftwareElementRequirementsWithInvalidAllocation(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinkError wle)
        /// <summary>
        /// Linked Software Element Requirements with invalid Allocation
        /// Requirements linked with 50 Software Architectural Design with Allocation other than "SW" 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsLinkedSoftwareElementRequirementsWithInvalidAllocation(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinks wl)
        {
            string Documentname;
            WorkitemLinkError workitemLinkError = new WorkitemLinkError();
            Workitem Tempworkitem;
            WorkitemLinks workitemLinks = new WorkitemLinks();

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(p.Uplinks);
            }
            foreach (Uplink u in w.Downlinks)
            {
                Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem != null)
                {
                    Documentname = p.GetDocumentname(Tempworkitem.DocumentId);
                    if (Documentname != null && Documentname.Length > 2)
                    {
                        if (Documentname.Substring(0, 2) == "50" && Documentname.Contains("Software Architectural Design"))
                        {
                            if (!w.Software || (w.Software && w.Project))
                            {
                                workitemLinks.Workitem = w;
                                Documentname = p.GetDocumentname(Tempworkitem.DocumentId);
                                WorkitemViewLink workitemViewLink = new WorkitemViewLink();
                                workitemViewLink.Id = Tempworkitem.Id;
                                workitemViewLink.Title = Tempworkitem.Title;
                                workitemViewLink.Type = Tempworkitem.Type;
                                workitemViewLink.LinkDisplay = Documentname.Substring(0, 3) + ":" + Tempworkitem.Type.Substring(0, 1).ToUpper() + ":" + Tempworkitem.Id;
                                workitemViewLink.PolarionDocumentLink = p.GetPolarionLink(Documentname, Tempworkitem.Id);
                                workitemLinks.LinkedWorkitems.Add(workitemViewLink);
                                wl = workitemLinks;
                                return true;
                            }
                            wl = workitemLinks;
                            return false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Fehler: " + Tempworkitem.Id);
                    }
                }
            }
            wl = workitemLinks;
            return false;
        }
        #endregion

        #region bool IsLinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Software Element Requirements correctly linked to the Software Architecture
        /// 2018-12-05 Umgestellt auf WorkitemLinks
        /// </summary>
        /// <param name="p">Project</param>
        /// <param name="d">Document</param>
        /// <param name="vm">ViewModel</param>
        /// <param name="w">Workitem</param>
        /// <returns></returns>
        private bool IsLinkedSoftwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinks wl)
        {
            string Documentname;
            Workitem LinkedWorkitem;
            Workitem WorkitemError;
            WorkitemLinks workitemLinks = new WorkitemLinks();

            if (w.Software && !w.Project)
            {
                if (w.Interface)
                {
                    // Interface mit Interface
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50" + d.DocName.Prefix, "Software", "relates_to", "interface", out WorkitemError);
                    if (LinkedWorkitem != null)
                    {
                        workitemLinks.Workitem = w;

                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        WorkitemViewLink workitemViewLink = new WorkitemViewLink();
                        workitemViewLink.Id = LinkedWorkitem.Id;
                        workitemViewLink.Title = LinkedWorkitem.Title;
                        workitemViewLink.Type = LinkedWorkitem.Type;
                        workitemViewLink.LinkDisplay = Documentname.Substring(0, 3) + ":" + LinkedWorkitem.Type.Substring(0, 1).ToUpper() + ":" + LinkedWorkitem.Id;
                        workitemViewLink.PolarionDocumentLink = p.GetPolarionLink(Documentname, LinkedWorkitem.Id);

                        workitemLinks.LinkedWorkitems.Add(workitemViewLink);
                        wl = workitemLinks;
                        return true;
                    }
                }
                else
                {
                    // Software mit Component
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50" + d.DocName.Prefix, "Software", "relates_to", "component", out WorkitemError);
                    if (LinkedWorkitem != null)
                    {
                        workitemLinks.Workitem = w;

                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        WorkitemViewLink workitemViewLink = new WorkitemViewLink();
                        workitemViewLink.Id = LinkedWorkitem.Id;
                        workitemViewLink.Title = LinkedWorkitem.Title;
                        workitemViewLink.Type = LinkedWorkitem.Type;
                        workitemViewLink.LinkDisplay = Documentname.Substring(0, 3) + ":" + LinkedWorkitem.Type.Substring(0, 1).ToUpper() + ":" + LinkedWorkitem.Id;
                        workitemViewLink.PolarionDocumentLink = p.GetPolarionLink(Documentname, LinkedWorkitem.Id);

                        workitemLinks.LinkedWorkitems.Add(workitemViewLink);
                        wl = workitemLinks;
                        return true;
                    }
                }
            }
            wl = workitemLinks;
            return false;
        }
        #endregion

        #region bool IsUnlinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Unlinked Hardware Element Requirements
        /// Hardware Element Requirements not linked to HW Architectural Design. 
        /// Required action: link HW requirements to components/interfaces in the HW Architectural Design
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsUnlinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        {
            Workitem Tempworkitem;
            string documentname;

            if (w.Hardware && !w.Project)
            {

                if (w.Downlinks == null)
                {
                    w.FillCheckDownlink(p.Uplinks);
                }
                foreach (Uplink u in w.Downlinks)
                {
                    Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                    if (Tempworkitem != null)
                    {
                        documentname = p.GetDocumentname(Tempworkitem.DocumentId);
                        if (documentname != null && documentname.Length > 2)
                        {
                            if (documentname.Substring(0, 2) == "50" && documentname.Contains("Hardware Architectural Design"))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Fehler: " + Tempworkitem.Id);
                        }
                    }
                }
                return true; // kein link gefunden
            }
            return false;
        }
        #endregion

        #region bool IsIncorrectlyLinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Incorrectly linked Hardware Element Requirements
        /// Interface HW Element Requirements linked to Components or non-Interface HW Element Requirements linked to Interfaces. 
        /// Required action: "correct invalid links"  
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsIncorrectlyLinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinkError wle)
        {
            // Info derzeit kein Prefix Check bei hardware  + d.DocName.Prefix
            string Documentname;
            Workitem LinkedWorkitem;
            Workitem WorkitemError;
            WorkitemLinkError workitemLinkError = new WorkitemLinkError();

            if (w.Hardware && !w.Project)
            {
                if (w.Interface)
                {
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Hardware", "relates_to", "interface", out WorkitemError);
                    if (LinkedWorkitem == null)
                    {
                        w.TextError50HW = "not linked to INT";
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = WorkitemError;
                        workitemLinkError.ErrorMsg = "not linked to INT";
                        workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                        Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                        wle = workitemLinkError;
                        return true;
                    }
                }
                else
                {
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Hardware", "relates_to", "component", out WorkitemError);
                    if (LinkedWorkitem == null)
                    {
                        w.TextError50HW = "not linked to COMP";
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = WorkitemError;
                        workitemLinkError.ErrorMsg = "not linked to COMP";
                        workitemLinkError.IdLinkedWorkitem = WorkitemError.Id;
                        Documentname = p.GetDocumentname(WorkitemError.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, WorkitemError.Id);
                        wle = workitemLinkError;
                        return true;
                    }
                }
            }
            wle = workitemLinkError;
            return false;
        }
        #endregion

        #region bool IsLinkedHardwareElementRequirementsWithInvalidAllocation(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Linked Hardware Element Requirements with invalid Allocation
        /// Requirements linked with 50 Hardware Architectural Design with Allocation other than "HW"
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsLinkedHardwareElementRequirementsWithInvalidAllocation(Project p, Document d, Check20ViewModel vm, Workitem w, out WorkitemLinkError wle)
        {
            Workitem Tempworkitem;
            string documentname;
            WorkitemLinkError workitemLinkError = new WorkitemLinkError();

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(p.Uplinks);
            }
            foreach (Uplink u in w.Downlinks)
            {
                Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem != null)
                {
                    documentname = p.GetDocumentname(Tempworkitem.DocumentId);
                    if (documentname != null && documentname.Length > 2)
                    {
                        if (documentname.Substring(0, 2) == "50" && documentname.Contains("Hardware Architectural Design"))
                        {
                            if (!w.Hardware || (w.Hardware && w.Project))
                            {
                                w.TextError50HW = "Invalid Alloc";
                                workitemLinkError.Workitem = w;
                                workitemLinkError.LinkedWorkitem = Tempworkitem;
                                workitemLinkError.ErrorMsg = "Invalid Alloc";
                                workitemLinkError.IdLinkedWorkitem = Tempworkitem.Id;
                                workitemLinkError.PolarionLink = p.GetPolarionLink(documentname, Tempworkitem.Id);
                                wle = workitemLinkError;
                                return true;
                            }
                            wle = workitemLinkError;
                            return false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Fehler: " + Tempworkitem.Id);
                    }
                }
            }
            wle = workitemLinkError;
            return false;
        }
        #endregion

        #region bool IsLinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w)
        /// <summary>
        /// Hardware Element Requirements correctly linked to the Hardware Architecture
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="vm"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool IsLinkedHardwareElementRequirements(Project p, Document d, Check20ViewModel vm, Workitem w,out WorkitemLinkError wle)
        {
            string Documentname;
            Workitem LinkedWorkitem;
            Workitem WorkitemError;
            WorkitemLinkError workitemLinkError = new WorkitemLinkError();

            if (w.Hardware && !w.Project)
            {
                if (w.Interface)
                {
                    // Hardware-Interface mit Interface
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Hardware", "relates_to", "interface", out WorkitemError);
                    if (LinkedWorkitem != null)
                    {
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = LinkedWorkitem;
                        workitemLinkError.ErrorMsg = "";
                        workitemLinkError.IdLinkedWorkitem = LinkedWorkitem.Id;
                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, LinkedWorkitem.Id);
                        wle = workitemLinkError;
                        return true;
                    }
                }
                else
                {
                    // Hardware mit Component
                    LinkedWorkitem = w.CheckLinkFromDoc(p, "50", "Hardware", "relates_to", "component", out WorkitemError);
                    if (LinkedWorkitem != null)
                    {
                        workitemLinkError.Workitem = w;
                        workitemLinkError.LinkedWorkitem = LinkedWorkitem;
                        workitemLinkError.ErrorMsg = "";
                        workitemLinkError.IdLinkedWorkitem = LinkedWorkitem.Id;
                        Documentname = p.GetDocumentname(LinkedWorkitem.DocumentId);
                        workitemLinkError.PolarionLink = p.GetPolarionLink(Documentname, LinkedWorkitem.Id);
                        wle = workitemLinkError;
                        return true;
                    }
                }
            }
            wle = workitemLinkError;
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