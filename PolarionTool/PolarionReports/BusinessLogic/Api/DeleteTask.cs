using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Serilog;

namespace PolarionReports.BusinessLogic.Api
{
    public class DeleteTask
    {
        public bool delete(ProjectModel msp, out ApiError apierror)
        {
            apierror = new ApiError();
            PolarionInit pi = new PolarionInit();
            Polarion po = new Polarion();
            Connection con = pi.Init(msp.Username, msp.Password);

            if (con == null)
            {
                Log.Error("Delete Task, Login not sucessfull");
                apierror.StatusCode = HttpStatusCode.BadRequest;
                apierror.Message = "Login not sucessfull";
                return false;
            }

            PlanApiDB planToDelete;
            PlanWorkitemLinkList planWorkitemLinkList;

            if (msp.ProjectID == null || msp.ProjectID.Length == 0)
            {
                Log.Error("Delete Task, ProjectID missing");
                apierror.StatusCode = HttpStatusCode.BadRequest;
                apierror.Message = "ProjectID missing";
                return false;
            }

            if (msp.Tasks == null || msp.Tasks.Count != 1)
            {
                Log.Error("Delete Task, Only 1 task in Tasklist allowed");
                apierror.StatusCode = HttpStatusCode.BadRequest;
                apierror.Message = "Only 1 task in Tasklist allowed";
                return false;
            }

            DatareaderP dr = new DatareaderP();
            // Project in DB suchen
            ProjectDB p = dr.GetProjectByID(msp.ProjectID, out string ErrorGetProjectByID);
            if (p.Id == null)
            {
                Log.Error("Delete task, Project with ID: " + msp.ProjectID + " not found");
                apierror.StatusCode = HttpStatusCode.BadRequest;
                apierror.Message = "Project with ID:" + msp.ProjectID + " not found";
                return false;
            }

            // Task einen Plan und ein Workitem haben
            Task t = msp.Tasks[0];

            // Verbindung von Plan und Workiten löschen
            if (!string.IsNullOrEmpty(t.PlanId))
            {
                Log.Debug("Delete Task, PlanId=" + t.PlanId);
                planToDelete = dr.GetPlanByID(msp.ProjectID, t.PlanId, out string ErrorGetPlanByID);
                if (planToDelete == null)
                {
                    Log.Error("Delete Task, Plan with ID:" + t.PlanId + " not found");
                    apierror.StatusCode = HttpStatusCode.BadRequest;
                    apierror.Message = "Plan with ID:" + t.PlanId + " not found";
                    dr.CloseConnection();
                    return false;
                }

                planWorkitemLinkList = dr.GetPlanWorkitemLinkList(planToDelete.c_pk, out string Error);
                if (planWorkitemLinkList.PlanWorkitemLinks.Count > 1)
                {
                    Log.Error("Delete Task, More then 1 Workitem linked with the plan id: " + planToDelete.c_id);
                    apierror.StatusCode = HttpStatusCode.PreconditionFailed;
                    apierror.Message = "More then 1 Workitem linked with the plan id:" + planToDelete.c_id;
                    dr.CloseConnection();
                    return false;
                }
                if (planWorkitemLinkList.PlanWorkitemLinks.Count == 1)
                {
                    Log.Debug("Delete Task, Remove PlanItem: " + planToDelete.c_id);
                    if (po.RemovePlanItem(con, msp.ProjectID, planToDelete.c_id, planWorkitemLinkList.PlanWorkitemLinks[0].WorkitemId))
                    {
                        // Remove OK
                    }
                    else
                    {
                        Log.Error("Delete Task, Cannot remove the Workitem from the plan with id: " + planToDelete.c_id);
                        apierror.StatusCode = HttpStatusCode.PreconditionFailed;
                        apierror.Message = "Cannot remove the Workitem from the plan with id:" + planToDelete.c_id;
                        dr.CloseConnection();
                        return false;
                    }
                }
            }


            // Remove Workitem
            if (!string.IsNullOrEmpty(t.WorkitemId))
            {
                // Der task hat ein Workitem
                Log.Debug("Delete Task, Remove Workitem: " + t.WorkitemId);
                Workitem w = dr.GetWorkitem(t.WorkitemId, out string ErrorGetWorkitem);
                if (w.Id == null)
                {
                    Log.Error("Delete Task, Workpackage with ID:" + t.WorkitemId + " not found");
                    apierror.StatusCode = HttpStatusCode.BadRequest;
                    apierror.Message = "Workpackage with ID:" + t.WorkitemId + " not found";
                    dr.CloseConnection();
                    return false;
                }

                if (w.Type == "projectTask" || w.Type == "pmWorkPackage" || w.Type == "workPackage")
                {
                    // Hat das WP Verknüpfungen ?
                    List<Models.Database.Api.Downlink> Downlinks = dr.GetWorkitemDownlinks(w.Id, out string ErrorGetWorkitemDownlinks);
                    if (Downlinks.Count > 0)
                    {
                        Log.Error("Delete Task, Workpackage with ID: " + t.WorkitemId + " has links to " + Downlinks.Count.ToString() + " Workitems " +
                                           "check and remove the links in Polarion");
                        apierror.StatusCode = HttpStatusCode.BadRequest;
                        apierror.Message = "Workpackage with ID:" + t.WorkitemId + " has links to " + Downlinks.Count.ToString() + " Workitems " +
                                           "check and remove the links in Polarion";
                        dr.CloseConnection();
                        return false;
                    }

                    List<Models.Database.Api.Uplink> Uplinks = dr.GetWorkitemUplinks(w.Id, out string ErrorGetWorkitemUplinks);
                    if (Uplinks.Count == 1)
                    {
                        Models.Database.Api.Uplink ul = Uplinks[0];
                        if (ul.Down.c_type == ul.Up.c_type)
                        {
                            // removes the link
                            if (po.RemoveLinkedWorkitem(con, msp.ProjectID, ul.Down.c_id, ul.Up.c_id, ul.c_role))
                            {
                                // remove the workitem -> Mark the Workitem for delete (es gibt derzeit kein Webinterface für das löschen eines Workitems
                                // Vorschlag: Im Titel [deleted] + Titel hinterlegen
                                t.Name = "[deleted] " + t.Name;
                                po.DeleteWorkitemFromName(con, msp.ProjectID, t);
                                //po.UpdateWorkitem(con, msp.ProjectID, t); //Resets the Start Date-> Unconvenient
                            }
                            else
                            {
                                Log.Error("Delete Task, Unable to remove Uplink from ID:" + t.WorkitemId + " to " + ul.Up.c_id + "check and remove the links in Polarion");
                                apierror.StatusCode = HttpStatusCode.BadRequest;
                                apierror.Message = "Unable to remove Uplink from ID:" + t.WorkitemId + " to " + ul.Up.c_id + "check and remove the links in Polarion";
                                dr.CloseConnection();
                                return false;
                            }
                        }
                    }
                    if (Uplinks.Count > 1)
                    {
                        Log.Error("Delete task, More then 1 Workitem linked with the workitem id:" + w.Id);
                        apierror.StatusCode = HttpStatusCode.PreconditionFailed;
                        apierror.Message = "More then 1 Workitem linked with the workitem id:" + w.Id;
                        dr.CloseConnection();
                        return false;
                    }
                }
                else
                {
                    Log.Error("Delete Task, More then 1 Workitem linked with the workitem id:" + w.Id);
                    apierror.StatusCode = HttpStatusCode.BadRequest;
                    apierror.Message = "More then 1 Workitem linked with the workitem id:" + w.Id;
                    dr.CloseConnection();
                    return false;
                }
            }
            
            if (!string.IsNullOrEmpty(t.PlanId))
            {
                // Plan löschen
                Log.Debug("Delete Task, delete Plan: " + t.PlanId);
                string[] planIds = { t.PlanId };
                if (po.DeletePlans(con, msp.ProjectID, planIds))
                {
                    // delete OK
                }
                else
                {
                    Log.Error("Delete Task, Polarion error delting the plan id:" + t.PlanId);
                    apierror.StatusCode = HttpStatusCode.Conflict;
                    apierror.Message = "Polarion error delting the plan id:" + t.PlanId;
                    dr.CloseConnection();
                    return false;
                }
            }

            dr.CloseConnection();
            return true;
        }

        public bool DeleteWorkitem(string workitemId, out ApiError apierror)
        {
            apierror = new ApiError();

            return true;
        }

    }
}