using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class Check20ViewModel
    {
        public string Projectname { get; set; }
        public string Errorname { get; set; }
        public string PolarionLink { get; set; }

        public List<Document> Documents { get; set; }
        public Tooltip20 Tooltip20 { get; set; }
        public TooltipReview TooltipReview { get; set; }
        public Document ActualDocument { get; set; }

        public TableLinkError TableLinkError { get; set; }

        public TableLink TableLink { get; set; }

        public RequViewModel RequirementList { get; set; }
        public RequApprovalViewModel RequApprovalList { get; set; }
        public RequErrorViewModel RequirementErrorList { get; set; }

        public RequTestcaseListViewModel RequTestcaseList { get; set; }
        public RequTestcaseErrorListViewModel RequTestcaseErrorList { get; set; }
        public RequTestcaseUnlinkedListViewModel RequTestcaseUnlinkedList { get; set; }
    }
}