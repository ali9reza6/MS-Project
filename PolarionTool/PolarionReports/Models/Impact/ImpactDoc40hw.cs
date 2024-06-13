using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactDoc40hw
    {
        public ImpactDocument Doc40hw { get; set; }
        public List<Workitem> HardwareRequrirements { get; set; }
        public string TooltipHardwareRequrirements { get; set; }
    }
}