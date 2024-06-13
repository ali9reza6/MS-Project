using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;

namespace PolarionReports.Models.Lists
{
    public class CustReqSuppActionViewModel : RequViewModel
    {
        public CustReqSuppActionViewModel()
        {
            Table = new List<Workitem>();
        }
    }
}