using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemLinkError
    {
        public Workitem Workitem { get; set; }
        public Workitem LinkedWorkitem { get; set; }
        public string ErrorMsg;
        public string IdLinkedWorkitem;
        public string PolarionLink;
    }
}