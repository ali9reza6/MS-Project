using PolarionReports.Models.Database;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class PlanViewModel
    {
        public ProjectDB Project       { get; set; }
        public ProjectDB Template      { get; set; }
        public PlanList  ProjectPlans  { get; set; }
        public PlanList  TemplatePlans { get; set; }

        public TreeViewFieldsSettings PlanFields { get; set; }
        public TreeViewFieldsSettings TempFields { get; set; }

        public string TargetPlanPK { get; set; }
        public string TemplatePlanPK { get; set; }
        public string TemplatePlanId { get; set; }

        public int    ProjectPK { get; set; }
        public string ProjectId { get; set; }
        public string InsertAction { get; set; }

        public string ErrorMsg { get; set; }
        public string Message { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string NewPlanName { get; set; }

    }
}