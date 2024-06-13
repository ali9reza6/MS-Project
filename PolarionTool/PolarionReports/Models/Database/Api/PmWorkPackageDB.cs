using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database.Api
{
    public class PmWorkPackageDB
    {
        /// <summary>
        /// Eindeutige Identifikation des Items.
        /// Polarion-Plan: c_id
        /// MS-Project: dieser Wert ist die eindeutige Beziehung zwischen Polarion und MS-Project
        /// </summary>
        public string c_id { get; set; }

        /// <summary>
        /// Visible heading
        /// Polarion-Plan: c_name
        /// </summary>
        public string c_title { get; set; }

        /// <summary>
        /// Polarion-Plan: c_duedate
        /// MS-Project: 
        /// </summary>
        public DateTime c_duedate { get; set; }

        /// <summary>
        /// Polarion-Plan: c_finishedon
        /// </summary>
        public DateTime c_resolvedon { get; set; }

        /// <summary>
        /// Polarion-Plan: c_startdate
        /// </summary>
        public DateTime c_plannedstart { get; set; }

        /// <summary>
        /// Tatsächlicher Start des Workpackages - CustomField "startDate"
        /// Polarion-Plan: c_startedon
        /// </summary>
        public DateTime cf_startdate { get; set; }

        /// <summary>
        /// Status des Workpackage - Für MS-Project; "open-inProgress-closed"
        /// Polarion-Plan: c_status
        /// </summary>
        public string c_status { get; set; }

        /// <summary>
        /// Geplante Anzahl von Stunden die für dieses Workpackage gepolant sind
        /// MS-Project: steuert die % Angabe des Fortschritts
        /// </summary>
        public double c_initialestimate { get; set; }

        /// <summary>
        /// Bereits gebuchter Aufwand in Stunden
        /// MS-Project: steuert die % Angabe des Fortschritts: c_timespent / c_initialestimate * 100
        /// </summary>
        public double c_timespent { get; set; }
        
        /// <summary>
        /// Datum letzte Änderung in Polarion
        /// </summary>
        public DateTime c_updated { get; set; }

        /// <summary>
        /// Primary Key des Plans womit das Workpackage verknüpft ist. (Achtung c_pk wird bei Polarion Reorg geändert!) 
        /// Ebene 1-3 1:1 Beziehung
        /// Ebene 4: 1:n 
        /// </summary>
        public int fk_plan_c_pk { get; set; }

        /// <summary>
        /// c_id des Plans womit das Workpackage verknüpft ist
        /// MS-Project: dieser Wert ist die eindeutige Beziehung zwischen Polarion und MS-Project
        /// </summary>
        public string fk_plan_c_id { get; set; }

        /// <summary>
        /// Wenn ein pmWorkpackage unter einem Workpackage der Ebene 3 hängt, so ist es nicht mehr direkt mit einem
        /// Plan verknüpft, sondern mit dem darüberliegenden pmWorkpackage.
        /// somit ist fk_plan_c_pk = 0 und fk_plan_c_id leer.
        /// Die Hirarchie wird dann über fk_wp_c_id abgebildet.
        /// </summary>
        public string fk_wp_c_id { get; set; }
    }
}