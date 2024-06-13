using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Gantt
{
    /// <summary>
    /// Klasse für alle Daten eines Workitems vom Typ "Workpackage" und Gantt-Spezifische Custom-Fields
    /// </summary>
    public class GanttWorkpackage
    {
        /// <summary>
        /// Primary Key (Datenbank) des Workpackage
        /// </summary>
        public int c_pk { get; set; }
        /// <summary>
        /// Id des Workpackages
        /// </summary>
        public string c_id { get; set; }
        public string c_title { get; set; }
        public string c_status { get; set; }
        /// <summary>
        /// Geplantes Startdatum aus Custom-Field [startDate]
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Geplante fertigstellung aus Workitem
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Tatsächliches Startdatum aus Custom-Field [startedOn]
        /// </summary>
        public DateTime? StartedOn { get; set; }
        /// <summary>
        /// Tatsächliche Fertigstellung aus Custom-Fields [finishedOn]
        /// </summary>
        public DateTime? FinishedOn { get; set; }
        public int c_initialestimate { get; set; }
        public int c_timespent { get; set; }

        /// <summary>
        /// Eindeutige ID für Gantt-Control [ganttId]
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// Reihenfolge der Workpackages pro Plan (plannedin)
        /// </summary>
        public int Sortorder { get; set; }
        /// <summary>
        /// Dependency aus Syncfusion Gantt-Control
        /// </summary>
        public string Dependency { get; set; }

        /// <summary>
        /// Workpackage aus Custom-Fields vervollständigen
        /// </summary>
        /// <param name="wcfl"></param>
        public void FillFromCustomFields(List<WorkitemCustomField> wcfl)
        {
            DateTime Datehelper;

            foreach(WorkitemCustomField wcf in wcfl)
            {
                switch (wcf.CfName)
                {
                    case "startedOn":
                        if (DateTime.TryParse(wcf.CfValue, out Datehelper)) this.StartedOn = Datehelper;
                        break;

                    case "startDate":
                        if (DateTime.TryParse(wcf.CfValue, out Datehelper)) this.StartDate = Datehelper;
                        break;

                    case "finishedOn":
                        if (DateTime.TryParse(wcf.CfValue, out Datehelper)) this.FinishedOn = Datehelper;
                        break;

                    case "dependencies":
                        this.Dependency = wcf.CfValue;
                        break;

                    case "ganttDependencies":
                        this.Dependency = wcf.CfValue;
                        break;

                    case "ganttSortorder":
                        this.Sortorder = Convert.ToInt32(wcf.CfValue);
                        break;

                    case "ganttId":
                        this.TaskId = Convert.ToInt32(wcf.CfValue);
                        break;

                    default:

                        break;
                }
            }
        }
    }
}