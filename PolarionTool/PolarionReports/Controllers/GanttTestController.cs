using PolarionReports.Models;
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
    public class GanttTestController : Controller
    {
        public static string PolarionUser;
        public static string PolarionPW;

        // GET: GanttTest
        public ActionResult Index(string id, string docFilter)
        {
            GanttViewModel vm = new GanttViewModel();

            PolarionUser = "User";
            PolarionPW = "Password";

            vm.Project = id;
            vm.Baseplan = docFilter;
            vm.DataUrl = "/GanttTest/Datasource/" + vm.Project + "/" + vm.Baseplan;

            List<DialogDialogButton> btn = new List<DialogDialogButton>() { };
            btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "LOGIN", isPrimary = true } });
            btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "CANCEL" } });
            ViewBag.promptbutton = btn;

            return View(vm);
        }

        public ActionResult Datasource(DataManagerRequest dm, string id, string docFilter)
        {
            Debug.WriteLine(id + " " + docFilter);
            Debug.WriteLine(PolarionUser + " " + PolarionPW);
            GanttTest gt = new GanttTest();

            List<GanttData> gd = gt.GetGanttData();
            var count = gd.Count();

            return Json(new { result = gd, count = count });
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

    public class promptButtonModel
    {
        public string content { get; set; }
        public bool isPrimary { get; set; }
    }
}