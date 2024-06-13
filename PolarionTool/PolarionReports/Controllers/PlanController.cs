using PolarionReports.BusinessLogic;
using PolarionReports.Custom_Helpers;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PolarionReports.Models.Database.Plan;

namespace PolarionReports.Controllers
{
    public class PlanController : Controller
    {
        //TreeViewFieldsSettings PlanFields = new TreeViewFieldsSettings();
        //TreeViewFieldsSettings TempFields = new TreeViewFieldsSettings();

        // GET: Plan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Insert(string id)
        {
            DatareaderP dr;
            string ErrorMsg = "";
            string Error;
            int ProjectPK;

            // Project einlesen

            PlanViewModel pv = new PlanViewModel();
            if (!int.TryParse(id, out ProjectPK))
            {
                // parameter ist nicht numerisch -> Versuch das Project über den namen zu lesen
            }
            
            dr = new DatareaderP();
            if (ProjectPK == 0)
            {
                pv.Project = dr.GetProjectByID(id, out Error);
                ErrorMsg += Error;
                ProjectPK = pv.Project.C_pk;
            }
            else
            {
                pv.Project = dr.GetProjectByPK(ProjectPK, out Error);
                ErrorMsg += Error;
            }

            // Plans einlesen
            pv.ProjectPlans = dr.GetAllPlansFromProject(ProjectPK, out Error);
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

            pv.ProjectId = pv.Project.Id;

            return View(pv);
        }

        [HttpPost]
        public ActionResult Insert([Bind] PlanViewModel pv)
        {
            PlanBL planBL = new PlanBL();
            Connection con;
            bool InsertTemplate = false;
            string Error;
            Plantype NewPlantype;

            // Fill the ViewModel from Database
            planBL.InitPv(pv);

            if (pv.Username == null ||
                pv.Password == null ||
                pv.NewPlanName == null)
            {
                pv.ErrorMsg = "Please fill the required fiels (New plan name, user, password) ";
                return View(pv);
            }

            // Check if a Plan with the new Name already exist in the Target Project
            Plan Check = pv.ProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == pv.NewPlanName);
            if (Check != null)
            {
                pv.ErrorMsg = "A plan with the new name already exists: " + Check.Plandb.Id;
                return View(pv);
            }

