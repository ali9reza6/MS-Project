using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syncfusion.EJ2.Navigations;
using PolarionReports.Models.Gantt;
using PolarionReports.Models.PlanTreeSort;
using PolarionReports.BusinessLogic.Api;
using System.Diagnostics;
using PolarionReports.BusinessLogic.Gantt;

namespace PolarionReports.BusinessLogic
{
    public class PlanBL
    {
        /// <summary>
        /// Init PlanViewModel:
        /// </summary>
        /// <param name="pv"></param>
        public void InitPv(PlanViewModel pv)
        {
            DatareaderP dr;
            string ErrorMsg = "";
            string Error;

            dr = new DatareaderP();
            if (pv.ProjectPK == 0)
            {
                pv.Project = dr.GetProjectByID(pv.ProjectId, out Error);
                ErrorMsg += Error;
                pv.ProjectPK = pv.Project.C_pk;
            }
            else
            {
                pv.Project = dr.GetProjectByPK(pv.ProjectPK, out Error);
                ErrorMsg += Error;
            }

            pv.ProjectPlans = dr.GetAllPlansFromProject(pv.ProjectPK, out Error);
            ErrorMsg += Error;
            pv.Template = dr.GetProjectByID("Template", out Error);
            ErrorMsg += Error;
            pv.TemplatePlans = dr.GetAllPlansFromProject(pv.Template.C_pk, out Error);
            ErrorMsg += Error;
            

            PlanTreeviewModel PlanTree = new PlanTreeviewModel();
            pv.PlanFields = new TreeViewFieldsSettings();
            pv.PlanFields.DataSource = PlanTree.GetPlanTreeviewModel(pv.ProjectPlans, 0, true, pv.ProjectPlans);
            pv.PlanFields.HasChildren = "HasChild";
            pv.PlanFields.Expanded = "Expanded";
            pv.PlanFields.Id = "PK";
            pv.PlanFields.ParentID = "PId";
            pv.PlanFields.Text = "Name";
            pv.PlanFields.ImageUrl = "Iconurl";


            PlanTreeviewModel TemplateTree = new PlanTreeviewModel();
            pv.TempFields = new TreeViewFieldsSettings();
            pv.TempFields.DataSource = TemplateTree.GetPlanTreeviewModel(pv.TemplatePlans, 0, true, pv.TemplatePlans);
            pv.TempFields.HasChildren = "HasChild";
            pv.TempFields.Expanded = "Expanded";
            pv.TempFields.Id = "PK";
            pv.TempFields.ParentID = "PId";
            pv.TempFields.Text = "Name";
            pv.TempFields.ImageUrl = "Iconurl";

        }

        public List<GanttData> SetSortorder(List<GanttData> gdl)
        {
            bool noSortorder = false;
            bool sortorderSet = false;
            int maxsortorder = 0;

            foreach (GanttData gd in gdl)
            {
                // für den TreeView muss die ParentId von 0 auf null gesetzte werden
                if (gd.ParentId == 0)
                {
                    gd.ParentId = null;
                }

                if (gd.Sortorder == 0)
                {
                    noSortorder = true;
                }
                else
                {
                    sortorderSet = true;
                }
            }

            if (noSortorder && !sortorderSet)
            {
                // kein Plan hat eine Sortorder => nach Startdatum sortieren und Sortorder neu vergeben und in Polarion speichern
                return gdl;
                /*
                gdl.Sort((p1, p2) => DateTime.Compare(p1.StartDate.Value, p2.StartDate.Value));
                maxsortorder = 0;
                foreach (GanttData gd in gdl)
                {
                    gd.Sortorder = ++maxsortorder;
                }
                // Polarion Update
                */
            }
            else if (sortorderSet && !noSortorder)
            {
                // alle Pläne haben eine Sortorder => kein Update in Polarion notwendig
                gdl = gdl.OrderBy(x => x.Sortorder).ToList();
                return gdl;
            }
            else
            {
                // manche Pläne haben keine Sortorder => Pläne ohne Sortorder ans Ende
                List<GanttData> gdl2 = gdl.FindAll(x => x.Sortorder == 0);
                gdl2.Sort((p1, p2) => DateTime.Compare(p1.StartDate.Value, p2.StartDate.Value));
                maxsortorder = gdl.Max(x => x.Sortorder);
                foreach (GanttData gd in gdl2)
                {
                    gd.Sortorder = ++maxsortorder;
                }
                return gdl;
            }
        }

