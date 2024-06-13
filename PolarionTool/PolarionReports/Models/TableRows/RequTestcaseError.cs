using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.TableRows
{
    /// <summary>
    /// Bildet die Liste Requ-TC-Err-Liste ab
    ///  - Requ-ID (link)
    ///  - Requ-Title
    ///  - Internal Comment
    ///  - Allocation
    ///  - Verification Method      ->CF: verificationMethod
    ///  - Verification Discipline  ->CF: verificationDiscipline und alte Einträge mit: verificationDiscpline
    ///  - doc prefix-C/I-ID(link)
    ///  - Error
    ///  - Requ- Status
    ///  - Link(to Requ)
    /// </summary>
    public class RequTestcaseError
    {
        public Workitem Requirement { get; set; }

        public string VerificationMethod { get; set; }

        public string VerificationDiscipline { get; set; }

        public string Error { get; set; }

        public List<Workitem> Testcases { get; set; }
    }
}