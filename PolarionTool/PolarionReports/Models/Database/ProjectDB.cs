using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class ProjectDB
    {
        public int C_pk { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Fk_projectgroup { get; set; }
        public string c_trackerprefix { get; set; }
    }

    public class ProjectDBList
    {
        public List<ProjectDB> List { get; set; }
    }
}