using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class CheckViewModel
    {
        public string Projectname { get; set; }
        public string Errorname { get; set; }
        public string PolarionLink { get; set; }

        public List<Document> Documents { get; set; }
        public Tooltip20 Tooltip20 { get; set; }
        public TooltipReview TooltipReview { get; set; }
    }
}