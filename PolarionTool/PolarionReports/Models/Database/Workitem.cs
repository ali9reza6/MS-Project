using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Workitem
    {
        public enum BreakDownType { Empty, BD20, BD30I, BD40SW, BD40HW }

        public int C_pk { get; set; }
        public int DocumentId { get; set; }
        public string Id { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Alloc { get; set; }
        public string TextLF { get; set; }
        public string TextCS { get; set; }      // Text Anzeige Asil und csClassification
        public string TextError { get; set; }   // Text für Fehlerliste
        public string TextError30Interface { get; set; }
        public string TextError50SW { get; set; }
        public string TextError50HW { get; set; }
        public string TextErrorESE { get; set; }
        public string InternalComments { get; set; }
        public string CustomerAction { get; set; }
        public string SupplierAction { get; set; }
        public string ReferredDoc { get; set; }
        public string ClarificationRole { get; set; }
        public string DocPrefix { get; set; }
        public double InitialEstimate { get; set; }     // c_initialestimate
        public double TimeSpent { get; set; }           // c_timespent
        public double RemainingEstimate { get; set; }   // c_remainingestimate
        public string PlannedIn { get; set; }           // für Anzeige von Workpackage ...
        public string DueDate { get; set; }             // c_duedate
        public string Asil { get; set; }                // CF: asil + asilClassification
        public string Cs { get; set; }                  // CyberSecurity CS: csClassification

        /// <summary>
        /// bei Features und Workpackeges wird hier der Feature-Type bzw. Workpackage-Type gespeichert 
        /// </summary>
        public string CFType { get; set; }

        /// <summary>
        /// bei Testcases wird hier das letzte Test-Result gespeichert 
        /// </summary>
        public string TestResult { get; set; }
        public List<TestcaseResult> TestcaseResults { get; set; }

        /// <summary>
        /// Mögliche Inhalte: 20 | 30I | 40 SW | 40 HW - In diesem Document befindet sich die Fein-Spezifikation 
        /// </summary>
        public BreakDownType BreakDownIn { get; set; }

        public bool FunctionalRequirement { get; set; }
        public bool LowLevel { get; set; }
        public bool Hardware { get; set; }
        public bool Software { get; set; }
        public bool Interface { get; set; }
        public bool Mechanic { get; set; }
        public bool ESE { get; set; }
        public bool Test { get; set; }
        public bool Prod { get; set; }
        public bool Env { get; set; }
        public bool Process { get; set; }
        public bool QS { get; set; }
        public bool Project { get; set; }
        public bool Layout { get; set; }
        public bool PM { get; set; }
        public bool SE { get; set; }    // Systems Engineer
        public bool MIT { get; set; }   // 
        public bool AllocEmpty { get; set; }
        public bool InBin { get; set; }
        /// <summary>
        /// Vollständiger Link um das Workitem in der Documentenansicht zu öffnen. 
        /// Wird in Listen benötigt, wo Workitems aus unterschiedlichen Documenten kommen (Impactanalyse)
        /// Wird nur bei Bedarf versorgt.
        /// </summary>
        public string PolarionDocumentLink { get; set; }

        public string InternalComments20
        {
            get
            {
                if (InternalComments != null)
                {
                    if (InternalComments.Length > 20)
                    {
                        return this.InternalComments.Substring(0, 20);
                    }
                    else
                    {
                        return InternalComments;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string CustomerAction20
        {
            get
            {
                if (CustomerAction != null)
                {
                    if (CustomerAction.Length > 20)
                    {
                        return CustomerAction.Substring(0, 20);
                    }
                    else
                    {
                        return CustomerAction;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string SupplierAction20
        {
            get
            {
                if (SupplierAction != null)
                {
                    if (SupplierAction.Length > 20)
                    {
                        return SupplierAction.Substring(0, 20);
                    }
                    else
                    {
                        return SupplierAction;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string ApprovalComment
        {
            get
            {
                string s = "";
                if (Comments.Count > 0)
                {
                    foreach (Comment c in Comments)
                    {
                        s += c.UserName + ": " + c.Text + Environment.NewLine;
                    }
                    return s;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ApprovalComment20
        {
            get
            {
                string s = "";
                if (Comments.Count > 0)
                {
                    foreach (Comment c in Comments)
                    {
                        s += c.Text;
                        if (s.Length > 20)
                        {
                            break;
                        }
                    }
                    if (s.Length > 20)
                    {
                        return s.Substring(0, 20);
                    }
                    else
                    {
                        return s;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Für die Anzeige angepasster Status Text
        /// </summary>
        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "reviewNeeded":
                        return "review";

                    case "clarificationNeeded":
                        return "clarification";

                    default:
                        return Status;
                }
            }
        }

        public string TypeDisplay
        {
            get
            {
                switch (Type)
                {
                    case "requirement":
                        return "Requirements";

                    case "testcase":
                        return "Test cases";

                    case "workPackage":
                        return "Work Packages";

                    case "feature":
                        return "Features";

                    case "interface":
                        return "Interfaces";

                    case "component":
                        return "Components";

                    case "changerequest":
                        return "Changerequest";

                    default:
                        return Type;
                }
                
            }
        }
        public List<Uplink> Uplinks { get; set; }
        public List<Uplink> Downlinks { get; set; }
        public List<Hyperlink> Hyperlinks { get; set; }

        public List<WorkitemApproval> Approvals { get; set; }

        public List<WorkitemAssignee> Assignees { get; set; }

        public List<String> ClarificationRoles { get; set; }

        public List<Comment> Comments { get; set; }

        #region public bool IsHSM()
        /// <summary>
        /// True wenn Workitem Allocation ist Hardware oder Software oder Mechanic
        /// </summary>
        /// <returns></returns>
        public bool IsHSM()
        {
            if (this.Hardware || this.Software || this.Mechanic)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region bool ProjectOnly()
        /// <summary>
        /// Check if the Workitem has only "Project" Allocation
        /// </summary>
        /// <returns>
        /// true ... only Project Allocation
        /// false .. not only Project Allocation
        /// </returns>
        public bool ProjectOnly()
        {   
            if (this.Project)
            {
                if (!this.Hardware &&
                    !this.Software &&
                    !this.Mechanic &&
                    !this.Interface &&
                    !this.ESE &&
                    !this.Test &&
                    !this.Prod &&
                    !this.Env && 
                    !this.Process &&
                    !this.QS &&
                    !this.Layout &&
                    !this.PM &&
                    !this.SE)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        #endregion

        #region public int FillCheckUplink(List<Uplink> ul)
        /// <summary>
        /// Sucht alle Uplinks zu diesen Workitem in der übergebenen Gesamtliste (Link geht von Workitem weg)
        /// Die gefundenen Uplinks werden im Workitem gespeichert
        /// </summary>
        /// <param name="ul">Gesamte Uplink Liste</param>
        /// <returns>Anzahl der gefundenen Uplinks</returns>
        public int FillCheckUplink(List<Uplink> ul)
        {
            int CountParent = 0;
            Uplinks = ul.FindAll(u => u.WorkitemId == C_pk);
            CountParent = Uplinks.Count(u => u.Role == "parent");

            // Uplinks überprüfen:
            if (Uplinks.Count == 0)
            {
                // Wenn keine Uplinks existieren dann
                if (this.DocumentId > 0)
                {
                    InBin = true;
                }
                return 0;
            }

            if (CountParent == 0)
            {
                InBin = true;
            }

            // Überprüfen, ob die Uplinks in das richtige Dokument gehen
            return Uplinks.Count;
        }
        #endregion

        #region public int FillCheckDownlink(List<Uplink> ul)
        /// <summary>
        /// Füllt die Downlink Liste im Workitem
        /// </summary>
        /// <param name="ul"></param>
        /// <returns>Anzahl der Downlinks (is Related to)</returns>
        public int FillCheckDownlink(List<Uplink> ul)
        {
            Downlinks = ul.FindAll(u => u.UplinkId == C_pk);
            return Downlinks.Count;
        }
        #endregion

        #region public void SetRequirementAllocation(string RequirementType)
        /// <summary>
        /// Setzt die Flags entsprechend des RequirementType-Strings:
        /// requirementtype_hardware
        /// requirementtype_software
        /// requirementtype_mechanic
        /// requirementtype_interface
        /// </summary>
        /// <param name="RequirementType"></param>
        public void SetRequirementAllocation(string RequirementType)
        {
            switch (RequirementType)
            {
                case "requirementtype_hardware":
                    this.Hardware = true;
                    MakeAllocText();
                    break;

                case "requirementtype_software":
                    this.Software = true;
                    MakeAllocText();
                    break;

                case "requirementtype_mechanic":
                    this.Mechanic = true;
                    MakeAllocText();
                    break;

                case "requirementtype_interface":
                    this.Interface = true;
                    MakeAllocText();
                    break;

                case "requirementtype_ese":
                    this.ESE = true;
                    MakeAllocText();
                    break;

                case "requirementtype_test":
                    this.Test = true;
                    MakeAllocText();
                    break;

                case "requirementtype_prod":
                    this.Prod = true;
                    MakeAllocText();
                    break;

                case "requirementtype_env":
                    this.Env = true;
                    MakeAllocText();
                    break;

                case "requirementtype_process":
                    this.Process = true;
                    MakeAllocText();
                    break;

                case "requirementtype_qs":
                    this.QS = true;
                    MakeAllocText();
                    break;

                case "requirementtype_project":
                    this.Project = true;
                    MakeAllocText();
                    break;

                case "requirementtype_layout":
                    this.Layout = true;
                    MakeAllocText();
                    break;

                case "requirementtype_pm":
                    this.PM = true;
                    MakeAllocText();
                    break;

                case "requirementtype_se":
                    this.SE = true;
                    MakeAllocText();
                    break;

                case "requirementtype_mit":
                    this.MIT = true;
                    MakeAllocText();
                    break;

                default:
                    break;

            }

        }
        #endregion

        #region public void SetLF(string LF)
        public void SetLF(string LF)
        {
            if (LF == "funcReq")
            {
                this.FunctionalRequirement = true;
            }
            if (LF == "lowLevel")
            {
                this.LowLevel = true;
            }

            this.SetTextLF();
            this.SetAsilCS();
        }
        #endregion

        public void SetBreakdown(string breakdown)
        {
            switch (breakdown)
            {
                case "20_doc":
                    {
                        this.BreakDownIn = BreakDownType.BD20;
                        break;
                    }
                case "30i_doc":
                    {
                        this.BreakDownIn = BreakDownType.BD30I;
                        break;
                    }
                case "40sw_doc":
                    {
                        this.BreakDownIn = BreakDownType.BD40SW;
                        break;
                    }
                case "40hw_doc":
                    {
                        this.BreakDownIn = BreakDownType.BD40HW;
                        break;
                    }
                default:
                    {
                        this.BreakDownIn = BreakDownType.Empty;
                        break;
                    }
            }
        }

        #region public void SetFunctionalRequirement(bool pFunctionalRequirement)
        public void SetFunctionalRequirement(bool pFunctionalRequirement)
        {
            this.FunctionalRequirement = pFunctionalRequirement;
            this.SetTextLF();
        }
        #endregion

        #region private void SetTextLF()
        private void SetTextLF()
        {
            if (this.LowLevel)
            {
                this.TextLF = "L/";
            }
            else
            {
                this.TextLF = "./";
            }
            if (this.FunctionalRequirement)
            {
                this.TextLF += "F";
            }
            else
            {
                this.TextLF += ".";
            }

        }
        #endregion

        /// <summary>
        /// Erstellt die Anzeige für Asil und CyberSecurtity (C/S)
        /// </summary>
        private void SetAsilCS()
        {
            if (this.Cs != null)
            {
                if (this.Cs == "csRelevant")
                {
                    this.TextCS = "C/" + GetAsilText();
                }
                else
                {
                    this.TextCS = "./" + GetAsilText();
                }
            }
        }

        /// <summary>
        /// Erzeugt die Einstellige Abkürzung für Asil
        /// </summary>
        /// <returns></returns>
        private string GetAsilText()
        {
            if (this.Asil == null)
            {
                return "-";
            }
            // c_string_value:  asila || asilb || asilc || asild || QM || qm || notrelevant
            if (this.Asil.Length > 2 && this.Asil.Substring(0, 2) == "as")
            {
                return this.Asil.Substring(4, 1).ToUpper();
            }
            else if (this.Asil.ToUpper() == "QM")
            {
                return ".";
            }
            else if (this.Asil == "notrelevant")
            {
                return ".";
            }
            return ".";
        }

        public string GetHighLowLevelText
        {
            get
            {
                if (this.LowLevel)
                {
                    return "L";
                }
                return "H";
            }

        }

        #region MakeAllocText
        /// <summary>
        /// Erzeugt einen beschreibenden Alloc Text für die Anzeige 
        /// </summary>
        private void MakeAllocText()
        {
            Alloc = "";
            if (this.Hardware)
            {
                Alloc += "HW";
            }
            if (this.Software)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "SW";
            }
            if (this.Mechanic)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "M";
            }
            if (this.Interface)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "I";
            }
            if (this.ESE)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "A";   // 2019-04-02 Änderung der Abkürzung ESE -> Arch
            }
            if (this.Test)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "T";
            }
            if (this.Prod)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "PD";
            }
            if (this.Env)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "E";
            }
            if (this.Process)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "PC";
            }
            if (this.QS)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "Q";
            }
            if (this.Project)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "PR";
            }
            if (this.Layout)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "L";
            }
            if (this.PM)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "PM";
            }
            if (this.SE)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "SE";
            }
            if (this.MIT)
            {
                if (Alloc.Length > 0) Alloc += ",";
                Alloc += "M";
            }
            if (Alloc.Length == 0)
            {
                Alloc = "[...]";
            }
        }
        #endregion MakeAllocText

        #region public Workitem CheckLinkTo(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole, string WorkitemType )
        /// <summary>
        /// Diese Funktion prüft ob ein Link mit den Parametern zu diesem Workitem existiert
        /// 2018-12-13 Logik korrigiert es werden nun richtige Weise die Downlinks überprüft
        /// </summary>
        /// <param name="myP">Projekt class</param>
        /// <param name="DocNamePrefix">zB.: 30</param>
        /// <param name="DocNameContains">zB.: Software</param>
        /// <param name="LinkRole">relates_to</param>
        /// <param name="WorkitemType">requirement, interface, component ...</param>
        /// <returns>linked Workitem or null</returns>
        public Workitem CheckLinkTo(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole, string WorkitemType)
        {
            Workitem Tempworkitem;
            string Documentname;

            if (this.Downlinks == null)
            {
                this.FillCheckDownlink(myP.Uplinks);
            }

            foreach (Uplink d in this.Downlinks)
            {
                Tempworkitem = myP.Workitems.Find(x => x.C_pk == d.WorkitemId); // 
                Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                if (Documentname.Length < 2)
                {
                    continue;
                }
                if (d.Role == LinkRole)
                {
                    if (Documentname.Substring(0, 2) == DocNamePrefix && Documentname.Contains(DocNameContains))
                    {
                        if (Tempworkitem.Type == WorkitemType)
                        {
                            return Tempworkitem;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region public Workitem CheckLinkFrom(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole, string WorkitemType )
        /// <summary>
        /// Diese Funktion prüft ob ein Link mit den Parametern zu diesem Workitem existiert
        /// </summary>
        /// <param name="myP">Projekt class</param>
        /// <param name="DocNamePrefix">zB.: 30</param>
        /// <param name="DocNameContains">zB.: Software</param>
        /// <param name="LinkRole">relates_to</param>
        /// <param name="WorkitemType">requirement, interface, component ...</param>
        /// <returns>linked Workitem or null</returns>
        public Workitem CheckLinkFrom(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole, string WorkitemType)
        {
            Workitem Tempworkitem;
            string Documentname;

            if (this.Uplinks == null)
            {
                this.FillCheckUplink(myP.Uplinks);
            }

            if (this.Downlinks == null)
            {
                this.FillCheckDownlink(myP.Uplinks);
            }

            foreach (Uplink u in this.Uplinks)
            {
                Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.UplinkId);
                if (Tempworkitem == null)
                {
                    continue;
                }
                Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                if (Documentname.Length < 2)
                {
                    continue;
                }
                if (u.Role == LinkRole)
                {
                    if (Documentname.Substring(0, DocNamePrefix.Length) == DocNamePrefix && Documentname.Contains(DocNameContains))
                    {
                        if (Tempworkitem.Type == WorkitemType)
                        {
                            return Tempworkitem;
                        }
                    }
                }
            }
            return null;
        }
        #endregion


        public Workitem CheckLinkFromDoc(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole, string WorkitemType, out Workitem ErrorWorkitem)
        {
            Workitem Tempworkitem;
            Workitem ew = new Workitem(); // Workitem bei Fehler
            string Documentname;

            if (this.Downlinks == null)
            {
                this.FillCheckDownlink(myP.Uplinks);
            }

            foreach (Uplink u in this.Downlinks)
            {
                Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem == null)
                {
                    continue;
                }
                Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                if (Documentname.Length < 2)
                {
                    continue;
                }
                if (u.Role == LinkRole)
                {
                    if (Documentname.Substring(0, DocNamePrefix.Length) == DocNamePrefix && Documentname.Contains(DocNameContains))
                    {
                        if (Tempworkitem.Type == WorkitemType)
                        {
                            ErrorWorkitem = ew;
                            return Tempworkitem;
                        }
                        else
                        {
                            // Link auf falsches Workitem gefunden
                            ew = Tempworkitem;
                        }
                    }
                }
            }
            ErrorWorkitem = ew;
            return null;
        }

        #region public bool CheckLinkExits(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole)
        /// <summary>
        /// Diese Funktion prüft ob ein Link mit den Parametern zu diesem Workitem existiert
        /// </summary>
        /// <param name="myP">Projekt class</param>
        /// <param name="DocNamePrefix">zB.: 30</param>
        /// <param name="DocNameContains">zB.: Software</param>
        /// <param name="LinkRole">relates_to</param>
        /// <returns>linked Workitem or null</returns>
        public bool CheckLinkExists(Project myP, string DocNamePrefix, string DocNameContains, string LinkRole)
        {
            Workitem Tempworkitem;
            string Documentname;

            foreach (Uplink u in this.Uplinks)
            {
                Tempworkitem = myP.Workitems.Find(x => x.C_pk == u.WorkitemId);
                Documentname = myP.GetDocumentname(Tempworkitem.DocumentId);
                if (Documentname.Length < 2)
                {
                    continue;
                }
                if (u.Role == LinkRole)
                {
                    if (Documentname.Substring(0, 2) == DocNamePrefix && Documentname.Contains(DocNameContains))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        // Approvals - Reviews:

        public int FillApprovals(List<WorkitemApproval> al, List<User> users)
        {
            if (this.Approvals == null)
            {
                this.Approvals = new List<WorkitemApproval>();
            }
            this.Approvals = al.FindAll(a => a.WorkitemId == C_pk);
            foreach (WorkitemApproval a in this.Approvals)
            {
                a.ApproverName = users.FirstOrDefault(u => u.C_pk == a.UserId).Name;
            }
            return this.Approvals.Count;
        }

        public bool AddClarificationRole(string ClarificatioRole)
        {
            if (ClarificationRoles == null)
            {
                ClarificationRoles = new List<string>();
            }
            ClarificationRoles.Add(SetClarificationRole(ClarificatioRole));
            ClarificationRoles.Sort();
            this.ClarificationRole = "";
            foreach (string s in ClarificationRoles)
            {
                if (this.ClarificationRole.Length > 0)
                {
                    this.ClarificationRole += ", ";
                }
                this.ClarificationRole += s;
            }
            return true;
        }

        public string SetClarificationRole(string ClarificationRole)
        {
            string s;

            if (ClarificationRole == "hw")
            {
                s = "HW";
            }
            else if (ClarificationRole == "sw")
            {
                s = "SW";
            }
            else if (ClarificationRole == "customer")
            {
                s = "Customer";
            }
            else if (ClarificationRole == "production")
            {
                s = "Production";
            }
            else if (ClarificationRole == "hwsw")
            {
                s = "HW/SW";
            }
            else if (ClarificationRole == "env")
            {
                s = "ENV";
            }
            else if (ClarificationRole == "requ_en")
            {
                s = "Requ Engineer";
            }
            else if (ClarificationRole == "qs")
            {
                s = "QS";
            }
            else if (ClarificationRole == "project")
            {
                s = "PM";
            }
            else
            {
                s = ClarificationRole;
            }
            return s;
        }

        public void FillComments(List<Comment> AllComments, List<User> Userlist)
        {
            this.Comments = AllComments.FindAll(c => c.WorkitemId == this.C_pk);
            foreach (Comment c in this.Comments)
            {
                c.UserName = Userlist.FirstOrDefault(u => u.C_pk == c.UserId).Name;
            }
            if (Comments.Count > 0)
            {
                Debug.WriteLine("Test");
            }
        }

        /// <summary>
        /// Sucht rekursiv alle Child Workitems
        /// </summary>
        /// <param name="p"></param>
        /// <param name="w"></param>
        /// <returns>Liste von Workitems</returns>
        public List<Workitem> GetAllChilds(Project p, Workitem w)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem Tempworkitem;

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(p.Uplinks);
            }
            foreach (Uplink d in w.Downlinks)
            {
                if (d.Role == "parent")
                {
                    Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                    wl.Add(Tempworkitem);
                    wl.AddRange(GetAllChilds(p, Tempworkitem));
                }
            }

            return wl;
        }

        /// <summary>
        /// Sucht alle direkten Childs eines Workitems
        /// </summary>
        /// <param name="p"></param>
        /// <param name="w"></param>
        /// <returns>Liste von Workitems</returns>
        public List<Workitem> GetDirectChilds(Project p, Workitem w)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem Tempworkitem;

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(p.Uplinks);
            }
            foreach (Uplink d in w.Downlinks)
            {
                if (d.Role == "parent")
                {
                    Tempworkitem = p.Workitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                    wl.Add(Tempworkitem);
                }
            }

            return wl;
        }

        /// <summary>
        /// Diese Routine prüft, ob der Typ des Workitems ein Feature | Workpackage | Testcase ist und somit in der 
        /// Impact Analyse Uplinks gesucht werden sollen
        /// </summary>
        /// <returns>
        /// true ... Uplink Check notwendig 
        /// false .. keine Uplink checken
        /// </returns>
        public bool CheckUplink()
        {
            if (Type == "testcase" || Type == "workPackage" || Type == "feature" || Type == "productFeature" || Type == "customerrequirement")
            {
                return true;
            }
            return false;
        }

        public string GetTimes
        {
            get
            {
                string Time = "";
                if (InitialEstimate > 0 || TimeSpent > 0 || RemainingEstimate > 0)
                {
                    Time = InitialEstimate.ToString() + "/" + TimeSpent.ToString() + "/" + RemainingEstimate.ToString();
                }

                return Time;
            }
        }
    }
}