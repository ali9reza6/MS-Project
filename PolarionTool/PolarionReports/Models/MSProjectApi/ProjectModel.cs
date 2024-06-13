using PolarionReports.Models.Database;
using PolarionReports.Models.Database.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.MSProjectApi
{
    public class ProjectModel
    {
        /// <summary>
        /// Eindeutige Projekt ID = Kurzname des Projekt zB.: E18008
        /// </summary>
        public string ProjectID { get; set; }

        public string Baseplan { get; set; }

        /// <summary>
        /// Polarion Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Polarion Password
        /// </summary>
        public string Password { get; set; }


        public List<Task> Tasks { get; set; }

    }
}