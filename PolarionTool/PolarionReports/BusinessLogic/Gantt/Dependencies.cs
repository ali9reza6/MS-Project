using PolarionReports.Models.PlanTreeSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic.Gantt
{
    public class Dependencies
    {
        class DpChange
        {
            int OldValue { get; set; }
        }

        public string UpdateLinks(string dependencies, List<ChangedSortorder> csl)
        {

            string newDependency = "";
            string newDependencies = "";
            List<DpChange> dl = new List<DpChange>();

            // zB.: 5287FS+0.68 days,2809FS
            string[] d = dependencies.Split(',');

            foreach(string s in d)
            {
                int Id = GetId(s);
                foreach (ChangedSortorder cs in csl)
                {
                    newDependency = s;
                    if (Id == cs.OldValue)
                    {
                        // update string
                        newDependency = cs.ToString() + s.Substring(Id.ToString().Length);
                        break;
                    }

                }
                if (newDependencies.Length > 0) newDependencies += ",";
                newDependencies += newDependency;
            }

            return newDependencies;
        }

        private int GetId(string d)
        {
            if (d.Contains("+"))
            {
                // string with positive Offset
                string[] parts = d.Split('+');
                return GetNumber(parts[0]);
            }

            if (d.Contains("-"))
            {
                // string with negativ Offset
                string[] parts2 = d.Split('-');
                return GetNumber(parts2[0]);
            }

            // string without offset  
            return GetNumber(d);
        }

        private int GetNumber(string s)
        {
            string numeric = new String(s.Where(Char.IsDigit).ToArray());
            if (int.TryParse(numeric, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}