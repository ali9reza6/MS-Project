using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolarionReports.Models.Database;

namespace PolarionReports.Models.TableRows
{
    /// <summary>
    /// Bildet die Liste Requ-TC-Liste ab
    ///  - Requ-ID (link)
    ///  - Requ-Title
    ///  - Internal Comment
    ///  - Allocation
    ///  - Verification Discipline
    ///  - Verification Method
    ///  - doc prefix-C/I-ID(link)
    ///  - TC-Status
    ///  - Requ- Status
    ///  - Link(to Requ)
    /// </summary>
    public class RequTestcase
    {
        public Workitem Requirement { get; set; }

        public string VerificationMethod { get; set; }

        public string VerificationDiscipline { get; set; }

        public List<Workitem> Testcases { get; set; }
    }
}