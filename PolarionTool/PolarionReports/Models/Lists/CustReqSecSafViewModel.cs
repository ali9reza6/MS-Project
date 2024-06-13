using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;

namespace PolarionReports.Models.Lists
{
    public class CustReqSecSafViewModel : RequViewModel
    {
        public CustReqSecSafViewModel()
        {
            Table = new List<Workitem>();
        }
    }
}