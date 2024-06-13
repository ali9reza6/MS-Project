using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemApproval
    {
        public int WorkitemId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string ApproverName { get; set; }

    }
}