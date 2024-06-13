using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PolarionReports.Models
{
    public class PlanTreeviewModel
    {
        public string PK       { get; set; }
        public string Id       { get; set; }
        public string PId      { get; set; }
        public string Name     { get; set; }
        public string Iconurl  { get; set; }
        public string Status   { get; set; }
        public string Type     { get; set; }
        public string Level    { get; set; }
        public bool   HasChild { get; set; }
        public bool   Expanded { get; set; }
        public bool   Selected { get; set; }

        /// <summary>
        /// Liefert ein TreeviewModel von Plans
        /// </summary>
        /// <param name="PlanList">Liste der Plans die im Treeview angezeigt werden sollen</param>
        /// <param name="ParentID">PrimaryKey des Head-Nodes</param>
        /// <param name="NoTemplate">true...keine Template-Plans im TreeView</param>
        /// <param name="AllPlans">Alle Plane des Projectes um auch die Templates für die Farben zu haben</param>
        /// <returns></returns>
        public List<PlanTreeviewModel> GetPlanTreeviewModel(PlanList PlanList, int ParentID, bool NoTemplate, PlanList AllPlans)
        {
            List<PlanTreeviewModel> tv = new List<PlanTreeviewModel>();
            List<Plan> pl;
            int Level = 0;

            if (ParentID == 0)
            {
                pl = PlanList.Plans.FindAll(n => n.Plandb.Parent == 0);
            }
            else
            {
                pl = PlanList.Plans.FindAll(n => n.Plandb.PK == ParentID);
            }

            pl = pl.OrderBy(x => x.Plandb.c_sortorder).ToList();

            if (pl != null)
            {
                foreach(Plan p in pl)
                {
                    if (NoTemplate)
                    {
                        if (p.Plandb.IsTemplate == NoTemplate)
                        {
                            continue;
                        }
                    }
                    
                    if (HasChilds(p.Plandb.PK, PlanList.Plans))
                    {
                        // Insert TreeItem with childs
                        tv.Add(new PlanTreeviewModel
                        {
                            PK = p.Plandb.PK.ToString(),
                            Id = p.Plandb.Id.ToString(),
                            Name = p.Plandb.Name,
                            Status = p.Plandb.Status,
                            Iconurl = GetIconUrl(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Type = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Level = Level.ToString(),
                            HasChild = true,
                            Expanded = false
                        });
                        // Fill Childs
                        FillTree(tv, p, PlanList, AllPlans, Level);
                    }
                    else
                    {
                        // Insert TreeItem
                        tv.Add(new PlanTreeviewModel
                        {
                            PK = p.Plandb.PK.ToString(),
                            Id = p.Plandb.Id.ToString(),
                            Name = p.Plandb.Name,
                            Status = p.Plandb.Status,
                            Iconurl = GetIconUrl(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Type = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Level = Level.ToString(),
                            HasChild = false,
                            Expanded = false
                        });  
                    }
                }
            }

            return tv;
        }

        private void FillTree(List<PlanTreeviewModel> tv, Plan Parent, PlanList PlanList, PlanList AllPlans, int Level)
        {
            int myLevel = Level + 1;
            List<Plan> ChildPlans = PlanList.Plans.FindAll(n => n.Plandb.Parent == Parent.Plandb.PK).OrderBy(n => n.Plandb.c_sortorder).ToList();
            
            if (ChildPlans != null)
            {
                foreach (Plan p in ChildPlans)
                {
                    if (HasChilds(p.Plandb.PK, PlanList.Plans))
                    {
                        Debug.WriteLine(p.Plandb.Name);

                        tv.Add(new PlanTreeviewModel
                        {
                            PK = p.Plandb.PK.ToString(),
                            Id = p.Plandb.Id.ToString(),
                            PId = p.Plandb.Parent.ToString(),
                            Name = p.Plandb.Name,
                            Status = p.Plandb.Status,
                            Iconurl = GetIconUrl(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Type = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Level = myLevel.ToString(),
                            HasChild = true,
                            Expanded = false
                        });

                        FillTree(tv, p, PlanList, AllPlans, myLevel); // Rekursion
                    }
                    else
                    {
                        // Debug.WriteLine("Plan ohne Childs" + p.Plandb.Name);
                        tv.Add(new PlanTreeviewModel
                        {
                            PK = p.Plandb.PK.ToString(),
                            Id = p.Plandb.Id.ToString(),
                            PId = p.Plandb.Parent.ToString(),
                            Name = p.Plandb.Name,
                            Status = p.Plandb.Status,
                            Iconurl = GetIconUrl(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Type = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, AllPlans.Plans),
                            Level = myLevel.ToString(),
                            HasChild = false,
                            Expanded = false
                        });
                    }
                }
            }
        }

        #region HasChilds
        /// <summary>
        /// Überprüft, ob für den PK (PrimaryKey) Childs existieren
        /// </summary>
        /// <param name="PK">Primary Key des Parents</param>
        /// <param name="Plans">Liste aller Plans</param>
        /// <returns></returns>
        private bool HasChilds(int PK, List<Plan> Plans)
        {
            int AnzahlPlan = Plans.Count(n => n.Plandb.Parent == PK);
           
            if (AnzahlPlan > 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region GetIconUrl(int Color, int TemplatePK, List<Plan> Plans)
        private string GetIconUrl(int Color, int TemplatePK, List<Plan> Plans)
        {
            string IconGreen = "/Content/images/plangre.png";
            string IconViolet = "/Content/images/planvio.png";
            string IconOrange = "/Content/images/planora.png";
            int MyColor;

            MyColor = Color;

            if (MyColor == 0)
            {
                // Farbe aus dem Template
                Plan Template = Plans.FirstOrDefault(p => p.Plandb.PK == TemplatePK);
                if (Template != null)
                {
                    MyColor = Template.Plandb.Color;
                }
            }

            if (MyColor == 1)
            {
                return IconGreen;
            }
            else if (MyColor == 2)
            {
                return IconViolet;
            }
            else if (MyColor == 3)
            {
                return IconOrange;
            }
            

            return null;
        }
        #endregion

        #region string GetPlanType(int Color, int TemplatePK, List<Plan> Plans)
        private string GetPlanType(int Color, int TemplatePK, List<Plan> Plans)
        {
            int MyColor;

            MyColor = Color;

            if (MyColor == 0)
            {
                // Farbe aus dem Template
                Plan Template = Plans.FirstOrDefault(p => p.Plandb.PK == TemplatePK);
                if (Template != null)
                {
                    MyColor = Template.Plandb.Color;
                }
            }

            if (MyColor == 1)
            {
                return "I"; // Iteration
            }
            else if (MyColor == 2)
            {
                return "R"; // Release
            }
            else if (MyColor == 3)
            {
                return "M"; // Milestone
            }

            return null;
        }
        #endregion
    }
}
