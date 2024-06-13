using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactFeature
    {
        public ImpactDocument FeatureDocument { get; set; }
        public List<Workitem> Features { get; set; }
        public string TooltipFeatures { get; set; }

        public ImpactFeature()
        {
            Features = new List<Workitem>();
        }
    }
}