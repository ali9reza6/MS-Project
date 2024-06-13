using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Hyperlink
    {
        public int WorkitemID { get; set; }
        public string URL { get; set; }

        public string URL30
        {
            get
            {
                if (URL != null && URL.Length > 30)
                {
                    return URL.Substring(0, 30);
                }
                else
                {
                    return URL;
                }
            }
        }
    }
}