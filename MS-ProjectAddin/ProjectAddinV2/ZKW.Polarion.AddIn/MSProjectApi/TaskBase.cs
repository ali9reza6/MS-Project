using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKW.Shared.MSProjectApi
{
    public class TaskBase
    {
        [JsonIgnore]
        public long WbsSortOrder
        {
            get
            {
                long ret = 0;
                if (!string.IsNullOrWhiteSpace(WBSCode))
                {
                    var split = WBSCode.Split('.');
                    long multi = (long)Math.Pow(100.0, 7.0);
                    WBSCode.Split('.').ToList().ForEach(x => { ret += (int.Parse(x) * multi); multi /= 100; });
                }
                return ret;
            }
        }
        /// <summary>
        /// Id des Polarion Plans (null wenn ab Level 4 nur mehr Workitems in Polarion vorhanden sind)
        /// </summary>
        public string PlanId { get; set; }

        /// <summary>
        /// Id des verknüpfeten oder ab Level 4 alleinigen Workpackages 
        /// </summary>
        public string WorkitemId { get; set; }

        /// <summary>
        /// Bezeichnung des Tasks
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id des übergeordneten Plans / Workitems
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Startdatum aus MS-Project
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Ende-Datum aus MS-Project
        /// </summary>
        public DateTime Finish { get; set; }

        /// <summary>
        /// MS-Project Id = Zeilenummer = SortOrder in Polarion
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Work Breakdown Structure = PSP Elementcode zB.: 1.3.1
        /// In Polarion wird der WBSCode am Beginn des names gestellt, getrennt mit 1 Blank
        /// </summary>
        public string WBSCode { get; set; }

        /// <summary>
        /// Dieser Task ist ein Milestone (Start und Endedatum gleich und Duration = 0)
        /// Plans die hinter dem WBSCode Mx haben sind Milestones (M0 ... M9)
        /// </summary>
        public bool Milestone { get; set; }

        /// <summary>
        /// Status aus Polarion Plan bzw. project task
        /// open | inprogress | closed
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// wird aus dem korrespondierenden Workpackage Feldern "Initial Estimate" und "Time Spent" berechnet in % ohne Komma
        /// </summary>
        public int PercentComplete { get; set; }

        /// <summary>
        /// Datum/Uhrzeit der letzten Änderung in Polarion
        /// </summary>
        public DateTime PolarionUpdate { get; set; }

        /// <summary>
        /// Level des Tasks: zB.: 1.3.4 = Level 3
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Custom Field "ptProcess" aus verknüpften ProjectTask - definiert den verwendeten Process
        /// </summary>
        public string PtProcess { get; set; }

        /// <summary>
        /// Custom Field "ptrole" aus verknüpften ProjectTask - 
        /// </summary>
        public string PtRole { get; set; }
    }
}