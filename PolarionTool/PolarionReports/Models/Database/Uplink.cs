using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Uplink
    {
        public int WorkitemId { get; set; }
        public int UplinkId { get; set; }
        public string Role { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string ToWorkItemName { get; set; }
        public bool UplinkToWrongDocument { get; set; }
        public string ErrorMessage { get; set; }

        public string Document2_3
        {
            get
            {
                if (Document2 != null)
                {
                    if (Document2.Length > 3)
                    {
                        return Document2.Substring(0, 3);
                    }
                    else
                    {
                        return Document2;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}