using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemAssignee
    {
        public int Workitem_PK { get; set; }
        public int User_PK { get; set; }
        public string UserName { get; set; }
    }
}