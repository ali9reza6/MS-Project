using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class TestViewModel
    {
        public string Message { get; set; }
        public string TestHtmlContent { get; set; }
        public Workitem w { get; set; }

        public string NewPlanName { get; set; }

    }
}