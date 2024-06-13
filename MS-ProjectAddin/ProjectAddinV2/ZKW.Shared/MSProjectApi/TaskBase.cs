//using PolarionReports.Models.Database.Api;
using System;

namespace ZKW.Shared.MSProjectApi
{
    public class TaskBase
    {
        /// <summary>
        /// Id des Polarion Plans (null wenn ab Level 4 nur mehr Wortkitems in Poalrion vorhanden sind)
        /// </summary>
        public string PlanId { get; set; }

        /// <summary>
        /// Id des verkn├╝pfeten Workitems oder ab Level 4 alleinigen Workitem
        /// </summary>
        public string WorkitemId { get; set; }

        /// <summary>
        /// Bezeichnung des Tasks
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id des ├╝bergeordneten Plans / Workitems
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Startdatum aus MS-Projekt
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Ende-Datum aus MS Project
        /// </summary>
        public DateTime Finish { get; set; }

        /// <summary>
        /// MS-Project Id = Zeilennummer 
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// wird aus dem korrespondierenden Workpackage Feldern "Initial Estimate" und "Time Spent" berechnet in % ohne Komma
        /// </summary>
        public int PercentComplete { get; set; }

        /// <summary>
        /// Datum/Uhrzeit der letzten ├änderung in Polarion
        /// </summary>
        public DateTime PolarionUpdate { get; set; }

        /// <summary>
        /// level des Tasks zB.: 1.3.4 = Level 3
        /// </summary>
        public int Level { get; set; }

        public string WBSCode { get; set; }

        public bool Milestone { get; set; }

        /// <summary>
        /// Init MS-Project Task with Polarion Plan
        /// </summary>
        /// <param name="pp">Polarion Plan</param>
        /// <param name="ParentId">c_id des parents (Achtung nicht c_pk)</param>
        //public Task(PlanApiDB pp, string ParentId)
        //{
        //    Id = pp.c_id;
        //    Name = pp.c_name;
        //    Start = pp.c_startdate;
        //    Finish = pp.c_duedate;
        //    PolarionUpdate = pp.c_updated;
        //    this.ParentId = ParentId;
        //}

        //public Task(PlanApiDB pp, PmWorkPackageDB wp, string ParentId)
        //{
        //    Id = pp.c_id;
        //    Name = pp.c_name;
        //    Start = pp.c_startdate;
        //    Finish = pp.c_duedate;
        //    PolarionUpdate = pp.c_updated;
        //    if (wp.c_initialestimate > 0 && wp.c_timespent > 0)
        //    {
        //        PercentComplete = Convert.ToInt32(wp.c_timespent / wp.c_initialestimate * 100);
        //    }
        //    this.ParentId = ParentId;
        //}

        /// <summary>
        /// Task aus einem Workpackage erzeugen
        /// </summary>
        /// <param name="wp">Workpackage</param>
        /// <param name="ParentId">c_id des Parents (pmWorkpackage)</param>
        //public Task(PmWorkPackageDB wp, string ParentId)
        //{
        //    Id = wp.c_id;
        //    Name = wp.c_title;
        //    Start = wp.c_plannedstart;
        //    Finish = wp.c_duedate;
        //    PolarionUpdate = wp.c_updated;
        //    if (wp.c_initialestimate > 0 && wp.c_timespent > 0)
        //    {
        //        PercentComplete = Convert.ToInt32(wp.c_timespent / wp.c_initialestimate * 100);
        //    }
        //    this.ParentId = ParentId;
        //}

        public void UpdatePolarion()
        {
            // Plan und pmWorkPackage einlesen:

            // Plan update
            //  Feldweise vergleichen und geänderte Werte updaten (Log erzeugen)

            // pmWorkPackage update
            //  Feldweise vergleichen und geänderte Werte updaten (Log erzeugen)
        }

        public void InsertPolarion()
        {

            // Plan anlegen

            // pmWorkPackage anlegen

            // pmWorkPackage mit Plan verbinden
        }
    }
}