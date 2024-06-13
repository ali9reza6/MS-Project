using PolarionReports.BusinessLogic;
using PolarionReports.Models.Lists;
using PolarionReports.Models.TableRows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Document
    {
        public enum Browser { IE, Chrome, Firefox }
        public Browser BrowserTyp { get; set; }
        public int C_pk { get; set; }
        public int ProjectId { get; set; }
        public string C_id { get; set; }
        public string C_type { get; set; }
        public string C_modulefolder { get; set; }
        public string C_status { get; set; }
        public string PolarionDocumentLink { get; set; }
        public string PolarionTableLink { get; set; }
        public int UplinkCounter { get; set; }

        public string PolarionChecklistLink { get; set; }
        public Document Checklist { get; set; }

        public DocumentName DocName { get; set; }

        // Listen vom 10-Check
        public Check10Lists check10Lists { get; set; }

        public List<Workitem> LinkedRequirementList { get; set; }
        public List<Workitem> ErrorList { get; set; }
        public List<Workitem> WrongDocumentLinkList { get; set; }
        public List<Workitem> RejectedList { get; set; }
        public List<Workitem> DeletedList { get; set; }

        // Obsulete lists in new Check20
        public List<Workitem> MissingRefInterface { get; set; }
        public List<Workitem> RefInterface { get; set; }
        public List<Workitem> MissingRefSW { get; set; }
        public List<Workitem> RefSW { get; set; }
        public List<Workitem> MissingRefHW { get; set; }
        public List<Workitem> RefHW { get; set; }

        public List<Workitem> Error30Interface { get; set; }
        public List<WorkitemLinkError> IncorrectlyLinkedElementRequirements { get; set; }

        public List<Workitem> Missing30Links { get; set; }

        public List<Workitem> Links30 { get; set; }                        // Alte Liste für den bestehenden 20 Check
        public List<WorkitemLinks> LinkedElementRequirements { get; set; } // Neue Liste im 20N Check mit Links

        public List<Workitem> StatusDraft { get; set; }
        public List<Workitem> StatusClarificationNeeded { get; set; }
        public List<Workitem> StatusReviewNeeded { get; set; }
        public List<Workitem> StatusDeferred { get; set; }
        public List<Workitem> StatusTBD { get; set; }
        public List<Workitem> SeverityShouldHave { get; set; }
        public List<Workitem> AllocMissing { get; set; }
        public List<Workitem> AllocInvalid { get; set; }    // Neue Liste ersetzt: AllocInterface und AllocESE
        public List<Workitem> AllocInterface { get; set; }
        public List<Workitem> AllocESE { get; set; }
        public List<Workitem> FuncReqHSM { get; set; }
        public List<Workitem> MissingHyperlink { get; set; }
        public List<Workitem> Hyperlinks { get; set; }
        public List<Workitem> SpecialReportCustomerAction { get; set; }
        public List<Workitem> SpecialReportSupplierAction { get; set; }
        public List<Workitem> SpecialReportESESW { get; set; }
        public List<Workitem> SpecialReportESEHW { get; set; }
        public List<Workitem> SafetyRelevant { get; set; }          // 2019-03-28
        public List<Workitem> CybersecurityRelevant { get; set; }   // 2019-03-28
        public List<Workitem> WorkitemsInBin { get; set; }

        // Neue 20 Check Listen:
        public List<Workitem> HighLevelElementRequirementsWithoutBreakDown { get; set; }
        public List<Workitem> BreakDown30InterfaceRequirements { get; set; }
        public List<Workitem> BreakDown40SWRequirements { get; set; }
        public List<Workitem> BreakDown40HWRequirements { get; set; }

        public List<Workitem> UnlinkedSoftwareElementRequirements { get; set; }
        public List<WorkitemLinkError> IncorrectlyLinkedSoftwareElementRequirements { get; set; }
        public List<WorkitemLinks> LinkedSoftwareElementRequirementsWithInvalidAllocation { get; set; }
        public List<WorkitemLinks> LinkedSoftwareElementRequirements { get; set; }

        public List<Workitem> UnlinkedHardwareElementRequirements { get; set; }
        public List<WorkitemLinkError> IncorrectlyLinkedHardwareElementRequirements { get; set; }
        public List<WorkitemLinkError> LinkedHardwareElementRequirementsWithInvalidAllocation { get; set; }
        public List<WorkitemLinkError> LinkedHardwareElementRequirements { get; set; }

        // Break Down Listen
        public List<Workitem> HighLevelRequirementsWithBreakDownRequirementsErroneouslySpecifiedByZKW { get; set; }
        public List<Workitem> HighLevelRequirementsToBeSpecifiedByCustomerWithoutAnyBreakDownRequirements { get; set; }
        public List<Workitem> HighLevelHWRequirementsToBeSpecifiedInSWRequirements { get; set; }
        public List<Workitem> HighLevelSWRequirementsToBeSpecifiedInHWRequirements { get; set; }
        public List<Workitem> HighLevelSWHWMechRequirementsToBeSpecifiedInTheInterfaceRequirementsWithInvalidAllocation { get; set; }
        public List<Workitem> HighLevelSWRequirementsToBeSpecifiedInSWRequirementsWithInvalidAllocation { get; set; }
        public List<Workitem> HighLevelHWRequirementsToBeSpecifiedInHWRequirementsWithInvalidAllocation { get; set; }
        public List<Workitem> HighLevelRequirementsToBeFurtherSpecifiedByZKWWithoutAnyBreakDownRequirements { get; set; }
        public List<Workitem> HighLevelElementRequirementsWithBreakDownRequirementsByCustomer { get; set; }
        public List<Workitem> HighLevelElementRequirementsWithBreakDownRequirementsByZKW { get; set; }

        // neue Liste im 20 Check - implementiert wie im 30AN-Check
        public List<Workitem> HighLevelRequirementsWithoutAnyBreakDownRequirements { get; set; }

        // Neue Liste in "Linkage to 30 Element Architecture"
        public List<WorkitemLinks> LinkedElementRequirementsWithInvalidAllocation { get; set; }

        // HW/ESE requirements linked to Checklist“
        public List<Workitem> HwEseRequirementsNotLinkedToChecklist { get; set; }
        public List<WorkitemLinkError> HwEseRequirementsLinkedToChecklist { get; set; }

        // Checklist Report
        public List<Workitem> TestcasesNotLinkedToElementRequirements { get; set; }
        public List<WorkitemLinkError> TestcasesLinkedToElementRequirements { get; set; }

        // 2019-08-09 Neue Listen Verification
        public List<RequTestcaseUnlinked> UnlinkedElementRequirements { get; set; }
        public List<RequTestcaseError> IncorrectlyLinkedElementRequirementsToTestcases { get; set; }
        public List<RequTestcase> LinkedElementRequirementsToTestcases { get; set; }
        public List<RequTestcase> ElementRequirementsWithMissingVerificationProperties { get; set; }

        // 2019-11-13 Liste für Document & Signature Status
        public DocSignatureStatusBL DocSignatureStatus;

        // Review Class
        public Review Review { get; set; }

        public Document()
        {

        }

        public void SetBrowserType(string Browser)
        {
            if (Browser == "InternetExplorer")
            {
                BrowserTyp = Document.Browser.IE;
            }
            else
            {
                BrowserTyp = Document.Browser.Chrome;
            }
        }

        /// <summary>
        /// Liefert den Level des Dokuments (10, 20, 30, ....)
        /// </summary>
        public string DocumentLevel
        {
            get
            {
                return this.C_id.Substring(0, 2);
            }
        }

        /// <summary>
        /// Liefert den Index des Dokuments (a, b, c, ...) oder "" wenn nicht vorhanden
        /// </summary>
        public string DucumentIndex
        {
            get
            {
                if (this.C_id.Substring(2, 1) != " ")
                {
                    return this.C_id.Substring(2, 1);
                }
                else
                {
                    return ""; // Index es gibt nur ein Dokument auf dieser Ebene
                }
            }
        }

        /// <summary>
        /// Liefert den Subindex eines Dokuments = string nach "_" bis zum nächsten " "
        /// </summary>
        public string DocumentSubIndex
        {
            get
            {
                return "";
            }
        }

        public string DocumentLink
        {
            get
            {
                //string link = "http://wnsvpol1/polarion/#/project/" + myP.C_id + "/wiki/Specification/" +
                //                              C_id + "?selection=";
                return "";
            }
        }

        public string PolarionTableLinkReviewFromWorkitems(List<Workitem> Workitems)
        {
            string Link = "";
            string nextLink = "";
            int counter = 0;
            int maxlength = 2000 - 220; // ie11 hat ´Beschränkung auf 2048 Byte lange hrefs bzw url Länge

            if (BrowserTyp != Browser.IE) maxlength = 16384 - 220; // Std. Länge bei IIS

            foreach (Workitem w in Workitems)
            {
                if (Link.Length > 2)
                {
                    nextLink = " || ";
                }
                nextLink += "id=" + w.Id;
                counter++;
                if ((Link.Length + nextLink.Length + (counter * 10)) > maxlength)
                {
                    break;
                }
                Link += nextLink;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&sidebar=approvals&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        #region public string PolarionTableLinkFromWorkitemIds(List<string> workitemIds)
        /// <summary>
        /// Neue Variante für das Erzeugen der Table-Ansicht aller Workitems einer Tabelle
        /// </summary>
        /// <param name="workitemIds">Liste der WorkitemIds</param>
        /// <returns>Link zu Poalrion</returns>
        public string PolarionTableLinkFromWorkitemIds(List<string> workitemIds)
        {
            string Link = "";
            string nextLink = "";
            int counter = 0;
            int maxlength = 2000 - 220; // ie11 hat Beschränkung auf 2048 Byte lange hrefs bzw url Länge

            if (BrowserTyp != Browser.IE) maxlength = 16384 - 220; // Std. Länge bei IIS

            foreach (string id in workitemIds)
            {
                if (Link.Length > 2)
                {
                    nextLink = " || ";
                }
                nextLink += "id=" + id;
                counter++;
                if ((Link.Length + nextLink.Length + (counter * 10)) > maxlength)
                {
                    break;
                }
                Link += nextLink;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }
        #endregion

        public string PolarionTableLinkFromWorkitems(List<Workitem> Workitems)
        {
            string Link = "";
            string nextLink = "";
            int counter = 0;
            int maxlength = 2000 - 220; // ie11 hat Beschränkung auf 2048 Byte lange hrefs bzw url Länge

            if (BrowserTyp != Browser.IE) maxlength = 16384 - 220; // Std. Länge bei IIS

            foreach (Workitem w in Workitems)
            {
                if (Link.Length > 2)
                {
                    nextLink = " || ";
                }
                nextLink += "id=" + w.Id;
                counter++;
                if ((Link.Length + nextLink.Length + (counter * 10)) > maxlength)
                {
                    break;
                }
                Link += nextLink;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        public string PolarionTableLinkFromWorkitems(List<WorkitemLinkError> WorkitemLinkErrors)
        {
            string Link = "";
            string nextLink = "";
            int counter = 0;
            int maxlength = 2000 - 220; // ie11 hat Beschränkung auf 2048 Byte lange hrefs bzw url Länge

            if (BrowserTyp != Browser.IE) maxlength = 16384 - 220; // Std. Länge bei IIS

            foreach (WorkitemLinkError wle in WorkitemLinkErrors)
            {
                if (Link.Length > 2)
                {
                    nextLink = " || ";
                }
                nextLink += "id=" + wle.Workitem.Id;
                counter++;
                if ((Link.Length + nextLink.Length + (counter * 10)) > maxlength)
                {
                    break;
                }
                Link += nextLink;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        public string PolarionTableLinkFromWorkitems(List<WorkitemLinks> WorkitemLink)
        {
            string Link = "";
            string nextLink = "";
            int counter = 0;
            int maxlength = 2000 - 220; // ie11 hat Beschränkung auf 2048 Byte lange hrefs bzw url Länge

            if (BrowserTyp != Browser.IE) maxlength = 16384 - 220; // Std. Länge bei IIS

            foreach (WorkitemLinks wl in WorkitemLink)
            {
                if (Link.Length > 2)
                {
                    nextLink = " || ";
                }
                nextLink += "id=" + wl.Workitem.Id;
                counter++;
                if ((Link.Length + nextLink.Length + (counter * 10)) > maxlength)
                {
                    break;
                }
                Link += nextLink;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }
    }

    public class DocumentName
    {
        public string OriginalDocumentname { get; set; }
        public string Level { get; set; }
        public string Prefix { get; set; }
        public string Subindex { get; set; }
        public string Number { get; set; }
        public bool InvalidDocumentName { get; set; }
        public string ErrorMessage { get; set; }


        public DocumentName(string Document)
        {
            char Test;
            int Startindex = 0;
            int i;
            int x = 0;

            this.OriginalDocumentname = Document;

            if (Document == null || Document.Length < 2)
            {
                return;
            }

            if (char.IsNumber(Document, 0) && char.IsNumber(Document, 1))
            {
                this.Level = Document.Substring(0, 2);
            }
            else
            {
                // kein relevantes Dokument
                return;
            }
            this.Level = Document.Substring(0, 2);
            if (Document.Substring(2, 1) == " " || Document.Substring(2, 1) == "_")
            {
                // kein Prefix, kein Subindex, keine Nummerierung
            }
            else
            {
                // an der 3. Stelle steht der Präfix sollte ein kleiner Buchstabe sein
                Test = Convert.ToChar(Document.Substring(2, 1));

                if (Char.IsUpper(Document, 2))
                {
                    this.InvalidDocumentName = true;
                    this.ErrorMessage = "Invalid Charakter an Pos 3";
                    return;
                }

                if (char.IsNumber(Document, 2))
                {
                    this.InvalidDocumentName = true;
                    this.ErrorMessage = "Number an Pos 3";
                    return;
                }

                this.Prefix = Document.Substring(2, 1);

                if (char.IsNumber(Document, 3))
                {
                    // Document hat Nummerierung
                    this.Number = Document.Substring(3, 1);
                    Startindex = 4;
                }
                else
                {
                    Startindex = 3;
                }
                if (Document.Substring(Startindex, 1) == "_")
                {
                    // Document hat subindizes
                    for (i = Startindex + 1; i < Document.Length; i++)
                    {
                        if (Document.Substring(i, 1) == " ")
                        {
                            // Ende des Subindex
                            break;
                        }
                        else
                        {
                            x++;
                        }
                    }

                }
                if (x > 0)
                {
                    this.Subindex = Document.Substring(Startindex + 1, x);
                }

            }

        }
    }
}