using PolarionReports.BusinessLogic.Api;
using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.Gantt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic.Gantt
{
    /// <summary>
    /// Klasse für Erstellung von Daten für Syncfusion Gantt-Control
    /// </summary>
    public class GanttTaskReader
    {
        /// <summary>
        /// Liefert eine Liste von GanttData erstellt aus Polarion:
        ///  * Pläne
        ///  * damit verbundene Workpackages (planned in)
        ///  * damit verbundene Testruns (planned in)
        ///  * rekursiv im Plan-Tree
        /// </summary>
        /// <param name="dr">Datareader für Polarion Datenbank</param>
        /// <param name="projectId">Id des Polarion Projekts</param>
        /// <param name="baseplanId">Id des Basisplans: Beginn der Rekursion</param>
        /// <returns></returns>
        public List<GanttData> GetGanttdata(DatareaderP dr, string projectId, string baseplanId)
        {
            List<GanttData> gdl = new List<GanttData>();
            List<GanttWorkpackage> gwpl;
            List<WorkitemCustomField> wcfl;
            GanttData gd;
            PlanApiDB Baseplan;
            ProjectDB projectDB;

            Polarion polarion = new Polarion();
            PolarionInit pol = new PolarionInit();
            Connection con = pol.Init("wnpolarion", "pol%17WN");

            /// maximale derzeit vergebene GanttId über den gesamten Plan mit allen Unterplänen, Workpackages und Testruns
            int maxGanttId = 0; 

            projectDB = dr.GetProjectByID(projectId, out string error3);
            List<PlanApiDB> plans = dr.GetPlanForProject(projectId, out string error);
            SetMilestone(plans); // Milestone Flag setzten
            Baseplan = plans.FirstOrDefault(p => p.c_id == baseplanId); //  p.fk_parent == 0 &&

            if (Baseplan == null)
            {
                return gdl;
            }

            Baseplan.fk_parent = 0;
            List<PlanCustomField> pcfl = dr.GetAllPlanCustomFields(Baseplan.c_pk, out string error6);
            GanttData bt = new GanttData(Baseplan, pcfl);   //Basis Task erstellen
            bt.Sortorder = 1;
            bt.Level = 1;
            // maxGanttId = bt.SetGanttId(maxGanttId); // taskId initialisieren
            gdl.Add(bt);

            // Wortitems von Typ Workpackage einlesen und mit Plan verbinden
            gwpl = dr.GetWorkpackageForPlan(projectDB.C_pk, Baseplan.c_id, out string error4);
            foreach(GanttWorkpackage gwp in gwpl)
            {
                wcfl = dr.GetAllCustomFieldsFromWorkitem(gwp.c_id, out string error5);
                gwp.FillFromCustomFields(wcfl);
                gd = new GanttData(gwp, wcfl);
                gd.ParentId = Baseplan.c_pk;
                gd.Level = 1;
                gdl.Add(gd);
            }

            // Testruns einlesen und mit Plan verbinden @@@ToDo

            // Childs rekursiv einlesen:
            gdl.AddRange(GetChildPlans(dr, plans, Baseplan, 2, projectDB.C_pk));

            // maximale TaskId ermitteln
            foreach (GanttData item in gdl)
            {
                if (item.TaskId > maxGanttId) { maxGanttId = item.TaskId; }
            }

            // TaskId und Sortorder initialisieren
            maxGanttId = SetTaskId(gdl, maxGanttId);
            SetSortorder(gdl);

            // Verknüpfung von Plan_C_PK auf TaskId ändern 
            foreach (GanttData item in gdl)
            {
                if (item.ParentId > 0)
                {
                    GanttData g = gdl.FirstOrDefault(n => n.Plan_c_pk == item.ParentId);
                    item.ParentId = g.TaskId;
                }
            }

            // geänderte Custom Fields (ganttId und ganttSortorder) in Polarion speichern
            foreach (GanttData item in gdl)
            {
                if (item.TaskType == GanttData.TaskTypeEnum.Plan)
                {
                    bool updatePlan = false;
                    // Customfields [ganttId] und [ganttSortorder] speichern
                    PlanCustomField pcfganttId = item.PlanCustomFields.FirstOrDefault(c => c.CfName == "ganttId");
                    PlanCustomField pcfganttSortorder = item.PlanCustomFields.FirstOrDefault(c => c.CfName == "ganttSortorder");
                    if (pcfganttId != null && pcfganttId.CfValue == item.TaskId.ToString())
                    {
                        // ganttId bereits im CustomField
                    }
                    else
                    {
                        // ganttId geändert oder neu
                        updatePlan = true;
                    }

                    if (pcfganttSortorder != null && pcfganttSortorder.CfValue == item.Sortorder.ToString())
                    {
                        // ganttSortorder bereits im CustomField
                    }
                    else
                    {
                        // ganttSortorder geändert oder neu
                        updatePlan = true;
                    }

                    if (updatePlan)
                    {
                        polarion.UpdatePlanCustomFields(con, projectId, item.c_id, item.TaskId.ToString(), item.Sortorder.ToString(), item.Dependency);
                    }
                }
                else if (item.TaskType == GanttData.TaskTypeEnum.Workpackage)
                {
                    WorkitemCustomField wcfganttId = item.WorkitemCustomFields.FirstOrDefault(c => c.CfName == "ganttId");
                    WorkitemCustomField wcfganttSortorder = item.WorkitemCustomFields.FirstOrDefault(c => c.CfName == "ganttsortorder");

                    if (wcfganttId != null && wcfganttId.CfValue == item.TaskId.ToString())
                    {
                        // ganttId bereits im Custom Field
                    }
                    else
                    {
                        // ganttId in Workitem Custom Field speichern
                        polarion.CreateWorkitemCF(con, projectId, item.c_id, "ganttId", item.TaskId.ToString());
                    }

                    if (wcfganttSortorder != null && wcfganttSortorder.CfValue == item.Sortorder.ToString())
                    {
                        // ganttSortorder bereits im Custom Field
                    }
                    else
                    {
                        // ganttsortorder in Workitem Custom Field speichern
                        polarion.CreateWorkitemCF(con, projectId, item.c_id, "ganttsortorder", item.Sortorder.ToString());
                    }
                }
                else if (item.TaskType == GanttData.TaskTypeEnum.Testrun)
                {

                }

            }

            return gdl;

        }

        public List<GanttData> GetChildPlans(DatareaderP dr, List<PlanApiDB> PolarionPlans, PlanApiDB Baseplan, int level, int projectPK)
        {
            List<GanttData> ChildTasks = new List<GanttData>();
            List<PlanApiDB> PolarionChildPlans = PolarionPlans.FindAll(p => p.fk_parent == Baseplan.c_pk);
            List<GanttWorkpackage> gwpl;
            List<WorkitemCustomField> wcfl;
            GanttData gd;

            foreach (PlanApiDB pp in PolarionChildPlans)
            {
                List<PlanCustomField> pcfl = dr.GetAllPlanCustomFields(pp.c_pk, out string error6);
                gd = new GanttData(pp, pcfl);
                gd.Level = level;
                ChildTasks.Add(gd);
                
                // Wortitems von Typ Workpackage einlesen und mit Plan verbinden
                gwpl = dr.GetWorkpackageForPlan(projectPK, pp.c_id, out string error2);
                foreach (GanttWorkpackage gwp in gwpl)
                {
                    wcfl = dr.GetAllCustomFieldsFromWorkitem(gwp.c_id, out string error4);
                    gwp.FillFromCustomFields(wcfl);
                    gd = new GanttData(gwp, wcfl);
                    gd.ParentId = pp.c_pk;
                    gd.Level = level;
                    ChildTasks.Add(gd);
                }

                // Testruns einlesen und mit Plan verbinden @@@ToDo

                ChildTasks.AddRange(GetChildPlans(dr, PolarionPlans, pp, (level + 1), projectPK)); // keine Rekursion zum Test Zeile auskommentieren
            }

            // Childtask sortiert zurückgeben
            return ChildTasks;
        }

        public void SetMilestone(List<PlanApiDB> projectPlans)
        {
            PlanApiDB template;

            foreach(PlanApiDB p in projectPlans)
            {
                if (p.fk_template > 0)
                {
                    template = projectPlans.FirstOrDefault(n => n.c_pk == p.fk_template);
                    if (template != null)
                    {
                        if (template.c_id.ToUpper().Contains("MILESTONE"))
                        {
                            p.Milestone = true;
                        }
                        else
                        {
                            p.Milestone = false;
                        }
                    }
                }
            }
        }

        public void SetSortorder(List<GanttData> gdl)
        {
            List<GanttData> subgdl;

            gdl[0].Sortorder = 1;
            subgdl = gdl.FindAll(n => n.ParentId == gdl[0].Plan_c_pk).OrderBy(n => n.Sortorder).ToList();
            if (subgdl.Count == 0)
            {
                return;
            }
            SetSubSortorder(gdl, subgdl);
        }

        public void SetSubSortorder(List<GanttData> gdl, List<GanttData> subgdl)
        {
            int maxSortorder = subgdl.Max(n => n.Sortorder);

            foreach(GanttData gd in subgdl)
            {
                if (gd.Sortorder == 0) { gd.Sortorder = ++maxSortorder; }
            }

            foreach(GanttData gd in subgdl)
            {
                if (gd.Plan_c_pk > 0)
                {
                    List<GanttData> subgdl2 = gdl.FindAll(n => n.ParentId == gd.Plan_c_pk).OrderBy(n => n.Sortorder).ToList();
                    if (subgdl2.Count > 0)
                    {
                        SetSubSortorder(gdl, subgdl2);
                    }
                }
            }

            // return gdl.OrderBy(n => n.Sortorder).ToList();

            /* derzeit keine Rekursion
            foreach (GanttData gd in gdl)
            {
                if (gd.Plan_c_pk > 0)
                {
                    List<GanttData> subgd = gdl.FindAll(n => n.ParentId == gdl[0].TaskId);
                    SetSubSortorder(subgd);
                }
            }
            */
        }

        /// <summary>
        /// Ausgehend vom Wurzelplan (kein Parent) wird die TaskId rekursiv vergeben 
        /// </summary>
        /// <param name="gdl">Liste mit allen Gantt-Tasks</param>
        /// <param name="maxGanttId">bisher maximal vergebene ID über Pläne, Workpackes und Testruns</param>
        /// <returns></returns>
        private int SetTaskId(List<GanttData> gdl, int maxGanttId)
        {
            int max = maxGanttId;
            GanttData gd = gdl.FirstOrDefault(n => n.ParentId == 0);
            if (gd == null) { return max; }
            if (gd.TaskId == 0)
            {
                gd.TaskId = 1;
                if (max == 0) { max = 1; }
            }

            List<GanttData> sublist = gdl.FindAll(n => n.ParentId == gd.Plan_c_pk);
            max = SetSubTaskId(gdl, sublist, max);
            return max;
        }

        /// <summary>
        /// Rekursive Vergabe der TaskId
        /// </summary>
        /// <param name="gdl">Liste mit allen tasks</param>
        /// <param name="sublist">Liste mit Subtaks</param>
        /// <param name="maxGanttId">bisher maximal vergebene Taskid</param>
        /// <returns></returns>
        private int SetSubTaskId(List<GanttData> gdl, List<GanttData> sublist, int maxGanttId)
        {
            int max = maxGanttId;
            // taskId in aktueller Ebene vergeben
            foreach(GanttData item in sublist)
            {
                if (item.TaskId == 0)
                {
                    item.TaskId = ++max;
                    Debug.WriteLine(item.TaskId.ToString());
                }
            }

            // Nächste Ebene aufrufen
            foreach (GanttData item in sublist)
            {
                if (item.Plan_c_pk > 0)
                {
                    // Nur Pläne können rekursiv verkettet werden (bei Workpackages und Test
                    List<GanttData> sublist2 = gdl.FindAll(n => n.ParentId == item.Plan_c_pk);
                    if (sublist2.Count > 0)
                    {
                        max = SetSubTaskId(gdl, sublist2, max);
                    }
                }
            }

            return max;
        }
    }
}