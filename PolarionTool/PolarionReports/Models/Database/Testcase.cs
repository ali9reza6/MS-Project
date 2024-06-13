using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Testcase : Workitem
    {
        public List<TestcaseResult> TestcaseResults { get; set; }

        public Testcase()
        {
            
        }

        public Testcase(Workitem w)
        {
           
        }

    }
}