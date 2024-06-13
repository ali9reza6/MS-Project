using PolarionReports.Models.Database.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using PolarionReports.Models.Database;

namespace PolarionReports.Models.Gantt
{
    /// <summary>
    /// Datenstruktur für Syncfusion Gantt-Control
    /// </summary>
    public class GanttData
    {
        public int TaskId { get; set; }
        public int? ParentId { get; set; }
        public string c_id { get; set; }
        public string TaskName { get; set; }
        public DateTime? BaselineStartDate { get; set; }
        public DateTime? BaselineEndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public int Progress { get; set; }
        public string Dependency { get; set; }
        public int Sortorder { get; set; }
        public int TaskIdSave { get; set; }
        public int Plan_c_pk { get; set; }
        public int Workpackage_c_pk { get; set; }
        public int Testrun_c_pk { get; set; }
        public int Plan_fk_parent { get; set; }
        public int Level { get; set; }
        public List<PlanCustomField> PlanCustomFields { get; set; }
        public List<WorkitemCustomField> WorkitemCustomFields { get; set; }
        public List<TestrunCustomField> TestrunCustomFields { get; set; }
        public enum TaskTypeEnum { Plan, Workpackage, Testrun}
        public TaskTypeEnum TaskType { get; set; } 

        public GanttData()
        {

        }

        public GanttData(PlanApiDB planApiDB, List<PlanCustomField> pcfl)
        {
            DateTime datehelper;

            this.TaskType = TaskTypeEnum.Plan;
            this.Plan_c_pk = planApiDB.c_pk; 
            this.ParentId = planApiDB.fk_parent;
            this.c_id = planApiDB.c_id;
            this.TaskName = planApiDB.c_name + " |P:" + planApiDB.c_pk.ToString();
            this.Sortorder = planApiDB.c_sortorder;

            this.PlanCustomFields = pcfl;

            foreach(PlanCustomField pcf in pcfl)
            {
                if (pcf.CfName == "ganttSortorder")    { this.Sortorder = Convert.ToInt32(pcf.CfValue); }
                if (pcf.CfName == "ganttDependencies") { this.Dependency = pcf.CfValue; }
                if (pcf.CfName == "ganttId")           { this.TaskId = Convert.ToInt32(pcf.CfValue); }
            }

            if (planApiDB.Milestone)
            {
                // Milestone
                //===========
                if (planApiDB.c_startdate.ToShortDateString() != "01.01.0001")
                {
                    this.StartDate = planApiDB.c_startdate;
                    this.EndDate = this.StartDate;
                    this.Duration = 0;
                }
                else
                {
                    if (planApiDB.c_duedate.ToShortDateString() == "01.01.0001")
                    {
                        if (planApiDB.c_startedon.ToShortDateString() == "01.01.0001")
                        {
                            if (planApiDB.c_finishedon.ToShortDateString() == "01.01.0001")
                            {
                                // keine Datums gesetzte
                                this.StartDate = DateTime.Today;
                                this.EndDate = this.StartDate;
                            }
                            else
                            {
                                this.StartDate = new DateTime(planApiDB.c_finishedon.Year, planApiDB.c_finishedon.Month, planApiDB.c_finishedon.Day);
                                this.EndDate = this.StartDate;
                            }
                        }
                        else
                        {
                            this.StartDate = new DateTime(planApiDB.c_startedon.Year, planApiDB.c_startedon.Month, planApiDB.c_startedon.Day);
                            this.EndDate = this.StartDate;
                        }
                    }
                    else
                    {
                        this.StartDate = planApiDB.c_duedate;
                        this.EndDate = this.StartDate;
                    }
                }

                return;
            }
            else
            {
                // relase oder iteration Plan
                //============================
                if (planApiDB.c_startdate.ToShortDateString() == "01.01.0001")
                {
                    // kein Start Datum
                    if (planApiDB.c_startedon.ToShortDateString() != "01.01.0001")
                    {
                        this.StartDate = new DateTime(planApiDB.c_startedon.Year, planApiDB.c_startedon.Month, planApiDB.c_startedon.Day);
                        this.BaselineStartDate = StartDate;
                    }
                    else
                    {
                        this.StartDate = DateTime.Today;
                    }
                }
                else
                {
                    if (planApiDB.c_startedon.ToShortDateString() != "01.01.0001")
                    {
                        this.BaselineStartDate = new DateTime(planApiDB.c_startdate.Year, planApiDB.c_startdate.Month, planApiDB.c_startdate.Day);
                        this.StartDate = new DateTime(planApiDB.c_startedon.Year, planApiDB.c_startedon.Month, planApiDB.c_startedon.Day);
                    }
                    else
                    {
                        this.StartDate = planApiDB.c_startdate;
                        this.BaselineStartDate = null;
                    }
                }

                if (planApiDB.c_finishedon.ToShortDateString() != "01.01.0001")
                {
                    // finished On eingetragen
                    this.EndDate = new DateTime(planApiDB.c_finishedon.Year, planApiDB.c_finishedon.Month, planApiDB.c_finishedon.Day);
                    if (planApiDB.c_duedate < this.StartDate)
                    {
                        this.BaselineEndDate = this.EndDate;
                    }
                    else
                    {
                        this.BaselineEndDate = new DateTime(planApiDB.c_duedate.Year, planApiDB.c_duedate.Month, planApiDB.c_duedate.Day);
                    }
                    if (this.BaselineStartDate == null)
                    {
                        this.BaselineStartDate = this.StartDate;
                    }
                }
                else
                {
                    if (planApiDB.c_duedate.ToShortDateString() == "01.01.0001")
                    {
                        // keine endeddatum vorhanden Start + 1 Tag
                        datehelper = StartDate.Value;
                        this.EndDate = datehelper.AddDays(1);
                        
                    }
                    else
                    {
                        this.EndDate = new DateTime(planApiDB.c_duedate.Year, planApiDB.c_duedate.Month, planApiDB.c_duedate.Day);
                        if (BaselineStartDate == null)
                        {
                            this.BaselineEndDate = null;
                        }
                        else
                        {
                            if (planApiDB.c_finishedon.ToShortDateString() == "01.01.0001")
                            {
                                this.BaselineEndDate = this.EndDate;
                            }
                            else
                            {
                                this.BaselineEndDate = new DateTime(planApiDB.c_finishedon.Year, planApiDB.c_finishedon.Month, planApiDB.c_finishedon.Day);
                            }
                        }
                    }
                }
            }
            this.Progress = 0;
            this.Dependency = null;
            this.Duration = null;
        }

