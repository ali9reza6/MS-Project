using PolarionReports.Models.Database;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class PolarionPlanInsertViewModel
    {
        public bool InsertFromTemplateProject { get; set; }
        public bool Ready { get; set; }
        public string ErrorMsg { get; set; }
        string Error;

        public string TargetPlanPK   { get; set; }
        public string TemplatePlanPK { get; set; }

        /// <summary>
        /// Grunddaten des Target-Projects
        /// </summary>
        public ProjectDB TargetProject { get; set; }
        public ProjectDB TemplateProject { get; set; }

        public Plan     TemplatePlan { get; set; }
        public PlanList TemplateProjectPlans { get; set; }

        public string TargetTemplateId { get; set; }
        
        /// <summary>
        /// In dieser Liste ist der Subtree des Selektierten Plans aus dem Template Tree
        /// </summary>
        public PlanList TemplateTree { get; set; }

        /// <summary>
        /// Das ist der Plan unter welchem der Subtree eingefügt wird
        /// </summary>
        public Plan     TargetPlan         { get; set; }
        public PlanList TargetProjectPlans { get; set; }

        public TreeViewFieldsSettings TemplateFields { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "new plan name")]
        public string NewPlanName { get; set; }

        /// <summary>
        /// leerer Contructor
        /// </summary>
        public PolarionPlanInsertViewModel()
        {
            InsertFromTemplateProject = false;
        }

        /// <summary>
        /// Initialisiert ViewModel mit Source und Target Plan
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public PolarionPlanInsertViewModel(string source, string target, string targetProjectId)
        {
            DatareaderP dr = new DatareaderP(); 

            int SourcePK;
            int TargetPK;

            int.TryParse(source, out SourcePK);
            int.TryParse(target, out TargetPK);

            InsertFromTemplateProject = false;
            Ready = false;

            TargetPlanPK = target;
            TemplatePlanPK = source;

            TemplatePlan = new Plan();
            TargetPlan   = new Plan();
            TemplateTree = new PlanList();


            TemplatePlan.Plandb = dr.GetPlanByPK(SourcePK, out Error);
            if (Error != "GetPlanByPK-OK") ErrorMsg += Error;

            TemplateProject = dr.GetProjectByPK(TemplatePlan.Plandb.ProjectPK, out Error);
            if (Error != "GetProjectByPK-OK") ErrorMsg += Error;

            TemplateProjectPlans = dr.GetAllPlansFromProject(TemplateProject.C_pk, out Error);
            if (Error != "GetAllPlansFromProject-OK") ErrorMsg += Error;

            if (TargetPK > 0)
            {
                TargetPlan.Plandb = dr.GetPlanByPK(TargetPK, out Error);
                if (Error != "GetPlanByPK-OK") ErrorMsg += Error;

                TargetProject = dr.GetProjectByPK(TargetPlan.Plandb.ProjectPK, out Error);
                if (Error != "GetProjectByPK-OK") ErrorMsg += Error;

            }
            else
            {
                // Plan ohne Parent wird eingefügt
                TargetProject = dr.GetProjectByID(targetProjectId, out Error);
                if (Error != "GetProjectByID-OK") ErrorMsg += Error;
            }
            TargetProjectPlans = dr.GetAllPlansFromProject(TargetProject.C_pk, out Error);
            if (Error != "GetAllPlansFromProject-OK") ErrorMsg += Error;
            

            TemplateProjectPlans.ProjectId = TemplateProject.Id;
            TargetProjectPlans.ProjectId = TargetProject.Id;

            // Plantypes setzen
            foreach (Plan p in TemplateProjectPlans.Plans)
            {
                if (p.Plandb.TemplatePK > 0)
                {
                    var plan = TemplateProjectPlans.Plans.Find(x => x.Plandb.PK == p.Plandb.TemplatePK);
                    //removing plans where the template/"type" cant be found
                    if(plan == null)
                    {
                        TemplateProjectPlans.Plans.Remove(plan);
                        continue;
                    }
                    p.TemplateId = plan.Plandb.Id;
                }
                p.PlanType = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, TemplateProjectPlans.Plans);
                p.WorkitemLinks = dr.GetPlanWorkitemLinkList(p.Plandb.PK, out Error);
            }

            foreach (Plan p in TargetProjectPlans.Plans)
            {
                p.PlanType = GetPlanType(p.Plandb.Color, p.Plandb.TemplatePK, TargetProjectPlans.Plans);
                p.WorkitemLinks = dr.GetPlanWorkitemLinkList(p.Plandb.PK, out Error);
            }

            // Subtree der Source befüllen
            TemplateTree.Plans = new List<Plan>();
            TemplatePlan.PlanType = GetPlanType(TemplatePlan.Plandb.Color, TemplatePlan.Plandb.TemplatePK, TemplateProjectPlans.Plans);
            TemplateTree.Plans.Add(TemplatePlan); // Head-Node
            TemplateTree.Plans.AddRange(TemplateTree.FillPlanSubTree(TemplatePlan.Plandb.PK, TemplateProjectPlans).Plans);

            foreach (Plan p in TemplateTree.Plans)
            {
                if (p.Plandb.TemplatePK > 0)
                {
                    var plan = TemplateProjectPlans.Plans.Find(x => x.Plandb.PK == p.Plandb.TemplatePK);
                    //removing plans where the template/"type" cant be found
                    if (plan == null)
                    {
                        TemplateProjectPlans.Plans.Remove(plan);
                        continue;
                    }
                    p.TemplateId = plan.Plandb.Id;
                }
            }
            // Name des neuen einzufügenden Planes ermitteln:
            // Wird eventuell später erledigt

        }

        private Plan.Plantype GetPlanType(int Color, int TemplatePK, List<Plan> Plans)
        {
            int MyColor;

            if (TemplatePK == 370471)
            {
                Debug.WriteLine(TemplatePK);
            }

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
                return Plan.Plantype.I; // Iteration
            }
            else if (MyColor == 2)
            {
                return Plan.Plantype.R; // Release
            }
            else if (MyColor == 3)
            {
                return Plan.Plantype.M; // Milestone
            }

            return Plan.Plantype.U; // Undefined;
        }

    }
}