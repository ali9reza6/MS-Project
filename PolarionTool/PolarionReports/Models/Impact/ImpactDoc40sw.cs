using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactDoc40sw
    {
        public ImpactDocument Doc40sw { get; set; }
        public List<Workitem> SoftwareRequirements { get; set; }
        public string TooltipSoftwareRequirements { get; set; }
    }
}