using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class TestrunCustomField
    {
        public int Fk_testrun { get; set; }
        public string CfName { get; set; }
        public string CfValue { get; set; }
    }
}