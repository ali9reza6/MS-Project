using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.Models.Gantt;
using PolarionReports.Models.MSProjectApi;
using PolarionReports.Models.PlanTreeSort;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using PlanningService = net.seabay.polarion.Planning;
using ProjectService = net.seabay.polarion.Project;
using TrackerService = net.seabay.polarion.Tracker;
using TestrunService = net.seabay.polarion.TestManagement;
using net.seabay.polarion;
//using net.seabay.polarion.Tracker;

namespace PolarionReports.BusinessLogic
{
    public class Polarion
    {
        /// <summary>
        /// Maximaltiefe wo noch PlanItems angelegt werden, darunter nur mehr workitems!
        /// </summary>
        /// 

        public static int MaxDeepPlan
        {
            get
            {
                var deep = ConfigurationManager.AppSettings["MaxDeepPlan"];
                int.TryParse(deep, out int ret);
                return ret;
            }
        }

        /// <summary>
        /// löscht die Pläne aus dem Projeket
        /// </summary>
        /// <param name="con">Connection to Polarion</param>
        /// <param name="ProjectId">Id des Projekts</param>
        /// <param name="PlanIds">Array of PlanIds</param>
        /// <returns></returns>
        /// <remarks>
        /// History 2020-03-05 LogFile
        /// </remarks>
        public bool DeletePlans(Connection con, string ProjectId, string[] PlanIds)
        {
            try
            {
                con.Planning.deletePlans(ProjectId, PlanIds);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei con.Planning.deletePlans " + e.Message);
                Debug.WriteLine(e.ToString());
                return false;
            }
            return true;
        }


