using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    /// <summary>
    /// Class für Anzeige Impact Analyse von Doc Level 20
    /// </summary>
    public class ImpactDoc20
    {
        public ImpactDocument Doc20 { get; set; }
        public Workitem WorkitemToAnalyze { get; set; }
        public List<Workitem> ElemetRequirements { get; set; }

        public ImpactDoc20(Impact impact,DocumentDB doc)
        {
            Doc20 = new ImpactDocument(impact.ProjectDb, doc);
        }
    }
}