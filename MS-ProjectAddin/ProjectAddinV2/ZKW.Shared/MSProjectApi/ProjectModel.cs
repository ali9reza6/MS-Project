//using PolarionReports.Models.Database;
//using PolarionReports.Models.Database.Api;
using System.Collections.Generic;

namespace ZKW.Shared.MSProjectApi
{
    public class ProjectModel
    {
        /// <summary>
        /// Eindeutige Projekt ID = Kurzname des Projekt zB.: E18008
        /// </summary>
        public string ProjectID { get; set; }

        public List<TaskBase> Tasks { get; set; }

        //public List<Task> GetPMTasks(DatareaderP dr, List<PlanApiDB> PolarionPlans)
        //{
        //    string BasePlanId = "Project_Plan";
        //    PlanApiDB Baseplan;
        //    List<PmWorkPackageDB> WPs;
        //    PmWorkPackageDB BaseWP;
        //    List<Task> Tasks = new List<Task>();
        //    Task task;

        //    // Wurzelplan suchen (fk_parent = null, c_id = "Project_Plan"
        //    // => Alle darunter verpnüpften Pläne suchen
        //    Baseplan = PolarionPlans.FirstOrDefault(p => p.fk_parent == 0 && p.c_id == BasePlanId);

        //    if (Baseplan == null)
        //    {
        //        // Fehler Basisplan nicht gefunden
        //        return Tasks;
        //    }
        //    else
        //    {
        //        WPs = dr.GetPmWorkPackageForPlan(Baseplan.c_pk, out string Error);
        //        if (WPs.Count == 1)
        //        {
        //            // Alles OK es muss genau ein Project Work Package geben!
        //            // Relevante Daten: Initial Estimate, Time Spent, Status, ? eventuell Assignees
        //            task = new Task(Baseplan, WPs[0], "");
        //            task.Level = 1;
        //            Tasks.Add(task);
        //        }
        //        else
        //        {
        //            if (WPs.Count > 1)
        //            {
        //                // Der Basis Plan darf nur ein Workpackage haben => Fehler
        //                task = new Task(Baseplan, WPs[0], "");
        //                task.ErrorMsg = "Mehr als 1 pmWorkpackage mit diesem Plan verknüft!";
        //                task.Level = 1;
        //                Tasks.Add(task);
        //                Debug.WriteLine(WPs.Count.ToString() + " WPs bei Basisplan");
        //            }
        //            else
        //            {
        //                // der Basis Plan muss genau ein Workpackage haben => Fehler
        //                Debug.WriteLine("kein WP bei Basis Plan");
        //                task = new Task(Baseplan, "");
        //                task.Level = 1;
        //                task.ErrorMsg = "kein pmWorkpackage verbunden!";
        //                Tasks.Add(task);
        //            }
        //        }
        //    }

        //    // Pläne rekursiv suchen:
        //    Tasks.AddRange(GetChildPlans(dr, PolarionPlans, Baseplan, 2));

        //    return Tasks;
        //}

        //public List<Task> GetChildPlans(DatareaderP dr, List<PlanApiDB> PolarionPlans, PlanApiDB Baseplan, int level)
        //{
        //    List<PmWorkPackageDB> WPs;
        //    List<PmWorkPackageDB> SubWPs;
        //    PmWorkPackageDB WP;
        //    Task task;
        //    List<Task> ChildTasks = new List<Task>();
        //    List<PlanApiDB> PolarionChildPlans = PolarionPlans.FindAll(p => p.fk_parent == Baseplan.c_pk);
        //    foreach(PlanApiDB pp in PolarionChildPlans)
        //    {
        //        // Workpackage einlesen:
        //        WPs = dr.GetPmWorkPackageForPlan(pp.c_pk, out string Error);
        //        if (WPs.Count == 0)
        //        {
        //            // kein Workpackage => Fehler aber Abbildung möglich: MS-Project task nur mit Plan erstellen
        //            task = new Task(pp, Baseplan.c_id);
        //            task.Level = level;
        //            task.ErrorMsg = "kein pmWorkpackage verbunden!";
        //            ChildTasks.Add(task);
        //        }
        //        else
        //        {
        //            if (WPs.Count == 1)
        //            {
        //                // genau ein WP tasks bis Ebene 3
        //                task = new Task(pp, WPs[0], Baseplan.c_id);
        //                task.Level = level;
        //                ChildTasks.Add(task);
                        
        //                if (level >= 3)
        //                {
        //                    // Ab Ebene 3 überprüfen, ob an dem WP weitgere WP,s verknüpft sind: 
        //                    SubWPs = dr.GetPmWorkPackageForWP(WPs[0].c_id, out string ErrorSubWPs);
        //                    foreach(PmWorkPackageDB wp in SubWPs)
        //                    {
        //                        task = new Task(wp, WPs[0].c_id);
        //                        task.Level = level + 1;
        //                        ChildTasks.Add(task);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Fehler: Ab der Ebene 4 werden weitere pmWorkPackages nicht mit dem Plan verknüpft, sondern mit dem
        //                // pmWorkPackage welches mit dem darüber liegenden Plan verknüpft ist
        //                task = new Task(pp, WPs[0], Baseplan.c_id);
        //                task.Level = level;
        //                task.ErrorMsg = "Mehr als 1 pmWorkpackage mit diesem Plan verknüft!";
        //                ChildTasks.Add(task);
        //            }
        //        }

        //        ChildTasks.AddRange(GetChildPlans(dr, PolarionPlans, pp, (level + 1)));
        //    }

        //    return ChildTasks;
        //}

    }
}