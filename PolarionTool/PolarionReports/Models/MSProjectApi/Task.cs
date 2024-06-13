using Newtonsoft.Json;
using PolarionReports.Models.Database.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Serilog;

namespace PolarionReports.Models.MSProjectApi
{
    public class Task : TaskBase
    {

        //public long SortOrder { get; set; }

        public Task()
        {

        }

        /// <summary>
        /// Init MS-Project Task with Polarion Plan
        /// </summary>
        /// <param name="pp">Polarion Plan</param>
        /// <param name="ParentId">c_id des parents (Achtung nicht c_pk)</param>
        public Task(PlanApiDB pp, string ParentId)
        {
            PlanId = pp.c_id;
            Name = this.GetNameWithoutWBSCode(pp.c_name);
            Start = pp.c_startdate;
            Finish = pp.c_duedate;
            PolarionUpdate = pp.c_updated;
            this.ParentId = ParentId;
            this.WBSCode = GetWBSCodeFromName(pp.c_name);
            this.Milestone = IsMailestone(pp.c_name);
            this.ProjectId = pp.c_sortorder;
            this.Status = pp.c_status;
            this.PtProcess = "";
            this.PtRole = "";
        }

        public Task(PlanApiDB pp, PmWorkPackageDB wp, string ParentId)
        {
            PlanId = pp.c_id;
            WorkitemId = wp.c_id;
            Name = this.GetNameWithoutWBSCode(pp.c_name);
            Start = pp.c_startdate;
            Finish = pp.c_duedate;
            PolarionUpdate = pp.c_updated;
            if (wp.c_initialestimate > 0 && wp.c_timespent > 0)
            {
                PercentComplete = Convert.ToInt32(wp.c_timespent / wp.c_initialestimate * 100);
            }
            this.ParentId = ParentId;
            this.WBSCode = GetWBSCodeFromName(pp.c_name);
            this.Milestone = IsMailestone(pp.c_name);
            this.ProjectId = pp.c_sortorder;
            this.Status = pp.c_status;
            this.PtProcess = "";
            this.PtRole = "";
        }

        /// <summary>
        /// Task aus einem projectTask erzeugen
        /// Create a task from a projectTask
        /// </summary>
        /// <param name="wp">projectTask</param>
        /// <param name="ParentId">c_id des Parents (pmWorkpackage)</param>
        public Task(PmWorkPackageDB wp, string ParentId)
        {
            PlanId = null;
            WorkitemId = wp.c_id;
            Name = this.GetNameWithoutWBSCode(wp.c_title);
            Start = wp.c_plannedstart;
            Finish = wp.c_duedate;
            PolarionUpdate = wp.c_updated;
            if (wp.c_initialestimate > 0 && wp.c_timespent > 0)
            {
                PercentComplete = Convert.ToInt32(wp.c_timespent / wp.c_initialestimate * 100);
            }
            this.ParentId = ParentId;
            this.WBSCode = GetWBSCodeFromName(wp.c_title);
            this.Milestone = false;
            this.Status = wp.c_status;
            this.PtProcess = "";
            this.PtRole = "";
        }

        public static string GetWBSCodeFromName(string Name)
        {
            string[] Nameparts = Name.Split(' ');
            string WBSCode = Nameparts[0];

            if (WBSCodeValid(WBSCode))
            {
                return WBSCode;
            }
            else
            {
                return "";
            }
        }

        public string GetNameWithoutWBSCode(string Name)
        {
            // überprüfen, ob der Name mit einem WBSCode beginnt
            string WBSCode = GetWBSCodeFromName(Name);

            if (WBSCodeValid(WBSCode))
            {
                return Name.Substring(WBSCode.Length+1);
            }
            else
            {
                return Name;
            }            
        }

        public static bool WBSCodeValid(string WBSCode)
        {
            if (WBSCode == null || WBSCode.Length == 0)
            {
                return false;
            }

            Regex WBSCodeRegex = new Regex(@"[\d.]");
            MatchCollection m = WBSCodeRegex.Matches(WBSCode);

            if (m.Count == WBSCode.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsMailestone(string Name)
        {
            string[] parts = this.GetNameWithoutWBSCode(Name).Split(' ');
            Regex MilestoneRegex = new Regex(@"[M\d]");
            MatchCollection m = MilestoneRegex.Matches(parts[0]);

            if (m.Count == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetParentWBSCode(string WBSCode)
        {

            if (this.WBSCode.Length == 1)
            {
                // = Wurzel = BasaPlan
                return "";
            }
            string parentWBSCode = "";
            string[] Parts = this.WBSCode.Split('.');
            for (int i = 0; i < (Parts.Count() - 1); i++)
            {
                parentWBSCode += Parts[i] + ".";
            }

            // remove last .
            try
            {
                parentWBSCode = parentWBSCode.Substring(0, parentWBSCode.Length - 1);
                return parentWBSCode;
            }
            catch (Exception)
            {
                Log.Error("Please make sure all the WBS numbers in the file are correct ");
                throw;
            }
            
        }

        [JsonIgnore]
        public long WbsSortOrder
        {
            get
            {
                try
                {
                    long ret = 0;
                    if (WBSCode == null) return 0;
                    var split = WBSCode.Split('.');
                    long multi = (long)Math.Pow(100, 7);
                    WBSCode.Split('.').ToList().ForEach(x => { ret += (int.Parse(x) * multi); multi /= 100; });
                    return ret;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(WBSCode + ex);
                    Debug.WriteLine(this.PlanId + ex);
                    return 0;
                }
            }

        }
    }
}