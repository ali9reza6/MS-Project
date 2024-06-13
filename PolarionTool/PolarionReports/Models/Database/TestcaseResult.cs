using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class TestcaseResult
    {
        public DateTime c_executed { get; set; }
        public string c_result { get; set; }
        public string c_name { get; set; }
    }
}