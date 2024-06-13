using PolarionReports.Models;
using PolarionReports.Models.Database;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Serilog;

namespace PolarionReports.Controllers
{
    public class HomeController : Controller
    {
        TreeViewFieldsSettings fields = new TreeViewFieldsSettings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// History:
        /// 2020-07-06 Close Connection eingebaut
        /// </remarks>
        public ActionResult Index()
        {
            DatareaderP dr;
            string ErrorMsg = "";
            string Error;
            string ip = Request.UserHostAddress;

            Log.Debug(ip + " Home Controller Index");

            if (Syncfusion.Licensing.SyncfusionLicenseProvider.ValidateLicense(Syncfusion.Licensing.Platform.ASPNETMVC))
            {
                Debug.WriteLine("Syncfusion Lizenz OK");
            }
            else
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTQzNjU1QDMxMzcyZTMyMmUzMElTMVZrUnZSekxUa2dkOEhCdHFHOC8rQVBXM3lleUpIc0tseEltQklPV2c9");
            }

            // 17.2.0.  MTQzNjU1QDMxMzcyZTMyMmUzMElTMVZrUnZSekxUa2dkOEhCdHFHOC8rQVBXM3lleUpIc0tseEltQklPV2c9 
            // 16.2.0.* MTQ0MjJAMzEzNjJlMzIyZTMwRG51WFVIY3BwcUF4MXhyTnlGWmJ0ZzVuTzZyS2NZSU1aUHM1a0d3dWZSRT0=

            if (Syncfusion.Licensing.SyncfusionLicenseProvider.ValidateLicense(Syncfusion.Licensing.Platform.ASPNETMVC))
            {
                Debug.WriteLine("Syncfusion Lizenz OK");
            }

            HomeView hv = new HomeView();
            hv.Projectgroups = new ProjectgroupList();
            hv.Projects = new ProjectDBList();
          
            dr = new DatareaderP();
            hv.Projectgroups.List = dr.GetAllProjectGroups(out Error);
            if (Error != null && Error != "GetAllProjectGroups-OK") ErrorMsg = Error + ", ";
            
            hv.Projects.List = dr.GetAllProjects(out Error);
            if (Error != null && Error != "GetAllProjects-OK") ErrorMsg += Error + ", ";

            if (ErrorMsg.Length > 0) hv.ErrorMsg = ErrorMsg;
            
            ProjectTreeviewModel projectTreeviewModel = new ProjectTreeviewModel();
            fields.DataSource = projectTreeviewModel.GetProjectTreeviewModel(hv.Projectgroups.List, hv.Projects.List);
            fields.HasChildren = "HasChild";
            fields.Expanded = "Expanded";
            fields.Id = "Id";
            fields.ParentID = "PId";
            fields.Text = "Name";
            ViewBag.fields = fields;

            dr.CloseConnection();
            
            return View(hv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// History:
        /// 2020-07-06 Close Connection eingebaut
        /// </remarks>
        [HttpGet]
        public ActionResult Navigation(string id)
        {
            Log.Debug("Home Controller Navigation");
            int ProjectId;
            string Error = "";

            NavigationView nv = new NavigationView();

            if (id.Substring(0,1) == "P")
            {
                if (int.TryParse(id.Substring(1), out ProjectId))
                {
                    DatareaderP dr = new DatareaderP();
                    nv.Project = dr.GetProjectByPK(ProjectId, out Error);
                    nv.LinkCheck10 = Url.Content("~/check10/Project/" + nv.Project.Id);
                    nv.LinkCheck20 = Url.Content("~/check/Project/" + nv.Project.Id);
                    nv.LinkCheck20N = Url.Content("~/check20/Project/" + nv.Project.Id);
                    nv.LinkCheck30 = Url.Content("~/check30EA/Project/" + nv.Project.Id);
                    nv.LinkCheck30N = Url.Content("~/check30AN/Project/" + nv.Project.Id);
                    nv.LinkImpact = Url.Content("~/impact/Project/" + nv.Project.Id);
                    nv.PlantemplateInsert = Url.Content("~/PlanTemplate/Insert/" + nv.Project.Id);
                    nv.PlanInsert = Url.Content("~/Plan/Insert/" + nv.Project.Id);
                    nv.PlanInsertFromTemplate = Url.Content("~/Plan/InsertFromTemplateProject/" + nv.Project.Id);
                    nv.PlanTreeSort = Url.Content("~/PlanSort/TreeSort/" + nv.Project.Id + "/SW_Release_Plan");
                    nv.GanttSW = Url.Content("~/Gantt/Plan/" + nv.Project.Id + "/SW_Release_Plan");
                    dr.CloseConnection();
                }
                else
                {
                    
                }
            }
            Debug.WriteLine("Test" + id);
            return PartialView(nv);
        }

        public ActionResult About()
        {
            Log.Debug("Home Controller About");
            ViewBag.Message = "ZKW Polarion Report Tool";

            return View();
        }

        public ActionResult Contact()
        {
            Log.Debug("Home Controller Contact");
            
            ViewBag.Message = "";

            return View();
        }
    }
}