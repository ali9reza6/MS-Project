using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.BusinessLogic;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class Check30ANController : Controller
    {
        // GET: Check30N
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Project(string id, string docFilter)
        {

            string Browser = Request.Browser.Browser;
            DatareaderP dr = new DatareaderP();
            Project myP = InitProject.GetProject(id, Browser);
            Check30NViewModel cv = new Check30NViewModel();
            Check30NBL check30NBL = new Check30NBL();

            cv.Projectname = myP.C_name;
            cv.PolarionLink = $"http://{Topol.PolarionServer}/polarion/#/project/{myP.C_id}/workitem?id=";

            if (id == null)
            {
                return View();
            }

            cv.Lists30N = new List<Check30NListsViewModel>();

            foreach(Document d in myP.Documents)
            {
                if (d.C_id.Substring(0,2) == "30" && d.C_id.Contains("Element Architecture"))
                {
                    if (docFilter != null)
                    {
                        if (!d.DocName.OriginalDocumentname.Contains(docFilter))
                        {
                            continue;
                        }
                    }
                    Check30NListsViewModel l30N = new Check30NListsViewModel();
                    l30N.Document = d;
                    l30N.Document.SetBrowserType(myP.Browser);
                    check30NBL.Check30NDocument(dr, myP, l30N);
                    cv.Lists30N.Add(l30N);
                }
            }

            if (cv.Lists30N.Count == 0)
            {
                // 30 Element Architecture nicht gefunden
                cv.ErrorMsg = "Document with ID: '30x Element Architecture' not found";
                return View(cv);
            }

            return View(cv);
        }

    }
}