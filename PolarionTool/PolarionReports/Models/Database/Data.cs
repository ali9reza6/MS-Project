using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Data
    {
        public Project Project { get; set; }
        public List<Document> Documents { get; set;}
        public List<Workitem> Workintems { get; set; }
        public List<Downlink> Downlinks { get; set; }
        public List<Uplink> Uplinks { get; set; }
        public List<WorkitemReference> WorkitemReferences { get; set; }

        public Project FillProject(string Projectname)
        {
            Project = new Project();

            // 

            return Project;
        }
    }
}