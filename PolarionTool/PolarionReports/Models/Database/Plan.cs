using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Plan
    {
        /// <summary>
        /// relevant Felder aus der Tabelle "plan" der Polarion Datnbank 
        /// </summary>
        public PlanDB Plandb { get; set; }

        /// <summary>
        /// ID des Templates
        /// </summary>
        public string TemplateId { get; set; }

        public enum Plantype { R, I, M, U }

        public Plantype PlanType { get; set; }

        /// <summary>
        /// Mit dieser Plan verlinkte Workitems
        /// </summary>
        public PlanWorkitemLinkList WorkitemLinks { get; set; }

        /// <summary>
        /// Überprüft die Namenskonventionen des Plans
        /// </summary>
        /// <returns></returns>
        public bool CheckPlanName(string NewPlanName, Plantype NewPlanType, out string ErrorMsg)
        {
            if (NewPlanName.Length < 5)
            {
                ErrorMsg = "Planname too short (< 5)";
                return true;
            }
            if (NewPlanName.Substring(0,3) != "SW_" &&
                NewPlanName.Substring(0,3) != "HW_")
            {
                ErrorMsg = "The plan name must start with 'SW_' OR 'HW_'";
                return true;
            }

            if (NewPlanName.Substring(3,2).All(char.IsNumber))
            {
                // OK
            }
            else
            {
                ErrorMsg = "Plan name error: After the prefix 2 numbers are required";
                return true;
            }

            if (NewPlanType == Plantype.R && NewPlanName.Length > 5)
            {
                ErrorMsg = "The Release Plan name ist > 5 Chars (example: SW_10)";
                return true;
            }

            if (NewPlanType == Plantype.I)
            {
                // Iteration Plan the last char in Name must Uppercase Letter
                if (NewPlanName.Length > 6)
                {
                    ErrorMsg = "The Iteration Plan name is > 6 Chars (example: SW_10A)";
                    return true;
                }
                if (NewPlanName.Substring(NewPlanName.Length-1).All(char.IsUpper))
                {
                    ErrorMsg = "";
                    return false;
                }
                else
                {
                    ErrorMsg = "Iteration Plan: the last char in name must uppercase letter";
                    return true;
                }
            }
            else if (NewPlanType == Plantype.M)
            {
                // No checks at this time
                ErrorMsg = "You can not insert a Milestone Plan";
                return true;
            }
            else
            {
                ErrorMsg = "";
                return false;
            }
        }

        public string GetSuffix()
        {
            if (PlanType == Plantype.R)
            {
                if (Plandb.Id == "Template")
                {
                    return "";
                }
                return Plandb.Id.Substring(Plandb.Id.Length - 2);
            }
            else if (PlanType == Plantype.I)
            {
                return Plandb.Id.Substring(Plandb.Id.Length - 3);
            }
            return "";
        }

        public void SetNewPlanName(string NewPlanName, bool IsTemplate)
        {
            string suffix;
            
            // Neuer name xx_nnA
            if (IsTemplate)
            {
                switch (Plandb.Id)
                {
                    case "SW_10":
                        Plandb.Id = "SW_Release_Template";
                        break;

                    case "SW_M_RE_10":
                        Plandb.Id = "SW_M_RE_Template";
                        break;

                    case "SW_10A":
                        Plandb.Id = "SW_Iteration_Template";
                        break;

                    case "SW_M_IS_10A":
                        Plandb.Id = "SW_M_IS_Template";
                        break;

                    case "SW_M_IE_10A":
                        Plandb.Id = "SW_M_IE_Template";
                        break;

                    default:
                        // Plandb.Id ist bereits im Template Project nach neuen Konvebtionen 
                        break;
                }
                Plandb.Name = Plandb.Id;
            }
            else
            {
                if (NewPlanName.Substring(NewPlanName.Length - 1, 1).All(char.IsUpper))
                {
                    // der neue Planname endet bereits mit einem Buchstaben -> es wird eine Iteration eingefügt
                    suffix = NewPlanName.Substring(NewPlanName.Length - 1, 1);
                }
                else
                {
                    suffix = "A";
                }

                switch (Plandb.Id)
                {
                    case "SW_Release_Template":
                        Plandb.Id = NewPlanName;
                        break;

                    case "SW_M_RS_Template":
                        Plandb.Id = "SW_M_RS_" + NewPlanName.Substring(3);
                        break;

                    case "SW_M_RE_Template":
                        Plandb.Id = "SW_M_RE_" + NewPlanName.Substring(3);
                        break;

                    case "SW_Iteration_Template":
                        Plandb.Id = NewPlanName + suffix;
                        break;

                    case "SW_M_IS_Template":
                        Plandb.Id = "SW_M_IS_" + NewPlanName.Substring(3); 
                        break;

                    case "SW_M_IF_Template":
                        Plandb.Id = "SW_M_IF_" + NewPlanName.Substring(3);
                        break;

                    case "SW_M_RD_Template":
                        Plandb.Id = "SW_M_RD_" + NewPlanName.Substring(3);
                        break;

                    case "SW_M_SF_Template":
                        Plandb.Id = "SW_M_SF_" + NewPlanName.Substring(3);
                        break;

                    default:
                        // Plandb.Id ist bereits im Template Project nach neuen Konventionen 
                        break;
                }
                Plandb.Name = Plandb.Id;
            }

        }
    }

    public class PlanList
    {
        /// <summary>
        /// Alle Plans eines Projektes
        /// </summary>
        public List<Plan> Plans { get; set; }

        public string ProjectId { get; set; }

        public PlanList FillPlanSubTree(int Basenode, PlanList AllPlans)
        {
            PlanList pl = new PlanList();
            pl.Plans = new List<Plan>();

            List<Plan> SubPlans = new List<Plan>(); 

            pl.Plans = AllPlans.Plans.FindAll(n => n.Plandb.Parent == Basenode);
            foreach(Plan p in pl.Plans)
            {
                SubPlans.AddRange(FillPlanSubTree(p.Plandb.PK, AllPlans).Plans);
            }

            pl.Plans.AddRange(SubPlans);
            return pl;
        }
    }

    

}