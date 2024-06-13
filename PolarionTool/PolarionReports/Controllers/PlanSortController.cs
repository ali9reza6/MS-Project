using Newtonsoft.Json;
using PolarionReports.BusinessLogic;
using PolarionReports.BusinessLogic.Gantt;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolarionReports.Controllers
{
    public class PlanSortController : Controller
    {
        // GET: PlanSort
        public ActionResult Index(string id, string docFilter)
        {
            PlanSortViewModel vm = new PlanSortViewModel();
            vm.ProjectId = id;
            vm.BasePlan = docFilter;

            return View(vm);
        }

        public ActionResult TreeSort(string id, string docFilter)
        {
            string ErrorMsg = "";
            string Error;
            int ProjectPK;
            int basePlanPK = 0;
            Plan basePlan;
            DatareaderP dr = new DatareaderP();
            PlanViewModel pv = new PlanViewModel();
            // Project einlesen

            pv.Project = dr.GetProjectByID(id, out Error);
            pv.ProjectId = pv.Project.Id;
            ErrorMsg += Error;
            ProjectPK = pv.Project.C_pk;

            // Plans einlesen
            pv.ProjectPlans = dr.GetAllPlansFromProject(ProjectPK, out Error);
            ErrorMsg += Error;

            if (docFilter != null)
            {
                basePlan = pv.ProjectPlans.Plans.FirstOrDefault(p => p.Plandb.Id == docFilter);
                if (basePlan != null)
                {
                    basePlanPK = basePlan.Plandb.PK;
                }
            }

            // Wenn eine c_sortorder vorhanden die Pläne nach der Sortorder sortieren
            // ...

            PlanTreeviewModel PlanTree = new PlanTreeviewModel();
            pv.PlanFields = new TreeViewFieldsSettings();
            pv.PlanFields.DataSource = PlanTree.GetPlanTreeviewModel(pv.ProjectPlans, basePlanPK, true, pv.ProjectPlans);
            pv.PlanFields.HasChildren = "HasChild";
            pv.PlanFields.Expanded = "Expanded";
            pv.PlanFields.Id = "PK";
            pv.PlanFields.ParentID = "PId";
            pv.PlanFields.Text = "Name";
            pv.PlanFields.ImageUrl = "Iconurl";

            pv.ProjectId = id;
            return View(pv);
        }


        public ActionResult Sort(string id, string docFilter)
        {
            PlanSortViewModel vm = new PlanSortViewModel();
            DatareaderP dr = new DatareaderP();
            GanttTaskReader gtr = new GanttTaskReader();
            PlanBL planBL = new PlanBL();

            vm.ProjectId = id;
            vm.BasePlan = docFilter;
            vm.DataUrl = "/PlanSort/Datasource/" + vm.ProjectId + "/" + vm.BasePlan;
            vm.Plans = gtr.GetGanttdata(dr, id, docFilter);
            planBL.SetSortorder(vm.Plans);   // Sortorder prüfen und setzen und in Polarion speichern

            ViewBag.datasource = vm.Plans;

            return View(vm);
        }

        /// <summary>
        /// Liefert die Gantt-Daten als JSON
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="id"></param>
        /// <param name="docFilter"></param>
        /// <returns></returns>
        public ActionResult Datasource(DataManagerRequest dm, string id, string docFilter)
        {

            PlanSortViewModel vm = new PlanSortViewModel();
            DatareaderP dr = new DatareaderP();
            GanttTaskReader gtr = new GanttTaskReader();
            PlanBL planBL = new PlanBL();

            vm.ProjectId = id;
            vm.BasePlan = docFilter;
            vm.Plans = gtr.GetGanttdata(dr, id, docFilter);

            // planBL.SetSortorder(vm.Plans);   // Sortorder prüfen und setzen und in Polarion speichern

            // gdl.Sort((p1, p2) => DateTime.Compare(p1.StartDate.Value, p2.StartDate.Value));

            // Sortorder vergeben
            var count = vm.Plans.Count();

            return Json(new { Items = vm.Plans, Count = count }, JsonRequestBehavior.AllowGet);
        }
    }
}