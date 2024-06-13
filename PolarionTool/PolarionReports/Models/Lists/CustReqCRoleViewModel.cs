using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Lists
{
    public class CustReqCRoleViewModel : RequViewModel
    {
        public CustReqCRoleViewModel()
        {
            this.Table = new List<Database.Workitem>();
        }
    }
}