using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class HomeView
    {
        public ProjectgroupList Projectgroups { get; set; }
        public ProjectDBList Projects { get; set; }
        public string ErrorMsg { get; set; }

        public HomeView()
        {
            ErrorMsg = "";
        }
    }
}