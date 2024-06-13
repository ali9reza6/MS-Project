using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PolarionReports.Controllers
{
    public class WpController : ApiController
    {
        // GET api/<controller>/5
        public List<PmWorkPackageDB> Get(int id)
        {
            DatareaderP dr = new DatareaderP();
            List<PmWorkPackageDB> wp = dr.GetPmWorkPackageForPlan(id, out string error);

            return wp;
        }
    }
}
