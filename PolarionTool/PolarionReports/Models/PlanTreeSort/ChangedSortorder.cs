using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.PlanTreeSort
{
    public class ChangedSortorder
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public int C_Pk { get; set; }
        public string PlanId { get; set; }
    }
}