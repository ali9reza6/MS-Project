using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serilog;

namespace PolarionReports.BusinessLogic.Api
{
    public class InsertTask
    {
        /// <summary>
        /// Fügt einen Task aus MS-Project in Polarion ein:
        /// Variante 1: bis Level 4 gibt es Plan und korrespondierendes projectTask Workitem
        /// Diese variante gibt es nicht mehr! Variante 2: ab Lebel 5 gibt es nur mehr das projectTask Workitem
        /// In den Workitems ist PlannedIn immer der korrespondierende Plan, Ab Level 5 der 
        /// Plan der im Level 4 definiert ist. Die Workitems sind ebenfalls Hierarchisch verbunden
        /// es werden die Rolle "parent" verwendet die in Polarion als "has parent" und "is parent" angezeigt wird.
        /// </summary>
        /// <param name="MSP"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool Insert(ProjectModel MSP, out ApiError Error)
        {
            bool insertbasePlan = false;
            Error = new ApiError();
            DatareaderP dr = new DatareaderP();
            Polarion po = new Polarion();
            PolarionInit pi = new PolarionInit();

            PlanApiDB ParentPlan;
            PmWorkPackageDB ParentWorkitem;
            List<PmWorkPackageDB> ParentWorkitems;

            if (MSP.Tasks.Count != 1)
            {
                // ungültige Aufruf:
                Log.Error("Error Insert, Insert: only one Task permitted (number of tasks sumbittet: " + MSP.Tasks.Count.ToString() + " tasks)");
                Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Error.Message = "Insert: only one Task permitted (number of tasks sumbittet: " + MSP.Tasks.Count.ToString() + " tasks)";
                return false;
            }

            Task task = MSP.Tasks[0];   // Beim Insert ist genau ein task in der Liste

            // The new WBSCode must not be in any existing plan or work item:
            //2019-10-31 WBS code can be duplicated because an inserted task can have the same code
            //The subsequent update changes the previous ones

            // Untersuchen, ob der parent Ok ist
            if (!(string.IsNullOrEmpty(task.PlanId) || string.IsNullOrEmpty(task.WorkitemId))) 
            {                                                                                  
                // ungültige Aufruf:
                Log.Error("Error Insert, PlanId or WorkitemId not null");
                Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Error.Message = "PlanId or WorkitemId not null";
                return false;
            }

            // parentId löschen - stimmt nich beim einfügen
            
            task.ParentId = "";
            ParentPlan = GetParentPlan(dr, MSP.ProjectID, task, out string errorGetParentPlan);

            if (errorGetParentPlan != "")
            {
                // Datenbankfehler
                Log.Error("Error Insert, Datenbankfehler: " + errorGetParentPlan);
                Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                Error.Message = errorGetParentPlan;
                dr.CloseConnection();
                return false;
            }
            if (ParentPlan == null)
            {
                // kein Wurzelplan gefunden: prüfen, ob WBS-Code 1 stellig
                if (task.WBSCode.Length == 1)
                {
                    // Wurzelplan soll eingefügt werden
                    Log.Debug("Insert Baseplan");
                    insertbasePlan = true;
                }
                else
                {
                    // ungültige Aufruf:
                    Log.Error("Error Insert, ParantId not Found in Polarion: " + task.ParentId);
                    Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    Error.Message = "ParantId not Found in Polarion: " + task.ParentId;
                    dr.CloseConnection();
                    return false;
                }
            }

            if (!insertbasePlan)
            {
                task.ParentPlanId = ParentPlan.c_id;    // parent Plan in neuen task eintragen

                // ParentProjectTask Workitem suchen
                ParentWorkitems = dr.GetPmWorkPackageForPlan(ParentPlan.c_pk, out string ErrorGetPmWorkPackageForPlan);
                if (ErrorGetPmWorkPackageForPlan != "")
                {
                    // Datenbankfehler
                    Log.Error("Error Insert, Database: " + ErrorGetPmWorkPackageForPlan);
                    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Error.Message = ErrorGetPmWorkPackageForPlan;
                    return false;
                }
                if (ParentWorkitems.Count == 0)
                {
                    Log.Error("Error Insert, Project Task for Plan : " + ParentPlan.c_name + " not found!");
                    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Error.Message = "Project Task for Plan : " + ParentPlan.c_name + " not found!";
                    dr.CloseConnection();
                    return false;
                }
                // TODO: AH Hier das richtige Plan Item finden
                //if (ParentWorkitems.Count > 1)
                //{
                //    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                //    Error.Message = "More then 1 Project Task for Plan : " + ParentPlan.c_name + " not found!";
                //    dr.CloseConnection();
                //    return false;
                //}
                var suche = Task.GetWBSCodeFromName(task.WBSCode);
                suche = (suche.Contains(".") ? suche.Substring(0, suche.LastIndexOf('.')) : suche) + " ";
                ParentWorkitem = ParentWorkitems.FirstOrDefault(x => x.c_title.StartsWith(suche));
                if (ParentWorkitem == null)
                {   // Wenn nicht gefunden das 1. nehmen
                    ParentWorkitem = ParentWorkitems[0];
                }
                task.ParentId = ParentWorkitem.c_id;
            }
            else
            {
                ParentWorkitem = new PmWorkPackageDB(); // empty parent because there is no parent when inserting the root plan
            }

            Connection con = pi.Init(MSP.Username, MSP.Password);

            if (con == null)
            {
                Log.Error("Error Insert, Login not sucessfull");
                Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Error.Message = "Login not sucessfull";
                dr.CloseConnection();
                return false;
            }
            // Insert Plan und project task
            if (po.InsertPlan(con, MSP.ProjectID, ParentWorkitem.c_id, task, MSP.Baseplan, out string error1).Item1)
            {
                // Plan Insert OK
            }
            else
            {
                // Fehler beim Plan Insert
                Log.Error("Error Insert, " + error1);
                Error.StatusCode = System.Net.HttpStatusCode.ExpectationFailed;
                Error.Message = error1;
                dr.CloseConnection();
                return false;
            }

            dr.CloseConnection();
            return true;
        }