        /// <summary>
        /// markiert ein Workitem für löschen.
        /// Da es kein API für das löschen eines Worlitems gibt, wird das zu löschende Worlitem
        /// Im Titel mit [delete] gekennzeichnet.
        /// Es muss dann im Polarion über die Suche gelöscht werden
        /// </summary>
        /// <param name="con">Connection to Polarion</param>
        /// <param name="WorkitemId">Id des zu löschenden Workitems</param>
        /// <returns></returns>
        public bool DeleteWorkitem(Connection con, string ProjectId, string WorkitemId)
        {
            try
            {
                TrackerService.WorkItem w = con.Tracker.getWorkItemById(ProjectId, WorkitemId);
                w.title = "[delete] " + w.title;
                try
                {
                    con.Tracker.updateWorkItem(w);
                }
                catch (Exception e)
                {
                    Log.Error("Polarion - DeleteWorkitem:" + e.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error("Polarion - DeleteWorkitem:" + e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 1.) Fügt den "ActualTemplatePlan" in das TagretProject ein
        /// 2.) Fügt alle im Project "Template" verknüpften Workitems zu dem neuem Plan hinzu
        /// </summary>
        /// <param name="con">Polarion Connection Object</param>
        /// <param name="vm">Viewmodel </param>
        /// <param name="ActualTemplatePlan">Einzufügender Plan aus dem Template</param>
        /// <param name="TargetPlanParentId">Parent - unter diesem Node wird der Plan eingefügt</param>
        /// <returns>id des neuen Plans</returns>
        /// <remarks>
        /// History: 2020-03-05 Fehlerausgaben in Log-File
        /// </remarks>
        public string InsertPlanAndWorkItem(Connection con, PolarionPlanInsertViewModel vm, Models.Database.Plan ActualTemplatePlan, string TargetPlanParentId, out string ErrorMsg)
        {
            // Workitems des Template Plans einlesen
            // TemplatePlan.WorkitemLinks = 

            bool Test = false;
            string Error = "";
            string PlanURI;
            string WorkitemURI;
            string TemplateID = GetTemplateId(ActualTemplatePlan); // release, iteration, Milestone
            List<string> Workitems = new List<string>();
            TrackerService.WorkItem TemplateWorkitem;
            TrackerService.WorkItem NewWorkitem;
            ProjectService.Project PolarionTargetProject;
            PlanningService.Plan NewPlan;

            try
            {
                PolarionTargetProject = con.Project.getProject(vm.TargetProject.Id);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei: con.Project.getProject");
                ErrorMsg = e.Message;
                return "";
            }

            // Insert Plan

            Debug.WriteLine(ActualTemplatePlan.Plandb.Id);
            if (Test)
            {
                ErrorMsg = Error;
                return "";
            }

            try
            {
                if (vm.InsertFromTemplateProject)
                {
                    // Einfügen von Plänen aus dem Template Projekt (Beschaffungsplan, PM Plan, Hardware ....)
                    // Hier werden die Plannamen aus dem Template übernommen
                    if (TargetPlanParentId == "0")
                    {
                        PlanURI = con.Planning.createPlan(vm.TargetProject.Id, ActualTemplatePlan.Plandb.Name, ActualTemplatePlan.Plandb.Id, null, ActualTemplatePlan.TemplateId);
                    }
                    else
                    {
                        PlanURI = con.Planning.createPlan(vm.TargetProject.Id, ActualTemplatePlan.Plandb.Name, ActualTemplatePlan.Plandb.Id, TargetPlanParentId, ActualTemplatePlan.TemplateId);
                    }
                }
                else
                {
                    if (TargetPlanParentId == "0")
                    {
                        PlanURI = con.Planning.createPlan(vm.TargetProject.Id, ActualTemplatePlan.Plandb.Id, ActualTemplatePlan.Plandb.Id, null, ActualTemplatePlan.TemplateId);
                    }
                    else
                    {
                        PlanURI = con.Planning.createPlan(vm.TargetProject.Id, ActualTemplatePlan.Plandb.Id, ActualTemplatePlan.Plandb.Id, TargetPlanParentId, ActualTemplatePlan.TemplateId);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Fehler createPlan" + e.Message);
                ErrorMsg = "Fehler createPlan" + e.Message;
                return "";
            }

            NewPlan = con.Planning.getPlanByUri(PlanURI);

            // Insert Workitems:
            try
            {
                if (ActualTemplatePlan.WorkitemLinks != null)
                {
                    foreach (PlanWorkitemLink l in ActualTemplatePlan.WorkitemLinks.PlanWorkitemLinks)
                    {
                        TemplateWorkitem = con.Tracker.getWorkItemById(vm.TemplateProject.Id, l.WorkitemId);
                        NewWorkitem = new TrackerService.WorkItem();

                        NewWorkitem.project = new TrackerService.Project();
                        NewWorkitem.project.uri = PolarionTargetProject.uri;

                        NewWorkitem.type = TemplateWorkitem.type;
                        NewWorkitem.severity = TemplateWorkitem.severity;
                        NewWorkitem.priority = TemplateWorkitem.priority;
                        NewWorkitem.description = TemplateWorkitem.description;
                        NewWorkitem.status = TemplateWorkitem.status;
                        NewWorkitem.title = TemplateWorkitem.title;
                        NewWorkitem.customFields = TemplateWorkitem.customFields; // -> erzeugt Fehler im API ???? bei jetzigem Template funktioniert es 
                        
                        WorkitemURI = con.Tracker.createWorkItem(NewWorkitem);
                        Workitems.Add(WorkitemURI);
                    }

                    // Link all new Workitems with The Plan
                    if (Workitems.Count > 0)
                    {
                        con.Planning.addPlanItems(PlanURI, Workitems.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Fehler createWorkitem" + e.Message);
                ErrorMsg = "Fehler createWorkitem" + e.Message;
                return "";
            }
            ErrorMsg = Error;
            return NewPlan.id;
        }

        /// <summary>
        /// Hier wird für jeden einzufügenden Plan Aufgrund des Plan Namens das korrespondierende Template gesucht
        /// </summary>
        /// <param name="plan"></param>
        /// <returns>ID des Template Plans </returns>
        /// <remarks>
        /// History: 2020-01-20 neue Templates eingebaut
        ///          2020-03-05 TemplateIDs werden aus dem Template-Projekt übernommen. Somit ist diese Funktion obsolet
        /// </remarks>
        private string GetTemplateId(Plan plan)
        {
            // Templates für SW Release Plan (Hauptplan mit Milestones)
            if (plan.Plandb.Name.Contains("SW M0"))
            {
                return "SW_T_Milestone_M0";
            }
            if (plan.Plandb.Name.Contains("SW M1"))
            {
                return "SW_T_Milestone_M1";
            }
            if (plan.Plandb.Name.Contains("SW M2"))
            {
                return "SW_T_Milestone_M2";
            }
            if (plan.Plandb.Name.Contains("SW M3"))
            {
                return "SW_T_Milestone_M3";
            }
            if (plan.Plandb.Name.Contains("SW M4"))
            {
                return "SW_T_Milestone_M4";
            }
            if (plan.Plandb.Name.Contains("SW M5"))
            {
                return "SW_T_Milestone_M5";
            }
            if (plan.Plandb.Name.Contains("SW M8"))
            {
                return "SW_T_Milestone_M8";
            }

            // Beschaffungsprojekt
            if (plan.Plandb.Name.Contains("B M0"))
            {
                return "PM_T_B_M0_Nomination";
            }
            if (plan.Plandb.Name.Contains("B M1"))
            {
                return "PM_T_B_M1_Project_Start";
            }
            if (plan.Plandb.Name.Contains("B M2"))
            {
                return "PM_T_B_M2_Concept_released";
            }
            if (plan.Plandb.Name.Contains("B M3"))
            {
                return "PM_T_B_M3_Purchasing_released";
            }
            if (plan.Plandb.Name.Contains("B M4"))
            {
                return "PM_T_B_M4_ready_for_Integration";
            }
            if (plan.Plandb.Name.Contains("B M5"))
            {
                return "PM_T_B_M5_Production_Line_released";
            }

            // Entwicklungsprojekt
            if (plan.Plandb.Name.Contains("E M0"))
            {
                return "PM_T_E_M0_Nomination";
            }
            if (plan.Plandb.Name.Contains("E M1"))
            {
                return "PM_T_E_M1_Project_Start";
            }
            if (plan.Plandb.Name.Contains("E M2"))
            {
                return "PM_T_E_M2_Concept_Release";
            }
            if (plan.Plandb.Name.Contains("E M3"))
            {
                return "PM_T_E_M3_Design_Freeze";
            }
            if (plan.Plandb.Name.Contains("E M4"))
            {
                return "PM_T_E_M4_Validation_Readiness";
            }
            if (plan.Plandb.Name.Contains("E M5"))
            {
                return "PM_T_E_M5_Product_Design_Release";
            }
            if (plan.Plandb.Name.Contains("E M6"))
            {
                return "PM_T_E_M6_PSO";
            }
            if (plan.Plandb.Name.Contains("E M7"))
            {
                return "PM_T_E_M7_SOP";
            }
            if (plan.Plandb.Name.Contains("E M8"))
            {
                return "PM_T_E_M8_Project_End";
            }

            if (plan.Plandb.Name.Contains("PM M0") || plan.Plandb.Name.Contains("PM E M0") || plan.Plandb.Name.Contains("PM B M0"))
            {
                return "PM_Milestone_M0";
            }
            else if (plan.Plandb.Name.Contains("PM M") || plan.Plandb.Name.Contains("PM E M") || plan.Plandb.Name.Contains("PM B M"))
            {
                return "PM_Milestone_MX";
            }
            else
            {
                switch (plan.PlanType)
                {
                    case Plan.Plantype.R:
                        return "release";

                    case Plan.Plantype.I:
                        return "iteration";

                    case Plan.Plantype.M:
                        return "Milestone";

                    default:
                        return "Release";
                }
            }
        }

        private string GetWorkitemIdFromTargetProject(PolarionPlanInsertViewModel vm)
        {
            foreach (Plan p in vm.TargetProjectPlans.Plans)
            {
                if (p.WorkitemLinks != null)
                {
                    if (p.WorkitemLinks.PlanWorkitemLinks.Count > 0)
                    {
                        PlanWorkitemLink l = p.WorkitemLinks.PlanWorkitemLinks[0];
                        return l.WorkitemId;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Fügt ein PlanTemplate in das Projekt ein
        /// </summary>
        /// <param name="con">Aktive Polarion Connection</param>
        /// <param name="projectId">ID des Zielproject</param>
        /// <param name="planTemplateId">ID des PlanTemplates (muss im Project "Template" definiert sein)</param>
        /// <returns></returns>
        public bool InsertPlanTemplate(Connection con, PlanTemplate template, string projectId, out string error)
        {
            error = "";
            string TemplateProjectId = "Template";
            string newPlanTemplateURI;
            string planTemplateEmpty = null;
            PlanningService.Plan planTemplate;
            PlanningService.Plan newPlanTemplate;

            // PlanTemplate aus Template Projekt laden

            try
            {
                planTemplate = con.Planning.getPlanById(TemplateProjectId, template.C_Id);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei con.Planning.getPlanById " + e.Message);
                error = e.Message;
                return false;
            }

            if (planTemplate.id != template.C_Id)
            {
                Log.Error("Error InsertPlanTemplate, wrong template: " + planTemplate.id);
                error = "wrong template: " + planTemplate.id;
                return false;
            }


            // PlanTemplate in Zielprojekt speichern

            try
            {
                newPlanTemplateURI = con.Planning.createPlanTemplate(projectId, template.C_Name, template.C_Id, planTemplateEmpty);
                newPlanTemplate = con.Planning.getPlanByUri(newPlanTemplateURI);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei PlanTemplate in Zielprojekt speichern " + e.Message);
                error = e.Message;
                return false;
            }

            // Bestimmte Felder aus dem Template Projekt übertragen
            int c = planTemplate.allowedTypes.Count();
            newPlanTemplate.allowedTypes = new PlanningService.EnumOptionId[c];
            int i = 0;
            foreach (PlanningService.EnumOptionId item in planTemplate.allowedTypes)
            {
                PlanningService.EnumOptionId allowedType = new PlanningService.EnumOptionId();
                allowedType.id = planTemplate.allowedTypes[i].id;
                con.Planning.addPlanAllowedType(newPlanTemplateURI, allowedType);
                i++;
            }
            // newPlanTemplate.allowedTypes = planTemplate.allowedTypes;
            newPlanTemplate.color = planTemplate.color;
            newPlanTemplate.description = planTemplate.description;
            newPlanTemplate.homePageContent = planTemplate.homePageContent;

            // wiki Content übernehmen
            try
            {
                PlanningService.Text wiki = con.Planning.getPlanWikiContent(planTemplate.uri);
                con.Planning.setPlanWikiContent(newPlanTemplateURI, wiki);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei wiki Content übernehmen " + e.Message);
                error = e.Message;
                return false;
            }

            try
            {
                con.Planning.updatePlan(newPlanTemplate);
            }
            catch (Exception e)
            {
                Log.Error("Fehler bei updatePlan " + e.Message);
                error = e.Message;
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="con">Polarion Connection Object</param>
        /// <param name="vm">ViewModel PlanInsert</param>
        /// <param name="ActualTemplatePlan">Aktueller Template Plan</param>
        /// <param name="TargetPlanParentId">Unter diesem Plan wird eingefügt</param>
        /// <param name="InsertBaseNode">Plan wir auf oberster Ebene eingefügt (zB.: einfügen Template)</param>
        /// <param name="InsertTemplateTree">true ... Template wird eingefügt </param>
        /// <returns>
        /// true ... Insert OK
        /// false .. Fehler bei Insert
        /// </returns>
        public bool InsertPlans(Connection con, PolarionPlanInsertViewModel vm, Models.Database.Plan ActualTemplatePlan, string TargetPlanParentId, bool InsertBaseNode, bool InsertTemplateTree)
        {
            string ErrorMsg;
            string NewTargetPlanParentId;
            if (InsertBaseNode)
            {
                // Insert Actual Plan
                NewTargetPlanParentId = InsertPlanAndWorkItem(con, vm, ActualTemplatePlan, TargetPlanParentId, out ErrorMsg);
                if (NewTargetPlanParentId == "" && ErrorMsg.Length > 1)
                {
                    // Fehler in Polarion API aufgetreten
                    Log.Error("Error in Polarion API: " + ErrorMsg);
                    vm.ErrorMsg = ErrorMsg;
                    return false;
                }
            }
            else
            {
                NewTargetPlanParentId = ActualTemplatePlan.Plandb.Id;
            }
            // Get Template Childs
            List<Plan> Childs = vm.TemplateTree.Plans.FindAll(p => p.Plandb.Parent == ActualTemplatePlan.Plandb.PK);
            foreach (Plan p in Childs)
            {
                // Neuen Namen des Plans festlegen -> Suffix vom Parent + "A" wenn Iteration-Plan 

                if (!vm.InsertFromTemplateProject)
                {
                    p.SetNewPlanName(vm.NewPlanName, InsertTemplateTree);
                }
                InsertPlanAndWorkItem(con, vm, p, NewTargetPlanParentId, out ErrorMsg);

                if (ErrorMsg.Length > 5)
                {
                    Log.Error("Error2 in Polarion API: " + ErrorMsg);
                    vm.ErrorMsg = ErrorMsg;
                    return false;
                }
                //--------------------------------------------------
                // Rekursiver Aufruf für die untergeordneten Pläne:
                //--------------------------------------------------
                if (InsertPlans(con, vm, p, NewTargetPlanParentId, false, InsertTemplateTree))
                {
                    // Insert OK -> nächster Plan
                }
                else
                {
                    return false;
                }
            }

            // Alles OK
            return true;
        }

        public string GetNewPlanName(Plan p, string NewPlanName)
        {
            if (p.PlanType == Plan.Plantype.I)
            {

            }
            else if (p.PlanType == Plan.Plantype.M)
            {

            }
            else
            {
                // Bei einem Release ist der neue Name aus der Eingabe im Formular
            }
            return "";
        }

        /// <summary>
        /// Speichern der CustomFields [ganttId], [ganttSortorder], [ganttDependencies]
        /// </summary>
        /// <param name="con">Offene connection zu Polarion</param>
        /// <param name="projectId"></param>
        /// <param name="planId"></param>
        /// <param name="ganttId"></param>
        /// <param name="ganttSortorder"></param>
        /// <param name="ganttDependencies"></param>
        /// <returns></returns>
        public bool UpdatePlanCustomFields(Connection con, string projectId, string planId, string ganttId, string ganttSortorder, string ganttDependencies)
        {
            PlanningService.Plan pp;
            try
            {
                pp = con.Planning.getPlanById(projectId, planId);
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdatePlanCF " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            pp.customFields = UpdatePlanCF(pp.customFields, "ganttDependencies", ganttDependencies);
            pp.customFields = UpdatePlanCF(pp.customFields, "ganttId", ganttId);
            pp.customFields = UpdatePlanCF(pp.customFields, "ganttSortorder", ganttSortorder);

            try
            {
                con.Planning.updatePlan(pp);
            }
            catch (Exception ex)
            {
                Log.Error("Error2 UpdatePlanCF " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool UpdatePlanFromGantt(Connection con, string projectId, string planId, GanttData gd)
        {
            PlanningService.Plan pp;

            try
            {
                pp = con.Planning.getPlanById(projectId, planId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            // Property update und check !
            if (gd.BaselineStartDate == null)
            {
                pp.startDate = new DateTime(gd.StartDate.Value.Year, gd.StartDate.Value.Month, gd.StartDate.Value.Day);
                pp.startDateSpecified = true;
                pp.dueDate = new DateTime(gd.EndDate.Value.Year, gd.EndDate.Value.Month, gd.EndDate.Value.Day);
                pp.dueDateSpecified = true;
            }
            else
            {

                pp.startedOn = new DateTime(gd.StartDate.Value.Year, gd.StartDate.Value.Month, gd.StartDate.Value.Day);
                pp.startedOnSpecified = true;
                pp.finishedOn = new DateTime(gd.EndDate.Value.Year, gd.EndDate.Value.Month, gd.EndDate.Value.Day);
                pp.finishedOnSpecified = true;
            }

            // Update Custom Fields:
            pp.customFields = UpdatePlanCF(pp.customFields, "ganttDependencies", gd.Dependency);
            pp.customFields = UpdatePlanCF(pp.customFields, "ganttId", gd.TaskId.ToString());
            pp.customFields = UpdatePlanCF(pp.customFields, "ganttSortorder", gd.Sortorder.ToString());

            try
            {
                con.Planning.updatePlan(pp);
            }
            catch (Exception ex)
            {
                Log.Error("Error updatePlan: " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Das Custom Fields Array wird um key-value erweitert bzw, upgedated 
        /// </summary>
        /// <param name="customFields">CustomField-Array des Plans</param>
        /// <param name="key">Key des zu ändernden Custom Fields</param>
        /// <param name="value">Inhalt des Custom Fields</param>
        /// <returns></returns>
        private PlanningService.Custom[] UpdatePlanCF(PlanningService.Custom[] customFields, string key, string value)
        {
            bool found = false;
            PlanningService.Custom[] newCustomFields = new PlanningService.Custom[0];

            if (value != null && value.Length > 0)
            {
                if (customFields == null || customFields.Count() == 0)
                {
                    //Debug.WriteLine("kein CF vorhanden");
                    PlanningService.Custom cf = new PlanningService.Custom();
                    cf.key = key;
                    cf.value = value;
                    newCustomFields = new PlanningService.Custom[1];
                    newCustomFields[0] = cf;
                }
                else
                {
                    // Es sind bereits Custom-Fields vorhanden 
                    // 1.) Check ob übergebenes CF bereits vorhanden wenn ja -> update
                    foreach (PlanningService.Custom item in customFields)
                    {
                        if (item.key == key)
                        {
                            found = true;
                            item.value = value;
                        }
                    }

                    if (found)
                    {
                        newCustomFields = customFields;
                    }
                    else
                    {
                        // Custum Fields erweitern!
                        newCustomFields = new PlanningService.Custom[customFields.Length + 1];
                        // Bisheriges Array kopieren
                        int i = 0;
                        foreach (PlanningService.Custom item in customFields)
                        {
                            newCustomFields[i] = item;
                            i++;
                        }
                        PlanningService.Custom cf = new PlanningService.Custom();
                        cf.key = key;
                        cf.value = value;
                        newCustomFields[i] = cf;
                    }
                }
            }
            else
            {
                // eventuell eingetragene CustomFields löschen
                if (customFields != null && customFields.Length > 1)
                {
                    newCustomFields = new PlanningService.Custom[customFields.Length - 1];
                }
                else
                {
                    newCustomFields = new PlanningService.Custom[0];
                }

                if (customFields != null)
                {
                    // Alle nicht zu löschenden CustomFields in neues Array übertrgaen
                    int i = 0;
                    foreach (PlanningService.Custom item in customFields)
                    {
                        if (item.key != key)
                        {
                            newCustomFields[i] = item;
                            i++; ;
                        }
                    }
                }
            }

            return newCustomFields;
        }

        public bool UpdateWorkitemFromGantt(Connection con, string projectId, string workitemId, GanttData gd)
        {
            TrackerService.WorkItem w = new TrackerService.WorkItem();
            try
            {
                w = con.Tracker.getWorkItemById(projectId, workitemId);
            }
            catch (Exception ex)
            {
                Log.Error("Error 1 UpdateWorkitemFromGantt " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            // Update Workitemfields
            w.dueDate = new DateTime(gd.EndDate.Value.Year, gd.EndDate.Value.Month, gd.EndDate.Value.Day);
            w.dueDateSpecified = true;

            try
            {
                con.Tracker.updateWorkItem(w);
            }
            catch (Exception ex)
            {
                Log.Error("Error 2 UpdateWorkitemFromGantt " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            // Update Custom Fields:
            if (gd.BaselineStartDate != null)
            {

            }
            else
            {
                // keine baseline Datums
                if (gd.StartDate != null)
                {
                    CreateWorkitemCF(con, w.uri, "startDate", gd.StartDate);
                }
            }

            if (gd.Dependency != null && gd.Dependency.Length > 0)
            {
                CreateWorkitemCF(con, w.uri, "dependencies", gd.Dependency);
            }
            else
            {
                // dependency löschen
                CreateWorkitemCF(con, w.uri, "dependencies", "");
            }

            return true;
        }

        /// <summary>
        /// Ein MS-Project task wird in Polarion gespeichert
        /// </summary>
        /// <param name="con"></param>
        /// <param name="ProjectId"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool UpdatePlan(Connection con, string ProjectId, Task t)
        {
            PlanningService.Plan pp;
            PlanningService.Plan parent;

            try
            {
                pp = con.Planning.getPlanById(ProjectId, t.PlanId);
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdatePlan, PlanId=" + t.PlanId + " " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            if (t.ParentId != null && t.ParentId.Length > 0)
            {
                try
                {
                    parent = con.Planning.getPlanById(ProjectId, t.ParentId);
                }
                catch (Exception ex)
                {
                    Log.Error("Error UpdatePlan, Read Parent PlanId=" + t.ParentId + " " + ex.Message);
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            // Property update und check !

            pp.name = t.WBSCode + " " + t.Name;
            pp.startDate = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
            pp.startDateSpecified = true;
            pp.dueDate = new DateTime(t.Finish.Year, t.Finish.Month, t.Finish.Day);
            pp.dueDateSpecified = true;
            pp.sortOrder = t.ProjectId;
            pp.sortOrderSpecified = true;

            try
            {
                con.Planning.updatePlan(pp);
            }
            catch (Exception ex)
            {
                Log.Error("Error updatePlan, PlanId=" + pp.id + " " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool UpdateSinglePlanSortorder(Connection con, string projectId, TreeSort ts, int sortorder)
        {
            PlanningService.Plan pp = new PlanningService.Plan();
            try
            {
                pp = con.Planning.getPlanById(projectId, ts.Id);
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateSinglePlanSortorder: " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            if (pp.sortOrder != sortorder)
            {
                pp.sortOrder = sortorder;
                pp.sortOrderSpecified = true;

                try
                {
                    con.Planning.updatePlan(pp);
                }
                catch (Exception ex)
                {
                    Log.Error("Error2 UpdateSinglePlanSortorder: " + ex.Message);
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool UpdateSinglePlanDependency(Connection con, string projectId, string planId, string dependency)
        {
            PlanningService.Plan pp = new PlanningService.Plan();
            try
            {
                pp = con.Planning.getPlanById(projectId, planId);
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateSinglePlanDependency: " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }


            if (pp.customFields == null || pp.customFields.Count() == 0)
            {
                //Debug.WriteLine("kein ganttDependencies");
                PlanningService.Custom cf = new PlanningService.Custom();
                cf.key = "ganttDependencies";
                cf.value = dependency;
                pp.customFields = new PlanningService.Custom[1];
                pp.customFields[0] = cf;
            }
            else
            {
                foreach (PlanningService.Custom item in pp.customFields)
                {
                    if (item.key == "ganttDependencies")
                    {
                        item.value = dependency;
                    }
                }
            }

            try
            {
                con.Planning.updatePlan(pp);
            }
            catch (Exception ex)
            {
                Log.Error("Error2 UpdateSinglePlanDependency: " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sortorder in Polarion updaten
        /// </summary>
        /// <param name="con"></param>
        /// <param name="ProjectId"></param>
        /// <param name="gdl"></param>
        /// <returns></returns>
        public bool UpdatePlanSortorder(Connection con, string ProjectId, List<GanttData> gdl)
        {

            PlanningService.Plan pp = new PlanningService.Plan();

            foreach (GanttData gd in gdl)
            {
                try
                {
                    pp = con.Planning.getPlanById(ProjectId, gd.c_id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue; // try next item
                }

                // Property update und check !

                if (pp.sortOrder != gd.Sortorder)
                {
                    pp.sortOrder = gd.Sortorder;
                    pp.sortOrderSpecified = true;

                    try
                    {
                        con.Planning.updatePlan(pp);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }


            return true;
        }

        public bool DeleteWorkitemFromName(Connection con, string ProjectId, Task t)
        {
            TrackerService.WorkItem w = new TrackerService.WorkItem();
            try
            {
                w = con.Tracker.getWorkItemById(ProjectId, t.WorkitemId);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 1 DeleteWorkitemFromName: " + ex.Message);
                return false;
            }
            w.title = t.WBSCode + " " + t.Name;
            try
            {
                con.Tracker.updateWorkItem(w);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 2 DeleteWorkitemFromName: " + ex.Message);
                return false;
            }

            return true;
        }

            public bool UpdateWorkitem(Connection con, string ProjectId, Task t)
        {
            TrackerService.WorkItem w = new TrackerService.WorkItem();
            List<TrackerService.Custom> customFields = new List<TrackerService.Custom>();
            List<TrackerService.Custom> newCustomFields = new List<TrackerService.Custom>();
            bool existentField = false;

            try
            {
                w = con.Tracker.getWorkItemById(ProjectId, t.WorkitemId);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 1 UpdateWorkitem: " + ex.Message);
                return false;
            }

            // Update Workitemfields
            w.dueDate = new DateTime(t.Finish.Year, t.Finish.Month, t.Finish.Day);
            w.dueDateSpecified = true;

            //w.plannedStart = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day); -> False field, the one we want is custom field "startDate"
            //w.plannedStartSpecified = true;

            //custom field id: startDate name: Start Date
            TrackerService.Custom[] existentCustomFields = w.customFields;

            if (!(existentCustomFields == null || existentCustomFields.Length == 0))
            {
                for (int i = 0; i < existentCustomFields.Length; i++)
                {
                    if (existentCustomFields[i].key.Equals("startDate") && t.Start != null)
                    {
                        existentCustomFields[i].value = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
                        existentField = true;
                    }
                }
                customFields.AddRange(existentCustomFields);
            }
            if (!existentField)
            {
                if (t.Start != null)
                {
                    var customF = new TrackerService.Custom();
                    customF.key = "startDate";
                    customF.value = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
                    newCustomFields.Add(customF);
                }
            }

            // custom field id: wprole name: Assigned Role   -- just for the first time?
            //if (!string.IsNullOrWhiteSpace(t.PtRole))
            //{
            //    var customF2 = new TrackerService.Custom();
            //    customF2.key = "wprole";
            //    TrackerService.EnumOptionId rolesEnum = new TrackerService.EnumOptionId();
            //    rolesEnum.id = "allocationRole_" + t.PtRole;
            //    customF2.value = rolesEnum;
            //    newCustomFields.Add(customF2);
            //}

            var customFieldkeys = con.Tracker.getDefinedCustomFieldKeys​(ProjectId, w.type.id);
            for (int i = 0; i < customFieldkeys.Length; i++)
            {
                for (int j = 0; j < newCustomFields.Count; j++)
                {
                    if (customFieldkeys[i].Equals(newCustomFields[j].key))
                    {
                        customFields.Add(newCustomFields[j]);
                    }
                }
            }
            if (customFields.Count > 0)
            {
                w.customFields = customFields.ToArray();
            }

            w.title = t.WBSCode + " " + t.Name;
            try
            {
                con.Tracker.updateWorkItem(w);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 2 UpdateWorkitem: " + ex.Message);
                return false;
            }

            return true;
        }

        public bool UpdateFromDeletedWorkitem(Connection con, string ProjectId, Task t, string deletedWiId)
        {
            TrackerService.WorkItem w = new TrackerService.WorkItem();
            TrackerService.WorkItem deletedW = new TrackerService.WorkItem();
            
            try
            {
                deletedW = con.Tracker.getWorkItemById(ProjectId, deletedWiId);
                w = con.Tracker.getWorkItemById(ProjectId, t.WorkitemId);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 1 UpdateFromDeletedWorkitem: " + ex.Message);
                return false;
            }

            try
            {
                //Following not necessary as these are taken from MS-P
                // Update Workitemfields
                //w.dueDate = deletedW.dueDate;
                //w.dueDateSpecified = true;
                //w.plannedStart = deletedW.plannedStart;
                //w.plannedStartSpecified = true;
                w.dueDate = new DateTime(t.Finish.Year, t.Finish.Month, t.Finish.Day);
                w.dueDateSpecified = true;

                w.severity = deletedW.severity;
                w.author = deletedW.author;
                w.initialEstimate = deletedW.initialEstimate;
                w.timeSpent = deletedW.timeSpent;
                w.remainingEstimate = deletedW.remainingEstimate;
                w.status = deletedW.status;
                w.resolution = deletedW.resolution;
                w.priority = deletedW.priority;
                w.description = deletedW.description;
                w.workRecords = deletedW.workRecords;
                w.approvals = deletedW.approvals;
                w.comments = deletedW.comments;
                w.hyperlinks = deletedW.hyperlinks;
                w.attachments = deletedW.attachments;

                w.customFields = deletedW.customFields; //Start date overwritten here?

                TrackerService.User[] users = deletedW.assignee;
                foreach (TrackerService.User user in users)
                {
                    con.Tracker.addAssignee(w.uri, user.id);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 1 UpdateFromDeletedWorkitem: " + ex.Message);
                return false;
            }
            try
            {
                con.Tracker.updateWorkItem(w);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - 2 UpdateFromDeletedWorkitem: " + ex.Message);
                return false;
            }

            return true;
        }
        /// <summary>
        /// Methode für MS-Project Sync
        /// </summary>
        /// <param name="con"></param>
        /// <param name="ProjectId"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        //public bool InsertPlan(Connection con, string ProjectId, string ParentWorkitem_c_id, Task t, string baseplanId, out string error)
        public Tuple<bool, Task, string> InsertPlan(Connection con, string ProjectId, string ParentWorkitem_c_id, Task t, string baseplanId, out string error)
        {
            error = "";
            string PlanURI;
            string WorkitemURI;
            string TemplateId = "release";  // derzeit Fix auf Release
            string NewPlanId;

            ProjectService.Project PolarionTargetProject;
            TrackerService.WorkItem NewWorkitem;
            TrackerService.WorkItem ParentWorkitem;

            PlanningService.Plan plan;
            List<string> Workitems = new List<string>();

            TrackerService.EnumOptionId eoType = new TrackerService.EnumOptionId();
            TrackerService.EnumOptionId eoSeverity = new TrackerService.EnumOptionId();
            TrackerService.PriorityOptionId eoPriority = new TrackerService.PriorityOptionId();
            TrackerService.EnumOptionId eoStatus = new TrackerService.EnumOptionId();
            TrackerService.PriorityOptionId poId = new TrackerService.PriorityOptionId();
            TrackerService.CustomField cf = new TrackerService.CustomField();
            try
            {
                PolarionTargetProject = con.Project.getProject(ProjectId);
            }
            catch (Exception e)
            {
                Log.Error("Error InsertPlan getProject: " + e.Message);
                error = e.Message;
                return Tuple.Create(false,t,error);
            }
            if (t.Level <= Polarion.MaxDeepPlan)
            {
                PlanningService.Plan NewPlan;
                if (t.WBSCode.Length == 1)
                {
                    NewPlanId = baseplanId; // Der Wurzelplan selbst wird eingefügt
                }
                else
                {
                    NewPlanId = baseplanId + "_" + t.WBSCode.Replace('.', '_');
                }


                string NewPlanName = t.WBSCode + " " + t.Name;
                NewPlan = null;

                // Überprüfen ob bereits ein Plan mit dieser ID vorhanden ist ?
                try
                {
                    NewPlan = con.Planning.getPlanById(ProjectId, NewPlanId);
                }
                catch (Exception e)
                {
                    Log.Error("Error InsertPlan getPlanById: " + e.Message);
                    error = e.Message;
                    return Tuple.Create(false, t, error);
                }

                if (NewPlan.location != null)  //Relevant ??
                {
                    NewPlanId += "1";
                }

                // Plan erzeugen:
                try
                {
                    if (string.IsNullOrEmpty(t.ParentPlanId)) t.ParentPlanId = null;
                    PlanURI = con.Planning.createPlan(ProjectId, NewPlanName, NewPlanId, t.ParentPlanId, TemplateId);
                    NewPlan = con.Planning.getPlanByUri(PlanURI);

                }
                catch (Exception e)
                {
                    Log.Error("Error InsertPlan createPlan: " + e.Message);
                    error = e.Message;
                    return Tuple.Create(false, t, error);
                }

                // Update Plan Properties
                NewPlan.startDate = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
                NewPlan.startDateSpecified = true;
                NewPlan.dueDate = new DateTime(t.Finish.Year, t.Finish.Month, t.Finish.Day);
                NewPlan.dueDateSpecified = true;
                NewPlan.sortOrder = t.ProjectId;
                NewPlan.sortOrderSpecified = true;

                // Update Task
                t.PlanId = NewPlan.id;

                try
                {
                    con.Planning.updatePlan(NewPlan);
                }
                catch (Exception ex)
                {
                    Log.Error("Error InsertPlan updatePlan: " + ex.Message);
                    error = ex.Message;
                    return Tuple.Create(false, t, error);
                }
                plan = NewPlan;
            }
            else
            {   // Bestehenden Plan verknüpfen
                plan = con.Planning.getPlanById(ProjectId, t.ParentPlanId);
                PlanURI = plan.uri;
            }

            // Verbundenes WorkPackage erzeugen:

            NewWorkitem = new TrackerService.WorkItem();

            NewWorkitem.project = new TrackerService.Project();
            NewWorkitem.project.uri = PolarionTargetProject.uri;

            NewWorkitem.dueDate = new DateTime(t.Finish.Year, t.Finish.Month, t.Finish.Day);
            NewWorkitem.dueDateSpecified = true;

            NewWorkitem.title = t.WBSCode + " " + t.Name;

            eoType.id = "workPackage";  // AH: workPackage statt projectTask
            NewWorkitem.type = eoType;

            eoSeverity.id = "must_have";
            NewWorkitem.severity = eoSeverity;

            eoPriority.id = "50.0";
            NewWorkitem.priority = eoPriority;

            eoStatus.id = "open";
            NewWorkitem.status = eoStatus;

            //custom field id: startDate name: Start Date
            List<TrackerService.Custom> customFields = new List<TrackerService.Custom>();
            TrackerService.Custom[] existentCustomFields = NewWorkitem.customFields;
            List<TrackerService.Custom> newCustomFields = new List<TrackerService.Custom>();
            bool existentField = false;

            if (!(existentCustomFields == null || existentCustomFields.Length == 0))
            {
                for (int i = 0; i < existentCustomFields.Length; i++)
                {
                    if (existentCustomFields[i].key.Equals("startDate"))
                    {
                        existentCustomFields[i].value = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
                        existentField = true;
                    }
                }
                customFields.AddRange(existentCustomFields);
            }
            if(!existentField)
            {
                if (t.Start != null)
                {
                    var customF = new TrackerService.Custom();
                    customF.key = "startDate";
                    customF.value = new DateTime(t.Start.Year, t.Start.Month, t.Start.Day);
                    newCustomFields.Add(customF);
                }
            }
            // custom field id: wprole name: Assigned Role
            if (!string.IsNullOrWhiteSpace(t.PtRole))
            {
                var customF2 = new TrackerService.Custom();
                customF2.key = "wprole";
                TrackerService.EnumOptionId rolesEnum = new TrackerService.EnumOptionId();
                rolesEnum.id = "allocationRole_" + t.PtRole;
                customF2.value = rolesEnum;
                newCustomFields.Add(customF2);

            }
            var customFieldkeys = con.Tracker.getDefinedCustomFieldKeys​(ProjectId, NewWorkitem.type.id);
            //TO DO: CHECK WHATS BEING PASSED ARE ACTUALLY CUSTTOM FIELDS IN THE POLARION ENUM OPTIONS
            for (int i = 0; i < customFieldkeys.Length; i++)
            {
                for (int j = 0; j < newCustomFields.Count; j++)
                {
                    if (customFieldkeys[i].Equals(newCustomFields[j].key))
                    {
                        //check datatype is enum to do this, else add normally
                        //if (newCustomFields[j].value.GetType().Equals(typeof(TrackerService.EnumOptionId))) 
                        //{
                        //    var enumOptionIds = con.Tracker.getEnumOptionsForId​(NewWorkitem.uri, newCustomFields[j].key); //Error in this method
                            
                        //    if (Array.Exists(enumOptionIds, element => element == newCustomFields[j].value)) 
                        //    {
                                customFields.Add(newCustomFields[j]);
                        //    }
                        //}
                    }
                }
            }
            if (customFields.Count > 0)
            {
                NewWorkitem.customFields = customFields.ToArray();
            }

            try
            {
                WorkitemURI = con.Tracker.createWorkItem(NewWorkitem);
                NewWorkitem = con.Tracker.getWorkItemByUri(WorkitemURI);

                //for (int i = 0; customFields.Count > 0 && i < customFields.Count; i++)
                //{
                //    var customFieldkey = customFields[i].key;
                //    var cField= con.Tracker.getCustomField(WorkitemURI, customFieldkey);
                //    cField.value = customFields[i].value;
                //    cField.key = customFieldkey;
                //    con.Tracker.setCustomField(cField);
                //}

                t.WorkitemId = NewWorkitem.id;
                UpdateWorkitem(con, ProjectId, t);
            }
            catch (Exception ex)
            {
                Log.Error("Error InsertPlan createWorkItem: " + ex.Message);
                error = ex.Message;
                return Tuple.Create(false, t, error);
            }

            // Custom Fields setzen:
            /*
            cf.parentItemURI = WorkitemURI;
            cf.key = "";
            cf.value = "";
            con.Tracker.setCustomField(cf);
            */
            Workitems.Add(WorkitemURI);

            if (Workitems.Count > 0)
            {
                try
                {
                    con.Planning.addPlanItems(PlanURI, Workitems.ToArray());
                }
                catch (Exception ex)
                {
                    Log.Error("Error InsertPlan addPlanItems: " + ex.Message);
                    error = ex.Message;
                    return Tuple.Create(false, t, error);
                }
            }

            // @@@ Workitem mit darüber liegendem Workitem verbinden
            if (!string.IsNullOrEmpty(ParentWorkitem_c_id))
            {
                // parent Workitem vorhanden -> link erzeugen
                try
                {
                    ParentWorkitem = con.Tracker.getWorkItemById(ProjectId, ParentWorkitem_c_id);
                    eoStatus.id = "parent";

                    con.Tracker.addLinkedItem(WorkitemURI, ParentWorkitem.uri, eoStatus);
                }
                catch (Exception ex)
                {
                    Log.Error("Error InsertPlan getWorkItemById: " + ex.Message);
                    error = ex.Message;
                    return Tuple.Create(false, t, error);
                }
            }
            return Tuple.Create(true, t, error);
        }

        /// <summary>
        /// Methode für MS-Project:
        /// fügt einen projecttask ein. Dieses Workitem ist mit einem Plan und mit einem darüberliegenden project task verknüpft.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="t"></param>
        /// <param name="parentPlan"></param>
        /// <param name="parentWorkitem"></param>
        /// <returns></returns>
        public bool CreateWorkitem(Connection con, Task t, string parentPlan, string parentWorkitem)
        {
            return true;
        }

        public bool CreateWorkitemCF(Connection con, string projectId, string workitemId, string cfName, object cfValue)
        {
            TrackerService.WorkItem w = new TrackerService.WorkItem();
            try
            {
                w = con.Tracker.getWorkItemById(projectId, workitemId);
            }
            catch (Exception ex)
            {
                Log.Error("Error 1 CreateWorkitemCF " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return CreateWorkitemCF(con, w.uri, cfName, cfValue);
        }

        public bool CreateWorkitemCF(Connection con, string WorkitemURI, string CFname, object CFvalue)
        {
            TrackerService.CustomField cf = new TrackerService.CustomField();
            cf.parentItemURI = WorkitemURI;
            cf.key = CFname;
            cf.value = CFvalue;
            try
            {
                con.Tracker.setCustomField(cf);
            }
            catch (Exception ex)
            {
                Log.Error("Error 1 CreateWorkitemCF URI " + ex.Message);
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool RemoveLinkedWorkitem(Connection con, string ProjectId, string WorkitemId, string LinkedWorkitemid, string Role)
        {
            TrackerService.EnumOptionId eoId = new TrackerService.EnumOptionId();
            eoId.id = Role;
            try
            {
                TrackerService.WorkItem w = con.Tracker.getWorkItemById(ProjectId, WorkitemId);
                TrackerService.WorkItem wl = con.Tracker.getWorkItemById(ProjectId, LinkedWorkitemid);
                con.Tracker.removeLinkedItem(w.uri, wl.uri, eoId);
            }
            catch (Exception ex)
            {
                Log.Error("Polarion - RemoveLinkedWorkitem: " + ex.Message);
                return false;
            }

            return true;
        }

        //To check if work item was deleted before but it is still present in MS proj -> means it was (copy-cut-pasted)
        public bool IsDeletedWorkItem(Connection con, string ProjectId, string WorkitemId) //need to return id?
        {
            TrackerService.WorkItem w;
            bool isDeleted = false;
            try
            {
                w = con.Tracker.getWorkItemById(ProjectId, WorkitemId);
                if (w.id == null || w.title == null)
                {
                    Log.Debug("EXCEPTION WorkItem ID in MS Proj, not found in Polarion " + WorkitemId);
                    Debug.WriteLine("EXCEPTION WorkItem ID in MS Proj, not found in Polarion "+ WorkitemId);
                    return false;
                }
                isDeleted = w.id.Equals(WorkitemId) && w.title.Contains("[deleted]");
            }
            catch (Exception ex)
            {
                Log.Debug("EXCEPTION WorkItem ID in MS Proj, not found in Polarion " + ex.Message+" "+WorkitemId);
                Debug.WriteLine("EXCEPTION WorkItem ID in MS Proj, not found in Polarion " + ex.Message+" "+WorkitemId);
                return false; 
            }
            return isDeleted;
        }

        /// <summary>
        /// löscht alle Verbindungen des Plans zu workitems
        /// </summary>
        /// <param name="con">Polarion Connection (logged in)</param>
        /// <param name="planId">Uri des Plans (gleicher Wert wie c_pk)</param>
        /// <returns></returns>
        public bool RemovePlanItem(Connection con, string projectId, string planId, string workitemId)
        {
            PlanningService.Plan plan = con.Planning.getPlanById(projectId, planId);
            TrackerService.WorkItem workitem = con.Tracker.getWorkItemById(projectId, workitemId);

            string[] toDelete = { workitem.uri };
            // Array aus WorkitemURI erstellen
            try
            {
                con.Planning.removePlanItems(plan.uri, toDelete);
            }
            catch (Exception e)
            {
                Log.Error("Error RemovePlanItem planId=" + planId + " " + e.Message);
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public void TestWorkitem(Connection con)
        {
            TrackerService.EnumOptionId status = new TrackerService.EnumOptionId();
            TrackerService.WorkItem Workitem;
            TrackerService.Text text;
            TrackerService.PriorityOptionId poId;

            status.id = "open";

            Workitem = con.Tracker.getWorkItemById("PMArena", "PMA-3083");
            status = Workitem.status;
            text = Workitem.description;
            poId = Workitem.priority;
        }

        public void TestTestrun(Connection con)
        {
            TestrunService.TestRun testrun;

           
        }
    }
}