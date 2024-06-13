using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class ProjectTreeviewModel
    {
        public string Id { get; set; }
        public string PId { get; set; }
        public string Name { get; set; }
        public bool HasChild { get; set; }
        public bool Expanded { get; set; }
        public bool Selected { get; set; }

        /// <summary>
        /// Liefert ein TreeviewModel von Projectgroup und Projects
        /// </summary>
        /// <param name="Projects"></param>
        /// <returns></returns>
        public List<ProjectTreeviewModel> GetProjectTreeviewModel(List<Projectgroup> Projectgroups, List<ProjectDB> Projects)
        {
            List<ProjectTreeviewModel> tv = new List<ProjectTreeviewModel>();

            Projectgroup pg = Projectgroups.FirstOrDefault(n => n.Name == "default");
            if (pg != null)
            {
                // Default Projectgroup wird nicht angezeigt
                // tv.Add(new ProjectTreeviewModel { Id = pg.C_pk.ToString(), Name = pg.Name, HasChild = true, Expanded = true});
                FillTree(tv, pg, Projectgroups, Projects);
            }

            return tv;
        }

        private void FillTree(List<ProjectTreeviewModel> tv, Projectgroup parent, List<Projectgroup> Projectgroups, List<ProjectDB> Projects)
        {
            List<Projectgroup> ChildPG = Projectgroups.FindAll(n => n.Parent == parent.C_pk);
            if (ChildPG != null)
            {
                foreach(Projectgroup pg in ChildPG)
                {
                    if (HasChilds(pg.C_pk, Projectgroups, Projects))
                    {
                        Debug.WriteLine(pg.Name);
                        if (parent.Name == "default")
                        {
                            tv.Add(new ProjectTreeviewModel
                            {
                                Id = pg.C_pk.ToString(),
                                Name = pg.Name,
                                HasChild = true,
                                Expanded = false
                            });
                        }
                        else
                        {
                            tv.Add(new ProjectTreeviewModel
                            {
                                Id = pg.C_pk.ToString(),
                                PId = parent.C_pk.ToString(),
                                Name = pg.Name,
                                HasChild = true,
                                Expanded = false
                            });
                        }

                        FillTree(tv, pg, Projectgroups, Projects);
                    }
                    else
                    {
                        Debug.WriteLine("PG ohne Childs" + pg.Name);
                        tv.Add(new ProjectTreeviewModel
                        {
                            Id = pg.C_pk.ToString(),
                            PId = parent.C_pk.ToString(),
                            Name = pg.Name,
                            HasChild = false,
                            Expanded = false
                        });
                    }
                }
            }

            List<ProjectDB> ChildP = Projects.FindAll(n => n.Fk_projectgroup == parent.C_pk);
            if (ChildP != null)
            {
                foreach(ProjectDB p in ChildP)
                {
                    tv.Add(new ProjectTreeviewModel
                    {
                        Id = "P" + p.C_pk.ToString(),
                        PId = parent.C_pk.ToString(),
                        Name = p.Name,
                        HasChild = false,
                        Expanded = false
                    });
                }
            }
        }

        private bool HasChilds(int C_pk, List<Projectgroup> Projectgroups, List<ProjectDB> Projects)
        {
            int AnzahlPG = Projectgroups.Count(n => n.Parent == C_pk);
            int AnzahlPr = Projects.Count(n => n.Fk_projectgroup == C_pk);
            if (AnzahlPG > 0 || AnzahlPr > 0)
            {
                return true;
            }
            return false;
        }
    }
}