using PolarionReports.Custom_Helpers;
using PolarionReports.Models.Database;
using PolarionReports.Models.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class Impact
    {
        public ProjectDB ProjectDb { get; set; }
        public List<DocumentDB> DocumentsDB { get; set; }
        public List<Workitem> AllWorkitems { get; set; }
        public List<Uplink> AllUplinks { get; set; }
        public List<WorkitemRequirementAllocation> Wral { get; set; }
        public List<WorkitemReference> WorkitemReferences { get; set; }
        public PlanWorkitemLinkList PlanWorkitemLinkList { get; set; }

        /// <summary>
        /// Diese Workitem wurde in der Startseite der Impact-Analyse eingegeben
        /// </summary>
        public Workitem WorkitemStartingPoint { get; set; }

        public string PolarionLink;
        public string ErrorMsg { get; set; }

        public TooltipImpact Tooltip { get; }

        public Impact()
        {
            Tooltip = new TooltipImpact();
        }

        public Impact(string ProjectId)
        {
            Tooltip = new TooltipImpact();
            // Daten für Analyze aus Datenbank einlesen:
            DatareaderP dr = new DatareaderP();
            string Error = "";

            this.ProjectDb = dr.GetProjectByID(ProjectId, out Error);
            this.DocumentsDB = dr.GetDocumentsDB(this.ProjectDb, out Error);
            this.AllWorkitems = dr.GetWorkitems(this.ProjectDb.C_pk, out Error);
            this.AllUplinks = dr.GetAllUplinks(this.ProjectDb.C_pk, out Error);
            this.Wral = dr.GetAllWorkitemRequirementAllocations(this.ProjectDb.C_pk, this.AllWorkitems, out Error);
            this.WorkitemReferences = dr.GetAllReferences(this.ProjectDb.C_pk, out Error);
            this.PlanWorkitemLinkList = dr.GetPlanWorkitemLinkListForProject(this.ProjectDb.C_pk, out Error);

            PolarionLink = $"http://{Topol.PolarionServer}/polarion/#/project/{this.ProjectDb.Id}/workitem?id=";
        }

        public string GetPolarionDocumentLink(int DocumentPK)
        {
            DocumentDB d = DocumentsDB.FirstOrDefault(x => x.C_pk == DocumentPK);
            if (d != null)
            {
                return d.PolarionDocumentLink;
            }
            else
            {
                return "";
            }
        }

        public List<Workitem> GetWorkitemsDown(Workitem w, string DocPrefix, string DocNameContains, string WorkitemType, bool Software, bool Hardware)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem TempWorkitem;

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(AllUplinks);
            }

            foreach(Uplink d in w.Downlinks)
            {
                TempWorkitem = AllWorkitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                if (TempWorkitem != null)
                {
                    TempWorkitem.FillCheckUplink(AllUplinks);
                    if (TempWorkitem.InBin)
                    {
                        continue;
                    }
                    DocumentDB doc = DocumentsDB.FirstOrDefault(x => x.C_pk == TempWorkitem.DocumentId);
                    if (doc != null)
                    {
                        if (doc.C_id.Substring(0,2) == DocPrefix && doc.C_id.ToUpper().Contains(DocNameContains.ToUpper()))
                        {
                            if (WorkitemType == "requirement")
                            {
                                // HW SW Filter nur bei requirement
                                if (TempWorkitem.Type == WorkitemType)
                                {
                                    if (w.Type == "customerrequirement")
                                    {
                                        // ein customerrequirement wird analysiert -> kein Filter auf HW/SW -> alle requirements
                                        wl.Add(TempWorkitem);
                                    }
                                    else
                                    {
                                        if ((Software && TempWorkitem.Software) ||
                                            (Hardware && TempWorkitem.Hardware))
                                        {
                                            wl.Add(TempWorkitem);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (TempWorkitem.Type == WorkitemType)
                                {
                                    wl.Add(TempWorkitem);
                                }
                            }

                        }
                    }
                }
            }

            return wl;
        }


        public List<Workitem> GetWorkitemsUp(Workitem w, string DocPrefix, string DocNameContains, string WorkitemType, bool Software, bool Hardware)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem TempWorkitem;

            if (w.Uplinks == null)
            {
                w.FillCheckUplink(AllUplinks);
            }

            foreach (Uplink d in w.Uplinks)
            {
                TempWorkitem = AllWorkitems.FirstOrDefault(x => x.C_pk == d.UplinkId);
                if (TempWorkitem != null)
                {
                    TempWorkitem.FillCheckUplink(AllUplinks);
                    if (TempWorkitem.InBin) continue;

                    DocumentDB doc = DocumentsDB.FirstOrDefault(x => x.C_pk == TempWorkitem.DocumentId);
                    if (doc != null)
                    {
                        if (doc.C_id.Substring(0, 2) == DocPrefix && doc.C_id.ToUpper().Contains(DocNameContains.ToUpper()))
                        {
                            if (WorkitemType == "requirement")
                            {
                                // HW SW Filter nur bei requirement
                                if (TempWorkitem.Type == WorkitemType)
                                {
                                    if ((Software && TempWorkitem.Software) ||
                                        (Hardware && TempWorkitem.Hardware))
                                    {
                                        wl.Add(TempWorkitem);
                                    }
                                }
                            }
                            else
                            {
                                if (TempWorkitem.Type == WorkitemType)
                                {
                                    wl.Add(TempWorkitem);
                                }
                            }

                        }
                    }
                }
            }

            return wl;
        }




        public List<Workitem> GetAllChilds(Workitem w)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem Tempworkitem;

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(AllUplinks);
            }
            foreach (Uplink d in w.Downlinks)
            {
                if (d.Role == "parent")
                {
                    Tempworkitem = AllWorkitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                    wl.Add(Tempworkitem);
                    wl.AddRange(GetAllChilds(Tempworkitem));
                }
            }

            return wl;
        }

        public Workitem GetParentLevel0(Workitem w, out bool Reference)
        {
            Workitem Tempworkitem;

            if (w.FillCheckUplink(this.AllUplinks) == 0)
            {
                // no uplinks
                Reference = false;
                return w;
            }
            else
            {
                foreach (Uplink u in w.Uplinks)
                {
                    Tempworkitem = this.AllWorkitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
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

    }
}