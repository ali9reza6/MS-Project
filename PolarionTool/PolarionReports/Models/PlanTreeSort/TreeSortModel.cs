using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.PlanTreeSort
{
    public class TreeSortModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProjectId { get; set; }
        public List<TreeSort> TreesortList { get; set; }
    }
}