using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactTestcase
    {
        public ImpactDocument TestcaseDocument { get; set; }
        public List<Workitem> Testcases { get; set; }
        public string TooltipTestcases { get; set; }

        public ImpactTestcase()
        {
            Testcases = new List<Workitem>();
        }
    }
}