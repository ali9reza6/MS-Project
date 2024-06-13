using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactDocument
    {
        // Felder aus der Datenbank 
        public DocumentDB Doc { get; set; }

        // Links zu Polarion
        public string PolarionDocumentLink { get; set; }
        public string PolarionTableLink { get; set; }

        public ImpactDocument()
        {
            this.Doc = new DocumentDB();
            this.Doc.C_id = "No Document";
        }

        public ImpactDocument(ProjectDB project, DocumentDB doc)
        {
            this.Doc = doc;
            // @@@ToDo Links erzeugen
            this.PolarionDocumentLink = doc.GetPolarionDocumentLink(project);
            this.PolarionTableLink = doc.GetPolarionTableLink(project);

        }

        public string PolarionTableLinkFromWorkitems(List<Testcase> Workitems)
        {
            string Link = "";

            foreach (Workitem w in Workitems)
            {
                if (Link.Length > 2) Link += " || ";
                Link += "id=" + w.Id;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        public string PolarionTableLinkFromWorkitems(List<Workitem> Workitems)
        {
            string Link = "";

            foreach (Workitem w in Workitems)
            {
                if (Link.Length > 2) Link += " || ";
                Link += "id=" + w.Id;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        public string PolarionTableLinkFromWorkitems(List<WorkitemLinkError> WorkitemLinkErrors)
        {
            string Link = "";

            foreach (WorkitemLinkError wle in WorkitemLinkErrors)
            {
                if (Link.Length > 2) Link += " || ";
                Link += "id=" + wle.Workitem.Id;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

        public string PolarionTableLinkFromWorkitems(List<WorkitemLinks> WorkitemLink)
        {
            string Link = "";

            foreach (WorkitemLinks wl in WorkitemLink)
            {
                if (Link.Length > 2) Link += " || ";
                Link += "id=" + wl.Workitem.Id;
            }

            Link = this.PolarionTableLink + System.Uri.EscapeDataString(Link) + "&tab=table";
            Link = Link.Replace("%3D", "%3A");

            return Link;
        }

    }
}