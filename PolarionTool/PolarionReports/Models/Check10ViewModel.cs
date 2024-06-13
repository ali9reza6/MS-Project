using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using PolarionReports.Models.Tooltip;

namespace PolarionReports.Models
{
    public class Check10ViewModel
    {
        public string Projectname { get; set; }
        public string Errorname { get; set; }
        public string PolarionLink { get; set; }

        public List<Document> Documents { get; set; }
        public Tooltip10 Tooltip10 { get; set; }
        public TooltipReview TooltipReview { get; set; }
        public Document ActualDocument { get; set; }

        public TableLinkError TableLinkError { get; set; }

        public TableLink TableLink { get; set; }

    }
}