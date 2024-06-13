using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactChild
    {
        public ImpactDocument ChildDocument { get; set; }
        public List<Workitem> Childs { get; set; }
        public string TooltipChilds { get; set; }

        public ImpactChild()
        {
            Childs = new List<Workitem>();
        }
    }
}