            if (pv.InsertAction == "InsertTemplate")
            {
                // Template aus Templateproject = Source
                Check = pv.TemplatePlans.Plans.FirstOrDefault(p => p.Plandb.Id == "Template");
                if (Check != null)
                {
                    pv.TemplatePlanPK = Check.Plandb.PK.ToString();
                    pv.TargetPlanPK = "0";
                    InsertTemplate = true;
                }
                else
                {
                    pv.ErrorMsg = "Template Plan not found in Template Project";
                    return View(pv);
                }
            }
            else if (pv.InsertAction == "InsertRelease")
            {
                // Insert Release
                Check = pv.ProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == "SW_Release_Template");
                NewPlantype = Plantype.R;
                if (Check != null)
                {
                    if (Check.CheckPlanName(pv.NewPlanName, NewPlantype, out Error))
                    {
                        pv.ErrorMsg = Error;
                        return View(pv);
                    }
                    pv.TemplatePlanPK = Check.Plandb.PK.ToString();
                    InsertTemplate = false;
                }
                else
                {
                    pv.ErrorMsg = "Template Plan 'SW_Release_Template' not found in Project";
                    return View(pv);
                }
            }
            else if (pv.InsertAction == "InsertIteration")
            {
                // Insert Iteration - Get Iteration in Template Tree from the Project
                Check = pv.ProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == "SW_Iteration_Template");
                NewPlantype = Plantype.I;
                if (Check != null)
                {
                    if (Check.CheckPlanName(pv.NewPlanName, NewPlantype, out Error ))
                    {
                        pv.ErrorMsg = Error;
                        return View(pv);
                    }
                    pv.TemplatePlanPK = Check.Plandb.PK.ToString();
                    // pv.TargetPlanPK = "0";
                    InsertTemplate = false;
                }
                else
                {
                    pv.ErrorMsg = "Template Plan 'SW_Iteration_Template' not found in Project";
                    return View(pv);
                }
            }
            else if (pv.InsertAction == "UpdateMilestone")
            {
                // Update Milestone in einem Plan (nicht im Template erlaubt)
            }
            else if (pv.InsertAction == "UpdateTemplate")
            {
                // Update Template:
                // Bestehendes Template löschen -> Alle Workitems im Titel mit "[delete]" versehen
                // Insert Template aufrufen
            }
            else
            {
                // unknown Action
            }

            Polarion Polarion = new Polarion();

            con = new Connection("https://", Topol.PolarionServer + "/polarion/");
            try
            {
                con.Login(pv.Username, pv.Password);
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                pv.ErrorMsg = e.ToString();
                return View(pv);
            }

            if (!con.IsLoggedIn)
            {
                pv.ErrorMsg = "Login failed!";
                return View(pv);
            }

            PolarionPlanInsertViewModel pivm = new PolarionPlanInsertViewModel(pv.TemplatePlanPK, pv.TargetPlanPK, pv.ProjectId);
            Plan BaseNodePlan = pivm.TemplateTree.Plans[0]; // BaseNode des einzufügenden Trees
            BaseNodePlan.Plandb.Id = pv.NewPlanName;
            BaseNodePlan.Plandb.Name = pv.NewPlanName;
            pivm.NewPlanName = pv.NewPlanName;

            if (pv.TargetPlanPK == "0")
            {
                Polarion.InsertPlans(con, pivm, BaseNodePlan, "0", true, InsertTemplate);
            }
            else
            {
                Polarion.InsertPlans(con, pivm, BaseNodePlan, pivm.TargetPlan.Plandb.Id, true, InsertTemplate);
            }

            pv.NewPlanName = "";
            // Pläne neu einlesen
            planBL.InitPv(pv);
            return View(pv);
        }

        public ActionResult InsertFromTemplateProject (string id)
        {
            DatareaderP dr;
            string ErrorMsg = "";
            string Error;
            int ProjectPK;

            // Project einlesen

            PlanViewModel pv = new PlanViewModel();
            if (!int.TryParse(id, out ProjectPK))
            {
                // parameter ist nicht numerisch -> Versuch das Project über dan namen zu lesen
            }

            dr = new DatareaderP();
            if (ProjectPK == 0)
            {
                pv.Project = dr.GetProjectByID(id, out Error);
                ErrorMsg += Error;
                ProjectPK = pv.Project.C_pk;
            }
            else
            {
                pv.Project = dr.GetProjectByPK(ProjectPK, out Error);
                ErrorMsg += Error;
            }

            // Plans einlesen
            pv.ProjectPlans = dr.GetAllPlansFromProject(ProjectPK, out Error);
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

            pv.ProjectId = pv.Project.Id;

            return View(pv);
        }

        [HttpPost]
        public ActionResult InsertFromTemplateProject([Bind] PlanViewModel pv)
        {
            PlanBL planBL = new PlanBL();
            Connection con;

            // Fill the ViewModel from Database
            planBL.InitPv(pv);

            if (pv.Username == null ||
                pv.Password == null )
            {
                pv.ErrorMsg = "Please fill the required fields (user, password) ";
                return View(pv);
            }

            // Check if a Plan with the new Name already exist in the Target Project
            Plan Check = pv.ProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == pv.TemplatePlanId);
            if (Check != null)
            {
                pv.ErrorMsg = "A plan with the name already exists: " + Check.Plandb.Id;
                return View(pv);
            }

            Polarion Polarion = new Polarion();

            con = new Connection("https://", Topol.PolarionServer + "/polarion/");
            try
            {
                con.Login(pv.Username, pv.Password);
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                pv.ErrorMsg = e.ToString();
                return View(pv);
            }

            if (!con.IsLoggedIn)
            {
                pv.ErrorMsg = "Login failed!";
                return View(pv);
            }

            PolarionPlanInsertViewModel pivm = new PolarionPlanInsertViewModel(pv.TemplatePlanPK, pv.TargetPlanPK, pv.ProjectId);
            Plan BaseNodePlan = pivm.TemplateTree.Plans[0]; // BaseNode des einzufügenden Trees
            BaseNodePlan.Plandb.Id = pv.TemplatePlanId;
            BaseNodePlan.Plandb.Name = pv.NewPlanName;
            pivm.NewPlanName = pv.NewPlanName;
            pivm.InsertFromTemplateProject = true;

            Polarion.InsertPlans(con, pivm, BaseNodePlan, "0", true, true);
            Debug.WriteLine(pivm.ErrorMsg);
            pv.NewPlanName = "";
            // Pläne neu einlesen
            planBL.InitPv(pv);
            pv.ErrorMsg = pivm.ErrorMsg;

            if (string.IsNullOrWhiteSpace(pv.ErrorMsg))
            {
                pv.Message = "Plan insert sucessful!";
            }

            return View(pv);
        }
    }
}