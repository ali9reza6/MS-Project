using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Downlink
    {
        public int WorkitemId { get; set; }
        public int DownlinkId { get; set; }
        public string Role { get; set; }
    }
}