using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class PlanTemplateViewModel
    {
        public ProjectDB Project { get; set; }
        public string Message { get; set; }
        public string ErrorMsg { get; set; }
        public List<PlanTemplate> InsertedTemplates { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}