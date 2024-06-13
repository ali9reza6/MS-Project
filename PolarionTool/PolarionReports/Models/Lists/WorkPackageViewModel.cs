using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Lists
{
    public class WorkPackageViewModel : RequViewModel
    {
        public string ProjectID { get; set; }
        public string StartingPoint { get; set; }
    }
}