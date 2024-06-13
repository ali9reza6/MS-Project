using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class PlanDB
    {
        public int PK { get; set; }
        public int Parent { get; set; }
        public int ProjectPK { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
        public bool IsTemplate { get; set; }
        public int  Color { get; set; }
        public int  TemplatePK { get; set; }
        
        /// <summary>
        /// Sortierreihenfolge der Pläne internes Feld in Polarion Datenbank 
        /// </summary>
        public int c_sortorder { get; set; }
    }
}