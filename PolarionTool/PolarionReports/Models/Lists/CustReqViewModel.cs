using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;

namespace PolarionReports.Models.Lists
{
    public class CustReqViewModel : RequViewModel
    {
        public CustReqViewModel()
        {
            this.Table = new List<Workitem>();
        }
    }
}