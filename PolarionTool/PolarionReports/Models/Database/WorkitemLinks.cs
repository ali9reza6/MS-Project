using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemLinks
    {
        public Workitem Workitem { get; set; }
        public List<WorkitemViewLink> LinkedWorkitems { get; set; }
        public string PolarionLink;

        public WorkitemLinks()
        {
            this.LinkedWorkitems = new List<WorkitemViewLink>();
        }
    }

    public class WorkitemViewLink
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string LinkDisplay { get; set; } // Doc:+Type:+ID
        public string PolarionDocumentLink { get; set; }
    }
}