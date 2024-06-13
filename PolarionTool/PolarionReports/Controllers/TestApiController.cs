using PolarionReports.BusinessLogic;
using PolarionReports.BusinessLogic.Api;
using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.MSProjectApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TrackerService = net.seabay.polarion.Tracker;

namespace PolarionReports.Controllers
{
    public class TestApiController : ApiController
    {
        // GET api/TestApi
        public IEnumerable<string> Get()
        {
            return new string[] { "Test1", "Test2" };
        }

        public Task Get(string id)
        {
            PolarionInit pi = new PolarionInit();
            Polarion po = new Polarion();
            DatareaderP dr = new DatareaderP();

            TrackerService.WorkItem Workitem;

            /*
            PlanApiDB plan = dr.GetPlanByWBSCode("PMArena", "1.5.3", out string Error);
            if (plan != null)
            {
                Task t = new Task(plan, "TestParentId");
                return t;
            }
            


            
            List<PlanApiDB> plans = dr.GetPlanForProject(id, out string error);

            PlanApiDB pp = plans.FirstOrDefault(p => p.c_id == "M1_Project_Start");
            PlanApiDB parent = plans.FirstOrDefault(p => p.c_pk == pp.fk_parent);
            Task t = new Task(pp, parent.c_id);

            t.Start = DateTime.Parse("17.07.2018");
            */

            Connection con = pi.Init("wnpolrose","wnPOL%18");

            Workitem = con.Tracker.getWorkItemById("PMArena", "PMA-3119");
            Workitem.dueDate = new DateTime(2019, 7, 20);
            Workitem.dueDateSpecified = true;
            con.Tracker.updateWorkItem(Workitem);

            //po.CreateWorkitemCF(con, Workitem.uri, "wpHardware", "Test");
            

            // po.UpdatePlan(con, id, t);

            // po.TestWorkitem(con);

            return new Task();
        }
    }
}
