using PolarionReports.Models;
using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolarionReports.Controllers
{
    public class ExportController : Controller
    {
        // GET: Export
        public ActionResult Index()
        {
            DatareaderP dr = new DatareaderP();
            Workitem w;
            ExportViewModel vm = new ExportViewModel();

            w = dr.GetWorkitem("RT-3306", out string error);

            

            vm.Title = w.Title;
            vm.Status = w.StatusDisplay;
            vm.Description = w.Description.Substring(10);

            return View(vm);
        }
    }
}