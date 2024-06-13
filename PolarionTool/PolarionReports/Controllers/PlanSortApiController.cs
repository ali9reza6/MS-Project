using Newtonsoft.Json;
using PolarionReports.BusinessLogic;
using PolarionReports.Models.MSProjectApi;
using PolarionReports.Models.PlanTreeSort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PolarionReports.Controllers
{
    public class PlanSortApiController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]TreeSortModel tsm)
        {
            /*
            int i = 1;
            foreach(TreeSort ts in tsm.TreesortList)
            {
                Debug.WriteLine(i++.ToString() + " " + ts.PK + " " + ts.Id);
            }
            */
            HttpResponseMessage m = new HttpResponseMessage();
            PlanBL planBL = new PlanBL();

            if (planBL.SaveSortorder(tsm))
            {
                m.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                m.StatusCode = HttpStatusCode.InternalServerError;
            }

            return m;
        }
    }
}
