using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic.Api
{
    public class TaskReader
    {
        public List<Task> GetPMTasks(DatareaderP dr, List<PlanApiDB> PolarionPlans, string basePlanId)
        {
            //string BasePlanId = "PM_Plan_Template_E"; // "Project_Plan";
            PlanApiDB Baseplan;
            List<PmWorkPackageDB> WPs;
            List<Task> Tasks = new List<Task>();
            //Task task;


            // Search rootplan (fk_parent = null, c_id = "Project_Plan")
            // => Search for all plans linked below
            Baseplan = PolarionPlans.FirstOrDefault(p => p.fk_parent == 0 && p.c_id == basePlanId); //Here giving BMW base plan why??

            if (Baseplan == null)
            {
                Debug.WriteLine("Error: at TaskReader.cs , BasePlan not found");
                // Fehler Basisplan nicht gefunden
                return Tasks;
            }
            else
            {
                WPs = dr.GetPmWorkPackageForPlan(Baseplan.c_pk, out string Error);
                if (WPs.Count == 1)
                {
                    // Alles OK es muss genau ein Project Work Package geben!
                    // Relevante Daten: Initial Estimate, Time Spent, Status, ? eventuell Assignees
                    var task = AddTaskEbene1(dr, Baseplan, WPs[0]);
                    task.ErrorMsg = "";// "Mehr als 1 Project-Task mit diesem Plan verknüft!";
                    Tasks.Add(task);
                }
                else
                {
                    // der Basis Plan muss genau ein Workpackage haben => Fehler
                    Debug.WriteLine("kein Project-Task bei Basis Plan");
                    Task task = new Task(Baseplan, "");
                    task.Level = 1;
                    task.ErrorMsg = "kein Project-Task verbunden!";
                    Tasks.Add(task);
                }
            }

            // Search plans recursively:
            Tasks.AddRange(GetChildPlans(dr, PolarionPlans, Baseplan, 2, Polarion.MaxDeepPlan)); //Maximum depth where PlanItems can still be created, including only workitems!

            // Sort Task

            return Tasks.OrderBy(x => x.WbsSortOrder).ToList(); // Where(x => x.WBSCode != "")
        }

        private static Task AddTaskEbene1(DatareaderP dr, PlanApiDB Baseplan, PmWorkPackageDB wp)
        {
            WorkitemCustomField wcf;
            List<WorkitemCustomField> wcfl;
            Task task = new Task(Baseplan, wp, "");
            // CustomFields des projectTask einlesen: ptProcess, ptrole
            wcfl = dr.GetAllCustomFieldsFromWorkitem(wp.c_id, out string Error2);
            wcf = wcfl.FirstOrDefault(c => c.CfName == "wpProcess");
            if (wcf != null) task.PtProcess = wcf.CfValue;
            wcf = wcfl.FirstOrDefault(c => c.CfName == "wprole");
            if (wcf != null) task.PtRole = wcf.CfValue;
            task.Level = 1;
            return task;
        }

        public List<Task> GetChildPlans(DatareaderP dr, List<PlanApiDB> PolarionPlans, PlanApiDB Baseplan, int level, int maxDeepPlan)
        {
            List<PmWorkPackageDB> WPs;
            Task task;
            List<Task> ChildTasks = new List<Task>();
            List<PlanApiDB> PolarionChildPlans = PolarionPlans.FindAll(p => p.fk_parent == Baseplan.c_pk);
            
            //All subplans from the base plan (only plans)
            foreach (PlanApiDB pp in PolarionChildPlans)
            {
                // Read the Work Packages for each plan:
                WPs = dr.GetPmWorkPackageForPlan(pp.c_pk, out string Error);

               
                string wbsPlan = Task.GetWBSCodeFromName(pp.c_name);  //WBS relevant?
                                                                      //Except if we are just reading from Polarion
                //WPs => Polarion Database Work Pachages <List>
                if (WPs.Count == 0) // A subplan exists without a Work Package... a child task is then added 
                {
                    // kein Workpackage => Fehler aber Abbildung möglich: MS-Project task nur mit Plan erstellen
                    task = new Task(pp, Baseplan.c_id);
                    task.Level = level;
                    task.ErrorMsg = "kein Project-Task verbunden!";
                    ChildTasks.Add(task);
                }
                else
                {
                    // AH: Handling new logic with variable number of plan levels
                    //if (level < maxDeepPlan)
                    {
                        Task planWp = null;
                        foreach (var wp in WPs)
                        {   
                            // first run is the workpackage from plan
                            if (Task.GetWBSCodeFromName(wp.c_title) == wbsPlan) //WorkPackage from the plan
                            {   
                                task = new Task(pp, wp, Baseplan.c_id);
                                task.Level = level;
                                FillCustomFileds(dr, task, wp); //Check for possible errors
                                ChildTasks.Add(task);
                                planWp = task;
                                if (level >= maxDeepPlan)
                                {
                                    // Ab Ebene > maxDeepPlan überprüfen, ob an dem WP weitgere WP,s verknüpft sind: 
                                    //Adds Sub WPgs which are not plans
                                    AddSubWorkPackages(dr, level + 1, ChildTasks, wp); 
                                }
                            }
                        }
                        //foreach (var wp in WPs)
                        //{   // 2. Durchlauf: untergeordnete WorkPackages
                        //    if (Task.GetWBSCodeFromName(wp.c_title) == wbsPlan)
                        //    {   // workPackage vom Plan
                        //        continue;
                        //    }
                        //    else
                        //    {   // einzelnes workPackage
                        //        task = new Task(wp, Baseplan.c_id);
                        //        task.Level = level + 1;
                        //        task.ParentId = planWp?.WorkitemId;
                        //    }

                        //    // CustomFields des projectTask einlesen: ptProcess, ptrole
                        //    wcfl = dr.GetAllCustomFieldsFromWorkitem(wp.c_id, out string Error2);
                        //    wcf = wcfl.FirstOrDefault(c => c.CfName == "ptProcess");
                        //    if (wcf != null) task.PtProcess = wcf.CfValue;
                        //    wcf = wcfl.FirstOrDefault(c => c.CfName == "ptrole");
                        //    if (wcf != null) task.PtRole = wcf.CfValue;


                        //    ChildTasks.Add(task);

                        //    if (level >= maxDeepPlan)
                        //    {
                        //        // Ab Ebene > maxDeepPlan überprüfen, ob an dem WP weitgere WP,s verknüpft sind: 
                        //        SubWPs = dr.GetPmWorkPackageForWP(wp.c_id, out string ErrorSubWPs);
                        //        foreach (PmWorkPackageDB swp in SubWPs)
                        //        {
                        //            task = new Task(swp, wp.c_id);
                        //            task.Level = level + 2;
                        //            ChildTasks.RemoveAll(x => x.WorkitemId == task.WorkitemId);
                        //            ChildTasks.Add(task);
                        //        }
                        //    }
                        //}

                    }
                    //else
                    //{
                    //    // Fehler: Ab der Ebene 5 werden weitere pmWorkPackages nicht mit dem Plan verknüpft, sondern mit dem
                    //    // pmWorkPackage welches mit dem darüber liegenden Plan verknüpft ist
                    //    task = new Task(pp, WPs[0], Baseplan.c_id);
                    //    task.Level = level;
                    //    // task.ErrorMsg = "Mehr als 1 Project-Task mit diesem Plan verknüft!";
                    //    ChildTasks.Add(task);
                    //}
                }

                //ChildTasks.AddRange(GetChildPlans(dr, PolarionPlans, pp, (level + 1), maxDeepPlan));
            }

            return ChildTasks;
        }

        /// <summary>
        /// Rekursive Funktion welche alle Subworkpackages eines Workpackage hinzufügt
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="level"></param>
        /// <param name="ChildTasks"></param>
        /// <param name="wp"></param>
        private static void AddSubWorkPackages(DatareaderP dr, int level, List<Task> ChildTasks, PmWorkPackageDB wp)
        {
            List<PmWorkPackageDB> SubWPs = dr.GetPmWorkPackageForWP(wp.c_id, out string ErrorSubWPs);
            foreach (PmWorkPackageDB swp in SubWPs)
            {
                var task = new Task(swp, wp.c_id);
                FillCustomFileds(dr, task, swp);
                task.Level = level;
                ChildTasks.RemoveAll(x => x.WorkitemId == task.WorkitemId); 
                ChildTasks.Add(task);
                AddSubWorkPackages(dr, level + 1, ChildTasks, swp);
            }
        }

        private static void FillCustomFileds(DatareaderP dr, Task task, PmWorkPackageDB wp)
        {
            // CustomFields des projectTask einlesen: ptProcess, ptrole
            var wcfl = dr.GetAllCustomFieldsFromWorkitem(wp.c_id, out string Error2);
            var wcf = wcfl.FirstOrDefault(c => c.CfName == "wpProcess");
            if (wcf != null) task.PtProcess = wcf.CfValue;
            wcf = wcfl.FirstOrDefault(c => c.CfName == "wprole");
            if (wcf != null) task.PtRole = wcf.CfValue;
        }
    }
}
