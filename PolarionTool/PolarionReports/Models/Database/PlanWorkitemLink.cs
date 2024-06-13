using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class PlanWorkitemLink
    {
        public int PlanPK     { get; set; }
        public int WorkitemPK { get; set; }
        public string WorkitemId { get; set; }
        public string PlanId { get; set; }
    }

    public class PlanWorkitemLinkList
    {
        /// <summary>
        /// Liste von Links Plan --> Workitem
        /// </summary>
        public List<PlanWorkitemLink> PlanWorkitemLinks { get; set; }

        public bool SetPlannedId(Workitem w)
        {
            if (w.PlannedIn == null || w.PlannedIn.Length == 0)
            {
                List<PlanWorkitemLink> PIList = PlanWorkitemLinks.FindAll(x => x.WorkitemPK == w.C_pk);
                if (PIList != null && PIList.Count > 0)
                {
                    foreach (PlanWorkitemLink pi in PIList)
                    {
                        if (w.PlannedIn == null) w.PlannedIn = "";
                        if (w.PlannedIn.Length > 0) w.PlannedIn += "/";
                        w.PlannedIn += pi.PlanId;
                    }
                }
                else
                {
                    w.PlannedIn = "";
                }
            }

            return true;
        }
    }
}