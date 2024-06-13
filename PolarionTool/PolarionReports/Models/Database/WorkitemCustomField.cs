using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    /// <summary>
    /// Workitem Custom Fields auf String-Values umgesetzt 
    /// </summary>
    public class WorkitemCustomField
    {
        public string WorkitemId { get; set; }
        public string CfName { get; set; }
        public string CfValue { get; set; }

    }
}