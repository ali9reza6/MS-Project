using PolarionReports.Models.Gantt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class PlanSortViewModel
    {
        public string ProjectId { get; set; }
        public string BasePlan { get; set; }
        public string DataUrl { get; set; }
        public string Message { get; set; }
        public List<GanttData> Plans { get; set; }
    }
}