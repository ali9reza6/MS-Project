using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class Check30EAViewModel
    {
        public string Projectname { get; set; }
        public string PolarionLink { get; set; }
        public string PolarionTableLink { get; set; }
        public string Errorname { get; set; }
        public string ErrorMsg { get; set; }

        public Document Document { get; set; }

        // 4.1.1 Linkage within 30 Element Architecture
        public List<Workitem> UnassociatedAdditionalRequirements { get; set; }
        public List<Workitem> IncorrectlyAssociatedAdditionalRequirements { get; set; }
        public List<Workitem> AssociatedAdditionalRequirements { get; set; }

        // 4.1.2 Linkage to 30 Interface Requirements
        public List<Workitem> UnlinkedAdditionalInterfaceRequirements { get; set; }
        public List<Workitem> LinkedAdditionalRequirementsWithInvalidAllocation { get; set; }
        public List<Workitem> LinkedAdditionalInterfaceRequirements { get; set;}

        // 4.1.3 Linkage to 40 Software Requirements
        public List<Workitem> UnlinkedAdditionalSoftwareRequirements { get; set; }
        public List<Workitem> LinkedAdditionalRequirementsWithInvalidAllocation40SW { get; set; }
        public List<Workitem> LinkedAdditionalSoftwareRequirements { get; set; }

        // 4.1.4 Linkage to 50 SW Architectural Design
        public List<Workitem> IncorrectlyLinkedSoftwareRequirements { get; set; }
        public List<Workitem> LinkedAdditionalRequirementsWithInvalidAllocation50SW { get; set; }
        public List<Workitem> LinkedAdditionalSoftwareRequirements50SW { get; set; }

        // 4.1.5 Linkage to 40 Hardware Requirements
        public List<Workitem> UnlinkedAdditionalHardwareRequirements { get; set; }
        public List<Workitem> LinkedAdditionalRequirementsWithInvalidAllocation40HW { get; set; }
        public List<Workitem> LinkedAdditionalHardwareRequirements { get; set; }

        // 4.1.6 Linkage to 50 HW Architectural Design (if applicable)
        public List<Workitem> IncorrectlyLinkedHardwareRequirements { get; set; }
        public List<Workitem> LinkedAdditionalRequirementsWithInvalidAllocation50HW { get; set; }
        public List<Workitem> LinkedAdditionalHardwareRequirements50HW { get; set; }

        // 4.2.1 Requirements Status
        public List<Workitem> StatusTBD { get; set; }
        public List<Workitem> StatusDraft { get; set; }
        public List<Workitem> StatusClarificationNeeded { get; set; }
        public List<Workitem> StatusDeferred { get; set; }
        public List<Workitem> SeverityShouldHave { get; set; }
        public List<Workitem> RejectedList { get; set; }

        // 4.2.2 Requirement Allocation
        public List<Workitem> AllocMissing { get; set; }
        public List<Workitem> InvalidAllocation { get; set; }
        public List<Workitem> AllocInterface { get; set; }
        public List<Workitem> AllocESE { get; set; }

        // 5.2.3 Functional requirements
        // => Document.FuncReqHSM

        // 4.2.3 Special reports
        public List<Workitem> ComponentsWithoutLinkedRequirements { get; set; }
        public List<Workitem> InterfacesWithoutLinkedRequirements { get; set; }
        public List<Workitem> SpecialReportESESW { get; set; }
        public List<Workitem> SpecialReportESEHW { get; set; }

        public List<Workitem> WorkitemInBin { get; set; }
        public Tooltip30EA Tooltip { get; set; }
        public TooltipReview TooltipReview { get; set; }

        public string PolarionTableLinkFromWorkitems(List<Workitem> Workitems)
        {
            string Link = "";

            foreach (Workitem w in Workitems)
            {
                if (Link.Length > 2) Link += " || ";
                Link += "id=" + w.Id;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
            
        }

    }
}