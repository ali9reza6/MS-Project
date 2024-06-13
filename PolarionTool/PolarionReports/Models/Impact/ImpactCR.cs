using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using PolarionReports.Models.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    /// <summary>
    /// Class für Anzeige: Impact Analyse eines CR's
    /// </summary>
    public class ImpactCR
    {
        public string PolarionLink { get; set; }
        public ProjectDB ProjectDB { get; set; }
        public string Titel { get; set; } // CR ID + Titel

        public bool FilterSW { get; set; }
        public bool FilterHW { get; set; }

        public string Filter { get; set; } // SW+HW Filter als URL Paramter: Filter=SH|S|H|OFF
        
        public Workitem CR { get; set; }
        
        public List<Workitem> RequirementsIncorrectlyLinkedToCR { get; set; }
        public string TooltipRequirementsIncorrectlyLinkedToCR { get; set; }

        public List<Workitem> RequirementsAffectedByCR { get; set; }
        public string TooltipRequirementsAffectedByCR { get; set; }

        public RequViewModel RequirementList { get; set; }
        public RequErrorViewModel RequirementErrorList { get; set; }
    }
}