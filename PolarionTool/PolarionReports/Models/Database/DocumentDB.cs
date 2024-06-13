using PolarionReports.Custom_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class DocumentDB
    {
        public int C_pk { get; set; }
        public int ProjectId { get; set; }
        public string C_id { get; set; }
        public string C_type { get; set; }
        public string C_modulefolder { get; set; }
        public DocumentName DocName { get; set; }
        public string c_status { get; set; }
        /// <summary>
        /// Dieser Link öffnet ein Workitem im Document
        /// http://zkwsvpol01/polarion/#/project/E18008/wiki/Specification/20a_abcd Element Requirements DCU-Module?selection=E18008-6439
        /// $"http://zkwsvpol01/polarion/#/project/{myP.C_id}/wiki/Specification/{d.C_id}?selection=";
        /// </summary>
        public string PolarionDocumentLink { get; set; }

        public string GetPolarionDocumentLink(ProjectDB ProjectDB)
        {
            string Link;

            Link = $"http://{Topol.PolarionServer}/polarion/#/project/{ProjectDB.Id}/wiki/{this.C_modulefolder}/" +
                     System.Uri.EscapeDataString(this.C_id) + "?selection=";
            return Link;
        }

        public string GetPolarionTableLink(ProjectDB projectDB)
        {
            string Link;
            Link = $"http://{Topol.PolarionServer}/polarion/#/project/{projectDB.Id}/wiki/{this.C_modulefolder}/"
                 + System.Uri.EscapeDataString(this.C_id) + "?query=";

            return Link;
        }
    }
}