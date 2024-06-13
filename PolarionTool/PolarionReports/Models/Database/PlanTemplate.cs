using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class PlanTemplate
    {
        public int C_Pk { get; set; }
        public string C_Id { get; set; }
        public string C_Name { get; set; }
        public string C_Allowedtypes { get; set; }
        public string C_Color { get; set; }
        public string C_Description { get; set; }
        public string C_Homepagecontent { get; set; }

    }
}