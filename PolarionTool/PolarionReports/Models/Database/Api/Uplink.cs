using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database.Api
{
    public class Uplink
    {
        public WorkitemBase Down { get; set; }
        public string c_role { get; set; }
        public WorkitemBase Up { get; set; }
    }
}