using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database.Api
{
    public class PlanApiDB
    {
        public int c_pk { get; set; }
        public DateTime c_duedate { get; set; }
        public DateTime c_finishedon { get; set; }
        public string c_id { get; set; }
        public string c_name { get; set; }
        public int fk_parent { get; set; }
        public DateTime c_startdate { get; set; }
        public DateTime c_startedon { get; set; }
        public string c_status { get; set; }

        public DateTime c_updated { get; set; }

        public int c_sortorder { get; set; }

        public int fk_template { get; set; }

        public bool Milestone { get; set; }
    }
}