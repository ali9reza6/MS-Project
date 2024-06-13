using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class PlanCustomField
    {
        public int Fk_plan { get; set; }
        public string CfName { get; set; }
        public string CfValue { get; set; }
    }
}