        public bool SaveSortorder(TreeSortModel tsm)
        {
            PolarionInit pol = new PolarionInit();
            Connection con = pol.Init(tsm.Username, tsm.Password);
            Polarion p = new Polarion();
            DatareaderP dr = new DatareaderP();
            List<ChangedSortorder> csl = new List<ChangedSortorder>();
            ChangedSortorder cs;
            Dependencies dep = new Dependencies();

            // Alle Pläne einlesen um nur geänderte Daten in Polarion zu bearbeiten
            ProjectDB projectDB = dr.GetProjectByID(tsm.ProjectId, out string error1);
            PlanList pl = dr.GetAllPlansFromProject(projectDB.C_pk, out string error2);

            int i = 1;

            foreach(TreeSort ts in tsm.TreesortList)
            {
                Plan plan = pl.Plans.FirstOrDefault(x => x.Plandb.Id == ts.Id);
                if (plan != null)
                {
                    if (plan.Plandb.c_sortorder != i)
                    {
                        cs = new ChangedSortorder();
                        cs.OldValue = plan.Plandb.c_sortorder;
                        cs.NewValue = i;
                        cs.C_Pk = plan.Plandb.PK;
                        cs.PlanId = plan.Plandb.Id;
                        csl.Add(cs);
                        p.UpdateSinglePlanSortorder(con, tsm.ProjectId, ts, i);
                    }
                    i++;
                }
            }

            if (csl.Count > 0)
            {
                // Update Plan dependencies 
                foreach (ChangedSortorder item in csl)
                {
                    string plandependency = dr.GetPlanDependencies(item.C_Pk, out string errorGetPlanDependencies);
                    if (plandependency.Length > 0)
                    {
                        string newPlandepencency = dep.UpdateLinks(plandependency, csl);
                        if (plandependency != newPlandepencency)
                        {
                            // Update:
                            p.UpdateSinglePlanDependency(con, tsm.ProjectId, item.PlanId, newPlandepencency);
                        }
                    }
                }

                // Update Workitem dependencies
                List<WorkitemCustomField> wcfl = dr.GetWorkitemDependencies(projectDB.C_pk, out string errorGetWorkitemDependencies);
                foreach (WorkitemCustomField wcf in wcfl)
                {
                    string newWorkitemdepenency = dep.UpdateLinks(wcf.CfValue, csl);
                    if (wcf.CfValue != newWorkitemdepenency)
                    {
                        // Update workitem CF
                        p.CreateWorkitemCF(con, projectDB.Id, wcf.WorkitemId, "dependencies", newWorkitemdepenency);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Die geänderten Daten weden in den Plänen und Workitems gespeichert
        /// </summary>
        /// <param name="gdl"></param>
        /// <returns></returns>
        public bool UpdatePlanData(string projectId, List<GanttData> gdl, string username, string password)
        {
            DatareaderP dr = new DatareaderP();
            PolarionInit pol = new PolarionInit();
            Connection con = pol.Init(username, password);
           
            Polarion p = new Polarion();

            ProjectDB projectDB = dr.GetProjectByID(projectId, out string error1);

            foreach (GanttData gd in gdl)
            {
                // Korrepondierende Polarion Objekte suchen Plan|Workpackage|Testrun wird
                gd.SetTaskTypeFromTaskName();

                // gd.TaskId beinhaltet die Sortorder PlanId aus Datenbank ??
                if (gd.TaskType == GanttData.TaskTypeEnum.Workpackage)
                {
                    // Dieser Task ist ein Workitem
                    Workitem w = dr.GetWorkitem(gd.GetPKFromTaskname(), out string error2);
                    p.UpdateWorkitemFromGantt(con, projectId, w.Id, gd);
                }
                else if (gd.TaskType == GanttData.TaskTypeEnum.Plan)

                {
                    // Dieser Task ist ein Plan
                    Debug.WriteLine(gd.TaskId.ToString() + " " + gd.TaskName);
                    PlanDB plan = dr.GetPlanByPK(gd.GetPKFromTaskname(), out string error3);
                    p.UpdatePlanFromGantt(con, projectId, plan.Id, gd);                    
                }
                else if (gd.TaskType == GanttData.TaskTypeEnum.Testrun)
                {
                    // Dieser task ist ein Testrun
                }
            }

            dr.CloseConnection();
            
            return true;
        }
    }
}