        /// <summary>
        /// 1.) Der parent Plan wird über die ParentID gesucht - wenn keine ParentId vorhanden ->
        /// 2.) Der Parent Plan wird über den WBSCode gesucht
        /// 2019-11-11 Änderung - es wird immer über den WBS Code gesucht
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="ProjectId"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public PlanApiDB GetParentPlan(DatareaderP dr, string ProjectId, Task t, out string Error)
        {
            Error = "";
            PlanApiDB ParentPlan = null;
            if (string.IsNullOrEmpty(t.ParentId))
            {
                // Parent über WBS-Code suchen
                ParentPlan = dr.GetPlanByWBSCode(ProjectId, t.GetParentWBSCode(t.WBSCode), out string Error1);
                Error = Error1;
            }
            else
            {
                ParentPlan = dr.GetPlanByID(ProjectId, t.ParentId, out string Error2);
                Error = Error2;
            }
            return ParentPlan;
        }

        public PmWorkPackageDB GetParentprojectTask(DatareaderP dr, string ProjectId, Task t)
        {
            PmWorkPackageDB ParentProjectTask = null;

            return ParentProjectTask;
        }

        /// <summary>
        /// Es wird überprüft, ob ein Plan mit dem übergebenen WBSCode bereits in Polarion vorhanden  ist
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="ProjectId"></param>
        /// <param name="WBSCode"></param>
        /// <returns>
        ///     false ... Plan vorhanden
        ///     true .... Plan nicht vorhanden
        /// </returns>
        public bool CheckNewWBSCode(DatareaderP dr, string ProjectId, string WBSCode)
        {
            PlanApiDB plan = dr.GetPlanByWBSCode(ProjectId, WBSCode, out string Error);
            if (plan != null && plan.c_id.Length > 0)
            {
                // Plan mit dem übergebenen WBS Code bereits in Polarion Projekt vorhanden.
                return true;
            }
            return false;
        }
    }
}