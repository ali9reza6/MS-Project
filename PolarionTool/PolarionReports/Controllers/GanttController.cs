using PolarionReports.BusinessLogic;
using PolarionReports.BusinessLogic.Gantt;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.Models.Gantt;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace PolarionReports.Controllers
{
    /// <summary>
    /// GanttController: Beinhaltet alle Methoden für die Kommunikation zwischen wntopol und Syncfusion Gantt-Control
    /// </summary>
    public class GanttController : Controller
    {
        public static string ProjectId;
        public static string PolarionUser;
        public static string PolarionPW;

        // GET: Gantt
        /// <summary>
        /// Nur für Testzwecke
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            GanttViewModel vm = new GanttViewModel();
            GanttTest gt = new GanttTest();
            DatareaderP dr = new DatareaderP();
            GanttTaskReader gtr = new GanttTaskReader();
            List<GanttData> gdl = gtr.GetGanttdata(dr, "E18010", "SW_Release_Plan");
            gdl.Sort((p1, p2) => DateTime.Compare(p1.StartDate.Value, p2.StartDate.Value));
            ViewBag.DataSource = gdl;

            return View(vm);
        }

        /// <summary>
        /// Initialisiert den View für einen Sysncfusion-Ganttchart
        /// Die Gantt-Daten werden über die Action "Datasource" diect vom GanttControl geholt
        /// Als Masterview wir _LayoutFull verwendet, um die volle Bildschirmbreite auszunutzen.
        /// </summary>
        /// <param name="id">Project-ID zB.: E18008</param>
        /// <param name="docFilter">ID des Wurzelplans zB.: SW_Release_Plan</param>
        /// <returns></returns>
        public ActionResult Plan(string id, string docFilter)
        {
            GanttViewModel vm = new GanttViewModel();

            ProjectId = id;
            vm.Project = id;
            vm.Baseplan = docFilter;
            vm.DataUrl = "/Gantt/Datasource/" + vm.Project + "/" + vm.Baseplan;

            List<DialogDialogButton> btn = new List<DialogDialogButton>() { };
            btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "LOGIN", isPrimary = true } });
            btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "CANCEL" } });
            ViewBag.promptbutton = btn;

            return View("plan","_LayoutFull", vm);
        }

        /// <summary>
        /// Liefert die Gantt-Daten als JSON
        /// Wird vom Gantt-Control aufgerufen, um die Gantt-Daten zu bekommen
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="id">ProjectId zB.: "E18008"</param>
        /// <param name="docFilter"> PlanId des Wurzelplans</param>
        /// <returns>JSON für Gantt</returns>
        public ActionResult Datasource(DataManagerRequest dm, string id, string docFilter)
        {
            Debug.WriteLine(id + " " + docFilter);
            Debug.WriteLine(PolarionUser + " " + PolarionPW);

            GanttViewModel vm = new GanttViewModel();
            DatareaderP dr = new DatareaderP();
            GanttTaskReader gtr = new GanttTaskReader();
            PlanBL planBL = new PlanBL();
            GanttData gd2;

            vm.Project = id;
            vm.Baseplan = docFilter;

            List<GanttData> gdl = gtr.GetGanttdata(dr, id, docFilter);

            // gdl = planBL.SetSortorder(gdl);   // Sortorder prüfen und setzen und in Polarion speichern

            // Sortorder auf ID umsetzten
            /*
            foreach(GanttData gd in gdl)
            {
                gd.TaskIdSave = gd.TaskId;
                gd.TaskId = gd.Sortorder;
                if (gd.ParentId != null || gd.ParentId > 0)
                {
                    gd2 = gdl.FirstOrDefault(x => x.TaskIdSave == gd.ParentId);
                    if (gd2 != null)
                    {
                        gd.ParentId = gd2.Sortorder;
                    }
                }
            }
            */

            var count = gdl.Count();

            /*
            foreach (GanttData gd in gdl)
            {
                if (gd.TaskId == 2808)
                {
                    Debug.WriteLine("Test");
                }
            }
            */

            return Json(new { result = gdl, count = count });
        }

        public ActionResult SaveUserPassword(string user, string password)
        {
            PolarionUser = user;
            PolarionPW = password;
            return Json(new { });
        }

        public class ICRUDModel<T> where T : class
        {
            public object key { get; set; }
            public T value { get; set; }
            public List<T> added { get; set; }
            public List<T> changed { get; set; }
            public List<T> deleted { get; set; }
        }

        public ActionResult BatchSave([FromBody]ICRUDModel<GanttData> data)
        {
            PlanBL planBL = new PlanBL();
            List<GanttData> uAdded = new List<GanttData>();
            List<GanttData> uChanged = new List<GanttData>();
            List<GanttData> uDeleted = new List<GanttData>();

            Debug.WriteLine(PolarionUser + " " + PolarionPW);
            //Performing insert operation
            if (data.added != null && data.added.Count() > 0)
            {
                foreach (var rec in data.added)
                {
                    Debug.WriteLine("data added");
                    // not allowed
                    // uAdded.Add(this.Create(rec));
                }
            }

            ////Performing update operation
            if (data.changed != null && data.changed.Count() > 0)
            {
                foreach (var rec in data.changed)
                {
                    Debug.WriteLine("data edit");
                    uChanged.Add(rec);
                }
                try
                {
                    planBL.UpdatePlanData(ProjectId, uChanged, PolarionUser, PolarionPW);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                
            }

            //Performing delete operation
            if (data.deleted != null && data.deleted.Count() > 0)
            {
                foreach (var rec in data.deleted)
                {
                    Debug.WriteLine("data deleted");
                    // not allowed
                    // uDeleted.Add(this.Delete(rec.taskID));
                }
            }
            return Json(new { addedRecords = uAdded, changedRecords = uChanged, deletedRecords = uDeleted });
        }
    }
}