using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactWorkPackage
    {
        public ImpactDocument WorkPackageDocument { get; set; }
        public List<Workitem> WorkPackages { get; set; }
        public string TooltipWorkPackeges { get; set; }

        public ImpactWorkPackage()
        {
            WorkPackages = new List<Workitem>();
        }
    }
}