using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.BusinessLogic;
using TrackerService = net.seabay.polarion.Tracker;
using ProjectService = net.seabay.polarion.Project;
using PlanningService = net.seabay.polarion.Planning;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolarionReports.BusinessLogic.Api;
using PolarionReports.Models.MSProjectApi;
using System.Diagnostics;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        /// <summary>
        /// Dieser Controller dient für allgemeine Testzwecke
        /// Test für SVN
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            TestViewModel tvm = new TestViewModel();
            DatareaderP dr = new DatareaderP();
            // List<WorkitemCustomField> wcfl = dr.GetAllCustomFieldsFromWorkitem("PMA-3169", out string Error);

            Task t = new Task();
            PolarionInit pi = new PolarionInit();
            Polarion po = new Polarion();

            Connection con = pi.Init("wnpolrose", "wnPOL%18");
            PlanningService.Plan pp = new PlanningService.Plan();
            PlanningService.Custom cf;
            List<PlanningService.Custom> cfl;

            try
            {
                pp = con.Planning.getPlanById("ZKW_Demo_Project", "SW_21");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // Property update und check !

            pp.sortOrder = 1;
            pp.sortOrderSpecified = true;

            if (pp.customFields == null || pp.customFields.Count() == 0)
            {
                Debug.WriteLine("kein ganttDependencies");
                cf = new PlanningService.Custom();
                cf.key = "ganttDependencies";
                cf.value = "Neuer Inhalt";
                pp.customFields = new PlanningService.Custom[1];
                pp.customFields[0] = cf;
            }
            else
            {
                foreach (PlanningService.Custom item in pp.customFields)
                {
                    Debug.WriteLine(item.key);
                    Debug.WriteLine(item.value);
                    if (item.key == "ganttDependencies")
                    {
                        item.value = "Was lustiges geändert!";
                    }
                }
            }
            try
            {
                con.Planning.updatePlan(pp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return View(tvm);
        }

        [HttpPost]
        public ActionResult Index([Bind] TestViewModel tvm)
        {
            return View(tvm);
        }

        public ActionResult Workitem()
        {
            TestViewModel tvm = new TestViewModel();
            Polarion Polarion = new Polarion();
            Connection con;
            string[] CFKeys;
            string[] AllCFKeys;

            string ProjectId = "ZKW_Demo_Project";
            string WorkitemId = "ZKWD-15360";

            ProjectId = "Template";
            WorkitemId = "TEMP-2565";

            TrackerService.WorkItem NewWorkitem;
            ProjectService.Project PolarionTargetProject;

            con = new Connection("https://", $"{Topol.PolarionServer}/polarion/");
            try
            {
                con.Login("wnpolrose", "wnPOL%18");
            }
            catch (Exception e)
            {
                ErrorViewModel error = new ErrorViewModel();
                tvm.Message = e.ToString();
                return View(tvm);
            }

            if (!con.IsLoggedIn)
            {
                tvm.Message = "Login failed!";
                return View(tvm);
            }

            TrackerService.WorkItem w = con.Tracker.getWorkItemById(ProjectId, WorkitemId);
            CFKeys = con.Tracker.getCustomFieldKeys(w.uri);
            TrackerService.CustomFieldType[] types = con.Tracker.getCustomFieldTypes(w.uri);

            AllCFKeys = con.Tracker.getDefinedCustomFieldKeys(ProjectId, "milestoneCheck");

            TrackerService.CustomField cf = con.Tracker.getCustomField(w.uri, "milestoneType");

            tvm.Message = "no Polarion Error";
            return View(tvm);
        }
    }
}