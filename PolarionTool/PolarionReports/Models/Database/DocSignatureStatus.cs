using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class DocSignatureStatus
    {
        public string c_verdict { get; set; }
        public DateTime c_verdicttime { get; set; }
        public string c_signerrole { get; set; }
        public int c_signedrevision { get; set; }
        public string c_name { get; set; }
        public string c_status { get; set; }
        public DateTime c_created { get; set; }
        public string c_text { get; set; }
        public string Color { get; set; }

        public void SetColor()
        {
            if (c_verdict == "invited") { Color = "orange"; }
            else if (c_verdict == "signed") { Color = "green"; }
            else if (c_verdict == "declined") { Color = "red"; }
        }
    }
}