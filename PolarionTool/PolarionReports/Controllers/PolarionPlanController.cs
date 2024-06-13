using PolarionReports.Models;
using PolarionReports.Models.Database;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using net.seabay.polarion.Project;
using System.Diagnostics;
using PolarionReports.BusinessLogic;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class PolarionPlanController : Controller
    {
        // GET: PolarionPlan
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Insert(string source, string target)
        {
            int SourcePK = 0;
            int TargetPK = 0;

            int.TryParse(source, out SourcePK);
            int.TryParse(target, out TargetPK);

            // wird nicht mehr verwendet
            PolarionPlanInsertViewModel vm = new PolarionPlanInsertViewModel(source, target, "0");

            PlanTreeviewModel TemplateTree = new PlanTreeviewModel();
            vm.TemplateFields = new TreeViewFieldsSettings();
            vm.TemplateFields.DataSource = TemplateTree.GetPlanTreeviewModel(vm.TemplateTree, vm.TemplatePlan.Plandb.PK, false, vm.TemplateProjectPlans);
            vm.TemplateFields.HasChildren = "HasChild";
            vm.TemplateFields.Expanded = "Expanded";
            vm.TemplateFields.Id = "PK";
            vm.TemplateFields.ParentID = "PId";
            vm.TemplateFields.Text = "Name";
            vm.TemplateFields.ImageUrl = "Iconurl";
            vm.ErrorMsg = "";

            return View(vm);
        }

        public ActionResult Delete(string ProjectId, string PlanId)
        {

            return View();
        }

        [HttpPost]
        public ActionResult Insert([Bind] PolarionPlanInsertViewModel vm)
        {

            Debug.WriteLine(vm.Username);
            Debug.WriteLine(vm.Password);
            Debug.WriteLine(vm.TemplatePlanPK + " / " + vm.TargetPlanPK);
            Debug.WriteLine(vm.NewPlanName);
            Connection con;

            PolarionPlanInsertViewModel vmDB = new PolarionPlanInsertViewModel(vm.TemplatePlanPK, vm.TargetPlanPK, vm.TargetProject.Id);
            PlanTreeviewModel TemplateTree = new PlanTreeviewModel();
            vmDB.TemplateFields = new TreeViewFieldsSettings();
            vmDB.TemplateFields.DataSource = TemplateTree.GetPlanTreeviewModel(vmDB.TemplateTree, vmDB.TemplatePlan.Plandb.PK, false, vmDB.TemplateProjectPlans);
            vmDB.TemplateFields.HasChildren = "HasChild";
            vmDB.TemplateFields.Expanded = "Expanded";
            vmDB.TemplateFields.Id = "PK";
            vmDB.TemplateFields.ParentID = "PId";
            vmDB.TemplateFields.Text = "Name";
            vmDB.TemplateFields.ImageUrl = "Iconurl";
            vmDB.ErrorMsg = "";

            Plan BaseNodePlan = vmDB.TemplateTree.Plans[0];

            Polarion Polarion = new Polarion();

            if (vm.Username == null ||
                vm.Password == null ||
                vm.NewPlanName == null)
            {
                vmDB.ErrorMsg = "Please fill the required fields (New plan name, user, password) ";
                return View(vmDB);
            }

            /*
            if (!ModelState.IsValid)
            {
                vmDB.ErrorMsg = "Please fill the required fiels (New plan name, user, password) ";
                return View(vmDB);
            }
            */

            vmDB.NewPlanName = vm.NewPlanName;
            BaseNodePlan.Plandb.Id = vm.NewPlanName;
            BaseNodePlan.Plandb.Name = vm.NewPlanName;
            // Check new PlanName
            if (BaseNodePlan.CheckPlanName(vm.NewPlanName, BaseNodePlan.PlanType, out string CheckMsg))
            {
                vmDB.ErrorMsg = CheckMsg;
                return View(vmDB);
            }

            // Check if a Plan with the new Name already exist in the Target Project
            Plan Check = vmDB.TargetProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == vm.NewPlanName);
            if (Check != null)
            {
                vmDB.ErrorMsg = "A plan with the new name already exists: " + Check.Plandb.Id;
                return View(vmDB);
            }

            con = new Connection("https://", $"{Topol.PolarionServer}/polarion/");
            try
            {
                con.Login(vm.Username, vm.Password);
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                vmDB.ErrorMsg = e.ToString();
                return View(vmDB);
            }

            if (!con.IsLoggedIn)
            {
                vmDB.ErrorMsg = "Login failed!";
                return View(vmDB);
            }

            // Alle Pläne werden unter dem TargetPlan eingefügt

            //Polarion.TestNewWorkitem(con);

            Polarion.InsertPlans(con, vmDB, BaseNodePlan, vmDB.TargetPlan.Plandb.Id, true, false);

            vmDB.Ready = true;
            return View(vmDB);
        }
    }
}