using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class NavigationView
    {
        public ProjectDB Project  { get; set; }
        public string LinkCheck10 { get; set; }
        public string LinkCheck20 { get; set; }
        public string LinkCheck20N { get; set; }
        public string LinkCheck30 { get; set; }
        public string LinkCheck30N { get; set; }
        public string LinkImpact { get; set; }
        public string PlantemplateInsert { get; set; }
        public string PlanInsert  { get; set; }
        public string PlanInsertFromTemplate { get; set; }
        public string PlanTreeSort { get; set; }
        public string GanttSW { get; set; }

        List<DocumentDB> Doc20 { get; set; }
    }
}