        public GanttData(GanttWorkpackage gwp, List<WorkitemCustomField> wcfl)
        {
            // string[] idparts = gwp.c_id.Split('-');
            // if (Int32.TryParse(idparts[1], out int idnumber)) this.TaskId = idnumber; // 2. Teil der Id

            this.TaskType = TaskTypeEnum.Workpackage;
            this.TaskId = gwp.TaskId;
            this.c_id = gwp.c_id;
            this.TaskName = gwp.c_title + " |W:" + gwp.c_pk;
            this.BaselineStartDate = gwp.StartDate;
            this.BaselineEndDate = gwp.DueDate;
            this.StartDate = gwp.StartedOn;
            this.EndDate = gwp.FinishedOn;
            this.Duration = null; //gwp.c_initialestimate;
            this.Progress = gwp.c_timespent;
            this.Dependency = gwp.Dependency;
            this.Sortorder = gwp.Sortorder;
            this.TaskIdSave = gwp.c_pk;
            this.Workpackage_c_pk = gwp.c_pk;

            this.WorkitemCustomFields = wcfl;

            // Standardwerte für Datum setzen:
            // baseline nur wenn StartDate versorgt = task hat begonnen
            if (this.StartDate != null)
            {
                // StartedOn beinhaltet echtes Startdatum des Workpackeges
                if (this.EndDate == null)
                {
                    // noch kein Endedatum(FinishedOn) in Polarion eingetragen
                    if (this.StartDate > gwp.DueDate)
                    {
                        // StartedOn > dueDate - Start hinter geplantem Fertigstellungstermin
                        this.EndDate = StartDate.Value.AddDays(1);
                    }
                    else
                    {
                        this.EndDate = gwp.DueDate;
                    }
                }
            }
            else
            {
                if (this.BaselineEndDate == null)
                {
                    // keine Endedatum eingetragen -> Heute + 1 Tag als Endedatum
                    this.BaselineEndDate = DateTime.Today.AddDays(1);
                }
                if (this.BaselineStartDate == null) this.BaselineStartDate = DateTime.Today;
                this.StartDate = this.BaselineStartDate;
                this.EndDate = this.BaselineEndDate;
                this.BaselineStartDate = null;
                this.BaselineEndDate = null;
            }
        }

        /// <summary>
        /// Wenn keine TaskId gesetzt -> TaskId = maxGanttId + 1
        /// </summary>
        /// <param name="maxGanttId"></param>
        /// <returns></returns>
        public int SetGanttId(int maxGanttId)
        {
            if (this.TaskId == 0)
            {
                this.TaskId = ++maxGanttId;
                return this.TaskId;
            }

            return maxGanttId;
        }

        /// <summary>
        /// Setzt den TaskType aus den Infos im TaskName (" |P:" oder " |W:" oder " |T:")
        /// Das Syncfusion Gantt-Control gibt die Custum Fields nichtz zurück.
        /// Deshalb wird der Primary Key und der Typ im TaskName zwischengespeichet 
        /// </summary>
        public void SetTaskTypeFromTaskName()
        {
            if (this.TaskName.Contains(" |P:"))
            {
                this.TaskType = TaskTypeEnum.Plan;
            }
            else if (this.TaskName.Contains(" |W:"))
            {
                this.TaskType = TaskTypeEnum.Workpackage;
            }
            else if (this.TaskName.Contains(" |T:"))
            {
                this.TaskType = TaskTypeEnum.Testrun;
            }
        }

        /// <summary>
        /// Liefert den PrimaryKey aus dem TaskName (" |P:" oder " |W:" oder " |T:")
        /// </summary>
        /// <returns></returns>
        public int GetPKFromTaskname()
        {
            int pk = 0;

            if (this.TaskName.Contains(" |P:"))
            {
                string[] parts = this.TaskName.Split(':');
                int.TryParse(parts[parts.Count() - 1], out pk);
            }
            else if (this.TaskName.Contains(" |W:"))
            {
                string[] parts = this.TaskName.Split(':');
                int.TryParse(parts[parts.Count() - 1], out pk);
            }
            else if (this.TaskName.Contains(" |T:"))
            {
                string[] parts = this.TaskName.Split(':');
                int.TryParse(parts[parts.Count() - 1], out pk);
            }

            return pk;
        }
    }
}