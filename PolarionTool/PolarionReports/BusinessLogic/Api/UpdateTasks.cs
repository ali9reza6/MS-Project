using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic.Api
{
    /// <summary>
    /// Diese Methode führt ein komplettes update der Polarion Properties die von MS-Project übernommen werden durch:
    /// This method carries out a complete update of the Polarion properties that are set on MS Project:
    /// 
    /// Plan:   MS-Project(Task)    Polarion
    ///        ----------------------------------
    ///         Finish              c_duedate
    ///         start               c_startdate
    ///         Name                c_name = WBSCode + " " + Name
    ///         ProjectId           c_sortorder
    ///         WBSCode
    ///         
    /// Workitem (Type: projectTask)
    ///         Finish              c_duedate
    ///         start               c_plannedstart
    ///         Name                c_title = WBSCode + " " + Name
    ///         
    /// @@@ zu klären:
    ///         Parent in MS-Project ändern
    ///         
    /// </summary>
    public class UpdateTasks
    {
        public ProjectModel Update(ProjectModel MSP, out ApiError Error)
        {
            Error = new ApiError();
            TaskReader taskreader = new TaskReader();
            PolarionInit pi = new PolarionInit();
            ProjectModel pm = new ProjectModel();
            DatareaderP dr = new DatareaderP();
            Task PolarionTask;
            Polarion po = new Polarion();

            List<PlanApiDB> plans = dr.GetPlanForProject(MSP.ProjectID, out string error);

            pm.ProjectID = MSP.ProjectID;
            pm.Baseplan = MSP.Baseplan;
            pm.Tasks = taskreader.GetPMTasks(dr, plans, pm.Baseplan); //Returns Polarion Tasks (wt Polarion info) 
            //Until here all WIs with status


            //Until here percent complete is there!
           
            // TODO: AH - hier ist die Problematik, dass 15 vs. 9 kommt...
            // (Polarion) pm mit MSP updaten
            //if (pm.Tasks.Count != MSP.Tasks.Count)                         //checks for different numbr of tasks betweeen the 2 models
            //{
            //    // Anzahl ungleich => Update Fehlermeldung
            //    Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
            //    Error.Message = "Number of Task " + MSP.Tasks.Count.ToString() + " not equal number of tasks in Polarion " + pm.Tasks.Count.ToString(); 
            //    return false;
            //}

            Connection con = pi.Init(MSP.Username, MSP.Password);

            // Fehlerbehandlung Login nicht Erfolgreich
            if (con == null)
            {
                Error.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Error.Message = "Login not sucessfull";
                return null;
            }


            try
            {
                foreach (Task msTask in MSP.Tasks)  //IS MS. Task we are looking for here??... check IsDeletedWorkItem(Connection con, string ProjectId, string WorkitemId)
                {
                   
                    //Only Plans updated
                    if (!string.IsNullOrEmpty(msTask.PlanId))
                    {
                        // task hat einen Plan und einen projectTask
                        // task in pm suchen:
                        PolarionTask = pm.Tasks.FirstOrDefault(p => p.PlanId == msTask.PlanId);  

                        if (PolarionTask != null)
                        {
                            // Inhalte vergleichen:
                            if (this.CompareAndUpdate(con, MSP.ProjectID, msTask, PolarionTask)) 
                            {
                                // Alles OK
                            }
                            else
                            {
                                // Fehler ist aufgetreten
                                Log.Error("Error in UpdateTasks.CompareAndUpdate(...) method");
                            }
                        }
                        else
                        {
                            try
                            { 
                            // Fehler Plan nicht in Polarion Liste
                            PolarionTask = UpdateWorkItem(MSP.ProjectID, pm, con, msTask);
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("EXCEPTION ERROR updating Plan" + ex.Message);
                                Debug.WriteLine("EXCEPTION ERROR updating Plan" + ex.Message);
                                throw;
                            }
                        }
                    }

                    //Only WorkItems updated
                    else if (!string.IsNullOrEmpty(msTask.WorkitemId))
                    {
                        bool wasDeleted = po.IsDeletedWorkItem(con, MSP.ProjectID, msTask.WorkitemId); //problem here!
                        
                        if (wasDeleted)
                            {
                                string deletedWiId = msTask.WorkitemId;
                                UpdateCutPastedTask updateCutPastedtask = new UpdateCutPastedTask();
                                PolarionTask = updateCutPastedtask.InsertUpdate(msTask, MSP, deletedWiId, pm, Error: out ApiError er);
                                //Careful MSP can be NULL
                            }
                            else
                            {
                                try
                                {
                                    PolarionTask = UpdateWorkItem(MSP.ProjectID, pm, con, msTask);  //Check here as well
                                }
                                catch (Exception ex)
                                {
                                    Log.Debug("EXCEPTION IM LOOKING FOR in UpdateWorkItem WorkItem!" + ex.Message);
                                    Debug.WriteLine("EXCEPTION IM LOOKING in UpdateWorkItem WorkItem!" + ex.Message);
                                    throw;
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("EXCEPTION IM LOOKING FOR" + ex.Message);
                Debug.WriteLine("EXCEPTION IM LOOKING FOR" + ex.Message);
                throw;
            }

            dr.CloseConnection();
            //ALL STATUSES here = ""
            return MSP;
        }

        private Task UpdateWorkItem(string projectId, ProjectModel pm, Connection con, Task t)
        {
            // Poltask has only one projecttask  
            Task PolarionTask = pm.Tasks.FirstOrDefault(p => p.WorkitemId == t.WorkitemId); //(Not linked tasks not included so Deleted WIs not included in this list)
            if (PolarionTask != null)
            {
                // Inhalte vergleichen:
                if (this.CompareAndUpdate(con, projectId, t, PolarionTask))
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

        bool CompareAndUpdate(Connection con, string ProjectId, Task MSprojectTask, Task PolarionTask)
        {   
            // TODO: AH
            // Compare fields:
            if ((!CompareDate(MSprojectTask.Finish, PolarionTask.Finish)) ||       
                (!CompareDate(MSprojectTask.Start, PolarionTask.Start)) ||
                (MSprojectTask.Name != PolarionTask.Name) ||
                (MSprojectTask.WBSCode != PolarionTask.WBSCode) ||
                (MSprojectTask.ProjectId != PolarionTask.ProjectId))
            {
                // Fields different-> Update to Polarion:
                Polarion po = new Polarion();
                if (!string.IsNullOrEmpty(PolarionTask.PlanId)) //if its a plan
                {
                    po.UpdatePlan(con, ProjectId, MSprojectTask);
                }
                if (!string.IsNullOrEmpty(MSprojectTask.WorkitemId)) //if its a WI
                {
                    po.UpdateWorkitem(con, ProjectId, MSprojectTask);
                }
            }
            // Fields different-> Update to MSP model (to be returned to add in)
            if ((MSprojectTask.PercentComplete != PolarionTask.PercentComplete) ||
                (MSprojectTask.Status != PolarionTask.Status))
            {
                MSprojectTask.PercentComplete = PolarionTask.PercentComplete;
                MSprojectTask.Status = PolarionTask.Status;
            }

            return true;
        }

        bool CompareDate(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
        }
    }
}