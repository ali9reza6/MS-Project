using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class WorkitemView
    {
        public string Heading { get; set; }
        public string Document { get; set; }
        public Workitem workitem { get; set; }
        public string PolarionLink { get; set; }
        public string PolarionDocumentLink { get; set; }
    }
}