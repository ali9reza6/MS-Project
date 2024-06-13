using PolarionReports.Custom_Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Project
    {
        public int    C_pk { get; set; }
        public string C_id { get; set; }
        public string C_name { get; set; }
        public int    Fk_projectgroup { get; set; }
        public string DatabaseErrorMessage { get; set; }
        public string Browser { get; set; }

        public List<User> Users { get; set; }
        public List<Document> Documents { get; set; }
        public List<Workitem> Workitems { get; set; }
        public List<Uplink> Uplinks { get; set; }
        public List<Downlink> Downlinks { get; set; }
        public List<WorkitemReference> WorkitemReferences { get; set; }
        public List<WorkitemRequirementAllocation> WorkitemRequirementAllocations { get; set; }
        public List<Hyperlink> Hyperlinks { get; set; }
        public List<WorkitemApproval> WorkitemApprovals { get; set; }
        public List<Comment> Comments { get; set; }
             
        public string GetDocumentname(int DocumentID)
        {
            Document d = Documents.FirstOrDefault(x => x.C_pk == DocumentID);
            if (d != null)
            {
                return d.C_id;
            }
            else
            {
                return "";
            }
        }

        #region public string GetUplinkDocumentname(Uplink u)
        /// <summary>
        /// Diese Methode liefert den Dokumentennamen wohin der uplink zeigt.
        /// Falls der Link in ein anderes Projekt geht wir derzeit nur ein Fehlertext erzeugt.
        /// Später sollte der andere Projektname und Documentname zurückgegeben werden.
        /// </summary>
        /// <param name="u">uplink</param>
        /// <returns></returns>
        public string GetUplinkDocumentname(Uplink u)
        {
            Workitem w = Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
            if (w == null)
            {
                u.UplinkToWrongDocument = true;
                u.ErrorMessage = "Link to other project";
                u.Document2 = "???";
                return u.Document2;
            }
            
            Document d = Documents.FirstOrDefault(x => x.C_pk == w.DocumentId);
            if (d == null)
            {
                u.ErrorMessage = "Workitem without Document";
                u.Document2 = "Workitem without Document";
                return u.Document2;
            }
            u.ToWorkItemName = w.Id;
            u.Document2 = d.C_id;
            return d.C_id;
        }
        #endregion

        #region public bool IsUplinkCustomerRequirement(Uplink u)
        /// <summary>
        /// Diese Methode überprüft ob der Uplink in ein Workitem geht, 
        /// welches ein "customerrequirement" ist. 
        /// </summary>
        /// <param name="u">Uplink zu einem anderem Workitem</param>
        /// <returns>
        /// true ... das verlinkte Workitem ist ein "customerrequirement"
        /// false .. das verlinkte Workitem ist kein "customerrequirement"
        /// </returns>
        public bool IsUplinkCustomerRequirement(Uplink u)
        {
            Workitem w = Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
            if (w == null)
            {
                return false;
            }
            if (w.Type == "customerrequirement") return true;
            else return false;
        }
        #endregion

        public bool CheckDocumentLinkNames(string FromDoc, string ToDoc)
        {
            switch (FromDoc.Substring(0,2))
            {
                case "20":
                    // Das 20 Dokument soll in ein 10 Doc verweisen

                    break;

                default:

                    break;
            }

            return true;
        }

        public bool EmptyDocument(string DocumentName)
        {
            int NumberOfWorkitems = 0;
            foreach (Document d in Documents)
            {
                if (d.C_id.Contains(DocumentName))
                {
                    // Document gefunden -> Check Anzahl der Workitems
                    NumberOfWorkitems = Workitems.Count(x => x.DocumentId == d.C_pk);
                }
            }
            if (NumberOfWorkitems > 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DocumentExits(string DocNamePrefix, string DocNameContains)
        {
            foreach (Document d in this.Documents)
            {
                if (d.DocName.OriginalDocumentname.Length < 2)
                {
                    continue;
                }

                if (d.DocName.OriginalDocumentname.Substring(0, DocNamePrefix.Length) == DocNamePrefix && d.DocName.OriginalDocumentname.Contains(DocNameContains))
                {
                    return true;
                }
                
            }
            return false;
        }

        /// <summary>
        /// Liefert das Document mit dem Prefix und Namensteil
        /// </summary>
        /// <param name="DocNamePrefix"></param>
        /// <param name="DocNameContains"></param>
        /// <returns>Document oder null wenn nicht gefunden</returns>
        public Document GetDocument(string DocNamePrefix, string DocNameContains)
        {
            foreach (Document d in this.Documents)
            {
                if (d.DocName.OriginalDocumentname.Length < 2)
                {
                    continue;
                }

                if (d.DocName.OriginalDocumentname.Substring(0, DocNamePrefix.Length) == DocNamePrefix && d.DocName.OriginalDocumentname.Contains(DocNameContains))
                {
                    return d;
                }

            }
            return null;
        }

        public Workitem GetParentLevel0(Workitem w, out bool Reference)
        {
            Workitem Tempworkitem;

            if (w.FillCheckUplink(Uplinks) == 0)
            {
                // no uplinks
                Reference = false;
                return w;
            }
            else
            {
                foreach (Uplink u in w.Uplinks)
                {
                    Tempworkitem = Workitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                    if (Tempworkitem == null)
                    {
                        // Linked Workitem not Found -> Link to other Project or error ???
                        Reference = false;
                        return w;
                    }
                    else
                    {
                        // break recursive calls if referces to other document
                        if (u.Role == "parent")
                        {
                            if (w.DocumentId != Tempworkitem.DocumentId)
                            {
                                Reference = true;
                                return w;
                            }
                            return GetParentLevel0(Tempworkitem, out Reference);
                        }
                    }
                }
            }
            Reference = false;
            return w;
        }

        public string GetPolarionLink(string DocumentId, string WorkitemId)
        {
            return $"http://{Topol.PolarionServer}/polarion/#/project/{C_id}/wiki/Specification/{DocumentId}?selection={WorkitemId}";
        }

        public string GetPolarionLink(string folder, string DocumentId, string WorkitemId)
        {
            return $"http://{Topol.PolarionServer}/polarion/#/project/{C_id}/wiki/{folder}/{DocumentId}?selection={WorkitemId}";
        }
    }
}