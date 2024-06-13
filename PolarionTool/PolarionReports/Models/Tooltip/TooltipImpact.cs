using PolarionReports.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Tooltip
{
    public class TooltipImpact
    {
        public string RequirenentsIncorrectlyLinkedToCR { get; set; }
        public string RequirementsAffectedByCR { get; set; }

        public TooltipImpact()
        {
            RequirenentsIncorrectlyLinkedToCR = Tooltips.I_RequirenentsIncorrectlyLinkedToCR;
            RequirementsAffectedByCR = Tooltips.I_RequirementsAffectedByCR;
        }
    }
}