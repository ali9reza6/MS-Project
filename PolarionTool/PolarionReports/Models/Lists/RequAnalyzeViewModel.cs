using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Lists
{
    public class RequAnalyzeViewModel
    {
        public string ProjectID { get; set; }
        public string StartingPoint { get; set; }
        public string Titel { get; set; }
        public string Id { get; set; }
        public string Color { get; set; }
        public string Tooltip { get; set; }
        public string PolarionLink { get; set; }
        public string PolarionDocumentLink { get; set; }
        public HtmlString PolarionTableLink { get; set; }
        public List<Workitem> Table { get; set; }
        public int ExpandLevel { get; set; }
    }
}