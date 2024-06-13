using PolarionReports.Models.TableRows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Lists
{
    public class RequTestcaseErrorListViewModel
    {
        public string Titel { get; set; }
        public string Id { get; set; }
        public string Color { get; set; }
        public string Tooltip { get; set; }
        public string PolarionLink { get; set; }
        public string PolarionDocumentLink { get; set; }
        public HtmlString PolarionTableLink { get; set; }
        public int ExpandLevel { get; set; }
        public List<RequTestcaseError> Table { get; set; }

        /// <summary>
        /// Liefert eine Liste der Requirement Id's
        /// </summary>
        /// <returns>List<string> Ids</returns>
        public List<string> GetWorkitemIds()
        {
            List<string> Ids = new List<string>();
            foreach (RequTestcaseError w in Table)
            {
                Ids.Add(w.Requirement.Id);
            }
            return Ids;
        }
    }
}