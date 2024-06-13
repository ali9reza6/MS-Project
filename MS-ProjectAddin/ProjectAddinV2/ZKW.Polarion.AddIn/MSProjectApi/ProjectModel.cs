//using PolarionReports.Models.Database;
//using PolarionReports.Models.Database.Api;
using System.Collections.Generic;

namespace ZKW.Shared.MSProjectApi
{
    public class ProjectModel
    {
        /// <summary>
        /// Eindeutige Projekt ID = Kurzname des Projekt zB.: E18008
        /// </summary>
        public string ProjectID { get; set; }
        public string Baseplan { get; set; }
        public string ErrorMsg { get; set; }

        public List<TaskBase> Tasks { get; set; }

    }
}