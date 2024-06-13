using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    /// <summary>
    /// Class für Impact Analyse Elemenr Requirement Document Level 10
    /// </summary>
    public class ImpactDoc10
    {
        public ImpactDocument Doc10 { get; set; }
        public List<Workitem> LinkedCustomerRequirements { get; set; }
        public string TooltipLinkedCustomerRequirements { get; set; }

        public ImpactDoc10()
        {
            this.LinkedCustomerRequirements = new List<Workitem>();
        }

        // @@@ ToDo eventuell 2. Kontruktor mit doc
    }
}