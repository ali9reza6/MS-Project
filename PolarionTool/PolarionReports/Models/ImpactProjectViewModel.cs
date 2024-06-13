using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class ImpactProjectViewModel
    {
        public ProjectDB projectDB { get; set; }

        public string ErrorMsg { get; set; }

        public string ProjectID { get; set; }

        public string ProjectTrackerID { get; set; }

        [Required]
        [Display(Name = "Req or CR")]
        public string AnalyseWorkitemId { get; set; }

        public ImpactProjectViewModel()
        {

        }

        public ImpactProjectViewModel(string ProjectID)
        {
            DatareaderP dr;

            dr = new DatareaderP();
            projectDB = dr.GetProjectByID(ProjectID, out string Error);
            ErrorMsg = Error + ", ";
        }
    }
}