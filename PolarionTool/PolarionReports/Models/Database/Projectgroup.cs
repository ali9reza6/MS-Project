using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Projectgroup
    {
        public int C_pk { get; set; }
        public string Name { get; set; }
        public int Parent { get; set; }
    }

    public class ProjectgroupList
    {
        public List<Projectgroup> List { get; set; }
    }
}