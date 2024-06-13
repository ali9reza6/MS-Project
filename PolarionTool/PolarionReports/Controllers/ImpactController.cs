using PolarionReports.BusinessLogic;
using PolarionReports.Models;
using PolarionReports.Models.Database;
using PolarionReports.Models.Impact;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.Controllers
{
    public class ImpactController : Controller
    {
        // GET: Impact
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Project(string id)
        {
            ImpactProjectViewModel vm = new ImpactProjectViewModel(id);
            vm.ProjectID = id;
            if (vm.projectDB.C_pk == 0)
            {
                // Project not found
                vm.ErrorMsg = "Project: " + id + " not found in Database!";
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Project([Bind] ImpactProjectViewModel vm)
        {
            if (vm.AnalyseWorkitemId == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Workitem/"+ vm.projectDB.c_trackerprefix + "-" + vm.AnalyseWorkitemId, "Impact");
            }
        }

        public ActionResult Workitem(string id)
        {
            DatareaderP dr = new DatareaderP();
            Workitem w;
            Workitem Tempworkitem;
            string WorkitemId;
            string Error;

            string[] ProjectId = id.Split('-');
            Impact Impact = new Impact(ProjectId[0]);  
            
            if (Impact.ProjectDb.Id != Impact.ProjectDb.c_trackerprefix)
            {
                WorkitemId = Impact.ProjectDb.c_trackerprefix + "-" + ProjectId[1];
            }
            else
            {
                WorkitemId = id;
            }

            w = Impact.AllWorkitems.FirstOrDefault(x => x.Id == WorkitemId);
            
            if (w == null || w.C_pk == 0)
            {
                // Workitem nicht gefunden -> Fehler
                // ImpactProjectViewModel vmp = new ImpactProjectViewModel(Impact.ProjectDb.Id);
                // vmp.ErrorMsg = "Workitem not found";
                return View();
            }

            Impact.WorkitemStartingPoint = w;

            if (w.Type == "changerequest")
            {
                // Es wird ein Change Request analysiert -> CR Startseite
                ImpactCR vm = new ImpactCR();
                w.Assignees = dr.GetAssigneesFromWorkitem(w.C_pk, out Error);
                vm.CR = w;
                vm.ProjectDB = Impact.ProjectDb;
                vm.Titel = "CR " + w.Id + " " + w.Title;
                vm.PolarionLink = "http://" + Topol.PolarionServer + "/polarion/#/project/" + vm.ProjectDB.Id + "/workitem?id=";

                vm.TooltipRequirementsAffectedByCR = Impact.Tooltip.RequirementsAffectedByCR;
                vm.TooltipRequirementsIncorrectlyLinkedToCR = Impact.Tooltip.RequirenentsIncorrectlyLinkedToCR;

                if (w.Uplinks == null)
                {
                    w.FillCheckUplink(Impact.AllUplinks);
                }

                vm.RequirementsIncorrectlyLinkedToCR = new List<Workitem>();
                foreach(Uplink u in w.Uplinks)
                {
                    Tempworkitem = Impact.AllWorkitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                    if (Tempworkitem != null)
                    {
                        if (Tempworkitem.Type == "requirement")
                        {
                            Tempworkitem.PolarionDocumentLink = Impact.GetPolarionDocumentLink(Tempworkitem.DocumentId) + Tempworkitem.Id;
                            vm.RequirementsIncorrectlyLinkedToCR.Add(Tempworkitem);
                        }
                        else
                        {
                            // Ignore all other Workitems
                        }
                    }
                }

                if (w.Downlinks == null)
                {
                    w.FillCheckDownlink(Impact.AllUplinks);
                }

                vm.RequirementsAffectedByCR = new List<Workitem>();
                foreach (Uplink d in w.Downlinks)
                {
                    Tempworkitem = Impact.AllWorkitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                    Tempworkitem.FillCheckUplink(Impact.AllUplinks);
                    if (Tempworkitem != null && !Tempworkitem.InBin && Tempworkitem.Status != "rejected")
                    {
                        if (Tempworkitem.Type == "requirement")
                        {
                            Tempworkitem.PolarionDocumentLink = Impact.GetPolarionDocumentLink(Tempworkitem.DocumentId) + Tempworkitem.Id;
                            vm.RequirementsAffectedByCR.Add(Tempworkitem);
                        }
                        else
                        {
                            // Ignore all other Workitems
                        }
                    }
                }

                // Listen sortieren
                vm.RequirementsAffectedByCR = vm.RequirementsAffectedByCR.OrderBy(x => x.Id).ToList();
                vm.RequirementsIncorrectlyLinkedToCR = vm.RequirementsIncorrectlyLinkedToCR.OrderBy(x => x.Id).ToList();

                return View("Cr", vm);

            }
            else if (w.Type == "requirement")
            {
                // Es wird ein Requirement analysiert -> Weiterleitung zu Analyse
                // string param = Impact.ProjectDb.Id + "?workitems=" + id;
                return RedirectToAction("Analyze", "Impact", new { id = Impact.ProjectDb.Id, workitems = id, workitemStartingPoint = Impact.WorkitemStartingPoint.Id});
            }
            else if (w.Type == "testcase" || w.Type == "workPackage" || w.Type == "feature" || w.Type == "productFeature" || w.Type == "customerrequirement")
            {
                // Bei diesen Workitems wird die Verlinkung in beide Richtungen untersucht
                return RedirectToAction("Analyze", "Impact", new { id = Impact.ProjectDb.Id, workitems = id, workitemStartingPoint = Impact.WorkitemStartingPoint.Id });
            }

            return View();
        }

        public ActionResult Analyze(string id, string workitems, string filter, string workitemStartingPoint)
        {
            if (id == null)
            {

            }

            string[] param = workitems.Split('|');
            Debug.WriteLine(workitems);
            Impact impact = new Impact(id);
            ImpactWorkitemViewModel vm = new ImpactWorkitemViewModel(id);
            ImpactBL bl = new ImpactBL();

            if (workitemStartingPoint != null)
            {
                impact.WorkitemStartingPoint = impact.AllWorkitems.FirstOrDefault(x => x.Id == workitemStartingPoint);
            }

            vm.Polarionlink = impact.PolarionLink;

            foreach (string workitemid in param)
            {
                // Analyze Workitem
                WorkitemAnalyze workitemAnalyze = new WorkitemAnalyze(workitemid);
                workitemAnalyze.Workitem = impact.AllWorkitems.FirstOrDefault(x => x.Id == workitemid);
                workitemAnalyze.WorkitemStartingPoint = impact.WorkitemStartingPoint;

                if (workitemAnalyze.Workitem != null)
                {
                    workitemAnalyze.OriginalDocument = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == workitemAnalyze.Workitem.DocumentId);
                    if (workitemAnalyze.OriginalDocument == null)
                    {
                        // Zu Analysierendes Workitem ist in keinem Document in diesem Projekt
                        workitemAnalyze.OriginalDocument = new DocumentDB();
                        workitemAnalyze.OriginalDocument.C_id = "No Document";
                        workitemAnalyze.WorkitemHeader = workitemAnalyze.Workitem.Type;
                    }

                    workitemAnalyze.ChildWorkitems = impact.GetAllChilds(workitemAnalyze.Workitem);
                    workitemAnalyze.ChildDocs = bl.AnalyzeChilds(impact, workitemAnalyze.ChildWorkitems);
                    workitemAnalyze.Docs10 = bl.AnalyzeDoc10(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs20 = bl.AnalyzeDoc20(impact, workitemAnalyze.Workitem);
                    // workitemAnalyze.Docs20 = new List<ImpactDoc20>();
                    workitemAnalyze.Docs30 = bl.AnalyzeDoc30(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs30i = bl.AnalyzeDoc30i(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs40hw = bl.AnalyzeDoc40hw(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs40sw = bl.AnalyzeDoc40sw(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs50hw = bl.AnalyzeDoc50hw(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Docs50sw = bl.AnalyzeDoc50sw(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Testcases = bl.AnalyzeTestcases(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.Features = bl.AnalyzeFeatures(impact, workitemAnalyze.Workitem);
                    workitemAnalyze.WorkPackages = bl.AnalyzeWorkPackage(impact, workitemAnalyze.Workitem);
                    vm.WorkitemsToAnalyse.Add(workitemAnalyze);
                }
            }

            return View(vm);
        }
    }
}