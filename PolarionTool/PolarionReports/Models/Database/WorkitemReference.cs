using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemReference
    {
        public int WorkitemID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
    }
}