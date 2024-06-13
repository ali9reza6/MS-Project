using PolarionReports.BusinessLogic;
using PolarionReports.BusinessLogic.Api;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolarionReports.Controllers
{
    public class PlanTemplateController : Controller
    {
        // GET: PlanTemplate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Insert(string id)
        {
            PlanTemplateViewModel vm = new PlanTemplateViewModel();
            DatareaderP dr = new DatareaderP();

            vm.Project = dr.GetProjectByID(id, out string error1);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Insert([Bind] PlanTemplateViewModel vm)
        {
            string error = "";
            DatareaderP dr = new DatareaderP();
            vm.Project = dr.GetProjectByID(vm.Project.Id, out string error1);
            if (error1 != "GetProjectByID-OK")
            {
                vm.ErrorMsg = error1;
                return View(vm);
            }

            PolarionInit pol = new PolarionInit();
            Polarion p = new Polarion();
            
            ProjectDB templateProject = dr.GetProjectByID("Template", out string error3);
            if (error3 != "GetProjectByID-OK")
            {
                vm.ErrorMsg = error1;
                return View(vm);
            }

            vm.InsertedTemplates = new List<PlanTemplate>();
            List<PlanTemplate> Templates = dr.GetAllPlanTemplatesFromProject(templateProject.C_pk, out string error4);
            if (error4 != "OK")
            {
                vm.ErrorMsg = error4;
                return View(vm);
            }

            List<PlanTemplate> TemplatesInProject = dr.GetAllPlanTemplatesFromProject(vm.Project.C_pk, out string error5);
            if (error5 != "OK")
            {
                vm.ErrorMsg = error5;
                return View(vm);
            }

            Connection con = pol.Init(vm.Username, vm.Password);
            if (con == null || !con.IsLoggedIn)
            {
                vm.ErrorMsg = "Polarion login not sucessful!";
                return View(vm);
            }

            foreach (PlanTemplate pt in Templates)
            {
                PlanTemplate neu = TemplatesInProject.FirstOrDefault(x => x.C_Id == pt.C_Id);
                if (neu == null)
                {
                    if (!p.InsertPlanTemplate(con, pt, vm.Project.Id, out string error6))
                    {
                        error = error6;
                        break;
                    }
                    else
                    {
                        vm.InsertedTemplates.Add(pt);
                    }
                }
            }

            vm.ErrorMsg = error;

            if (string.IsNullOrWhiteSpace(vm.ErrorMsg))
            {
                if (vm.InsertedTemplates.Count == 0)
                {
                    vm.Message = "All Plantemplates already in " + vm.Project.Name;
                }
                else
                {
                    vm.Message = vm.InsertedTemplates.Count.ToString() + " Plantemplates sucessfully inserted!";
                }
                
            }

            return View(vm);
        }

        public ActionResult Test()
        {
            string error = "";
            string username = "wnpolrose";
            string password = "wnPOL%18";
            DatareaderP dr = new DatareaderP();
            PolarionInit pol = new PolarionInit();
            Polarion p = new Polarion();
            ProjectDB project = dr.GetProjectByID("ZKW_Demo_Project", out string error1);
            ProjectDB templateProject = dr.GetProjectByID("Template", out string error2);

            List<PlanTemplate> Templates = dr.GetAllPlanTemplatesFromProject(templateProject.C_pk, out string error3);
            List<PlanTemplate> TemplatesInProject = dr.GetAllPlanTemplatesFromProject(project.C_pk, out string error4);

            Connection con = pol.Init(username, password);

            foreach (PlanTemplate pt in Templates)
            {
                PlanTemplate neu = TemplatesInProject.FirstOrDefault(x => x.C_Id == pt.C_Id);
                if (neu == null)
                {
                    if (!p.InsertPlanTemplate(con, pt, "ZKW_Demo_Project", out string error5))
                    {
                        error = error5;
                        break;
                    }
                }
            }

            ViewBag.error = error;

            return View();
        }
    }
}