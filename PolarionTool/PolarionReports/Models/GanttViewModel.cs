using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class GanttViewModel
    {
        public string Project { get; set; }
        public string Baseplan { get; set; } 
        public string DataUrl { get; set; }
    }
}