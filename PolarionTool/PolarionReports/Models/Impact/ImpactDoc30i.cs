using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactDoc30i
    {
        public ImpactDocument Doc30i { get; set; }
        public List<Workitem> InterfaceRequirements { get; set; }
        public string TooltipInterfaceRequirements { get; set; }
    }
}