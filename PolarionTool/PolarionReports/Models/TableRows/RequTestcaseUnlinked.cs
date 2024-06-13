using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolarionReports.Models.Database;

namespace PolarionReports.Models.TableRows
{
    /// <summary>
    /// Bildet die Liste Requ-TC-Unlinked-Liste ab
    ///  - Requ-ID (link)
    ///  - Requ-Title
    ///  - Internal Comment
    ///  - Allocation
    ///  - Verification Method
    ///  - Verification Discipline
    ///  - Requ- Status
    ///  - Link(to Requ)
    /// </summary>
    public class RequTestcaseUnlinked
    {
        public Workitem Requirement { get; set; }

        public string VerificationMethod { get; set; }

        public string VerificationDiscipline { get; set; }
    }
}