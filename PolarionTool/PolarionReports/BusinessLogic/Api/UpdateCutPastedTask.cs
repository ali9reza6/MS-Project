using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic.Api
{
    public class UpdateCutPastedTask
    {
        /// <summary>
        /// When a task is Cut or Deleted in MS-Project and pasted again in another place (Level) it is expected for it to be updated
        /// This method reads those deleted and pasted Tasks filtering by: 
        /// (MS.Task.WiId exists AND Polarion workpackage with same ID has name "[deleted]")
        /// The method created a new Task and Updates it with the deleted task parameters
        /// </summary>
        /// <returns></returns>
        public Task InsertUpdate(Task msTask, ProjectModel MSP, string deletedWiId, ProjectModel pm, out ApiError Error)
        {
            bool insertbasePlan = false;
            Error = new ApiError();
            DatareaderP dr = new DatareaderP();
            Polarion po = new Polarion();
            PolarionInit pi = new PolarionInit();
            Task PolarionTask;
            PlanApiDB ParentPlan;
            PmWorkPackageDB ParentWorkitem;
            List<PmWorkPackageDB> ParentWorkitems;
            TaskReader taskreader = new TaskReader();

            // parentId löschen - stimmt nich beim einfügen

            msTask.ParentId = "";
            ParentPlan = GetParentPlan(dr, MSP.ProjectID, msTask, out string errorGetParentPlan);

            if (errorGetParentPlan != "")
            {
                // Datenbankfehler
                Log.Error("Error Insert, Datenbankfehler: " + errorGetParentPlan);
                Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                Error.Message = errorGetParentPlan;
                dr.CloseConnection();
                return null;
            }
            if (ParentPlan == null)
            {
                // kein Wurzelplan gefunden: prüfen, ob WBS-Code 1 stellig
                if (msTask.WBSCode.Length == 1)
                {
                    // Wurzelplan soll eingefügt werden
                    Log.Debug("Insert Baseplan");
                    insertbasePlan = true;
                }
                else
                {
                    // ungültige Aufruf:
                    Log.Error("Error Insert, ParantId not Found in Polarion: " + msTask.ParentId);
                    Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    Error.Message = "ParantId not Found in Polarion: " + msTask.ParentId;
                    dr.CloseConnection();
                    return null;
                }
            }

            if (!insertbasePlan)
            {
                msTask.ParentPlanId = ParentPlan.c_id;    // parent Plan in neuen task eintragen

                // ParentProjectTask Workitem suchen
                ParentWorkitems = dr.GetPmWorkPackageForPlan(ParentPlan.c_pk, out string ErrorGetPmWorkPackageForPlan);
                if (ErrorGetPmWorkPackageForPlan != "")
                {
                    // Datenbankfehler
                    Log.Error("Error Insert, Database: " + ErrorGetPmWorkPackageForPlan);
                    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Error.Message = ErrorGetPmWorkPackageForPlan;
                    return null;
                }
                if (ParentWorkitems.Count == 0)
                {
                    Log.Error("Error Insert, Project Task for Plan : " + ParentPlan.c_name + " not found!");
                    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Error.Message = "Project Task for Plan : " + ParentPlan.c_name + " not found!";
                    dr.CloseConnection();
                    return null;
                }
                // TODO: AH Hier das richtige Plan Item finden
                //if (ParentWorkitems.Count > 1)
                //{
                //    Error.StatusCode = System.Net.HttpStatusCode.NotFound;
                //    Error.Message = "More then 1 Project Task for Plan : " + ParentPlan.c_name + " not found!";
                //    dr.CloseConnection();
                //    return false;
                //}
                var suche = Task.GetWBSCodeFromName(msTask.WBSCode);
                suche = (suche.Contains(".") ? suche.Substring(0, suche.LastIndexOf('.')) : suche) + " ";
                ParentWorkitem = ParentWorkitems.FirstOrDefault(x => x.c_title.StartsWith(suche));
                if (ParentWorkitem == null)
                {   // Wenn nicht gefunden das 1. nehmen
                    ParentWorkitem = ParentWorkitems[0];
                }
                msTask.ParentId = ParentWorkitem.c_id;
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
                return null;
            }
            // Insert Plan und project task
            var insertPlanResults = po.InsertPlan(con, MSP.ProjectID, ParentWorkitem.c_id, msTask, MSP.Baseplan, out string error1);
            if (insertPlanResults.Item1)
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
                return null;
            }

            pm = UpdteProjectModel(pm, MSP, taskreader, dr);

            PolarionTask = UpdateFromDeletedWorkItem(MSP.ProjectID, pm, con, msTask, deletedWiId); //PROBLEM HERE

            dr.CloseConnection();

            return PolarionTask;
        }

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

        private ProjectModel UpdteProjectModel(ProjectModel pm, ProjectModel MSP, TaskReader taskreader, DatareaderP dr)
        {
            List<PlanApiDB> plans = dr.GetPlanForProject(MSP.ProjectID, out string error);
            pm.Tasks = taskreader.GetPMTasks(dr, plans, pm.Baseplan);
            return pm;
        }

        private Task UpdateFromDeletedWorkItem(string projectId, ProjectModel pm, Connection con, Task msTask, string deletedWiId)
        {
            // Poltask has only one projecttask  
            Task PolarionTask = pm.Tasks.FirstOrDefault(p => p.WorkitemId == msTask.WorkitemId); //Deleted (Not linked tasks not included)
            if (PolarionTask != null)                                                              // ProjectModel pm ... is not updated here ... No polTask found
            {
                // Inhalte vergleichen:
                if (CompareAndUpdate(con, projectId, msTask, PolarionTask, deletedWiId))
                {
                    // Alles OK
                }
                else
                {
                    // Fehler ist aufgetreten
                }
            }
            else
            {
                // project Task nicht in Polarion
            }

            return PolarionTask;
        }

        bool CompareAndUpdate(Connection con, string ProjectId, Task MSprojectTask, Task PolarionTask, string deletedWiId)
        {
            // TODO: AH
            // Compare fields:
            //if ((!CompareDate(MSprojectTask.Finish, PolarionTask.Finish)) ||             //Not necessary?
            //    (!CompareDate(MSprojectTask.Start, PolarionTask.Start)) ||
            //    (MSprojectTask.Name != PolarionTask.Name) ||
            //    (MSprojectTask.WBSCode != PolarionTask.WBSCode) ||
            //    (MSprojectTask.ProjectId != PolarionTask.ProjectId))
            //{
                // Fields different-> Update Polarion:
                Polarion po = new Polarion();
                //if (!string.IsNullOrEmpty(PolarionTask.PlanId)) //if its a plan            Not implemented yet
                //{
                //    po.UpdatePlan(con, ProjectId, MSprojectTask);
                //}

            //    if (!string.IsNullOrEmpty(MSprojectTask.WorkitemId)) //if its a WI
            //    {
            //        po.UpdateFromDeletedWorkitem(con, ProjectId, MSprojectTask, deletedWiId); // Not necessary, delete this method
            //    }

            ////}

            return true;
        }

        bool CompareDate(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
        }
    }
}