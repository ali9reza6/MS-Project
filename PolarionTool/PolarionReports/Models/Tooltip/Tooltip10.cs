using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolarionReports.App_GlobalResources;

namespace PolarionReports.Models.Tooltip
{
    public class Tooltip10
    {
        public string AcceptedCustomerRequirements { get; set; }
        public string CustomerRequirementsInClarification { get; set; }
        public string CustomerRequirementsInClarificationWithoutClarificationRole { get; set; }
        public string CustomerRequirementsWithClarificationRoleCustomer { get; set; }
        public string CustomerRequirementsWithCustomerAction { get; set; }
        public string CustomerRequirementsWithStatusNo { get; set; }
        public string CustomerRequirementsWithStatusOpen { get; set; }
        public string CustomerRequirementsWithStatusMaybeAccepted { get; set; }
        public string CustomerRequirementsWithSupplierAction { get; set; }
        public string CyberSecurityRelatedCustomerRequirements { get; set; }
        public string DeletedCustomerRequirements { get; set; }
        public string IncorrectlyLinkedCustomerRequirements { get; set; }
        public string LinkageTo20ElementRequirements { get; set; }
        public string LinkedCustomerRequirements { get; set; }
        public string PartlyAcceptedCustomerRequirements { get; set; }
        public string PartlyAcceptedCustomerRequirementsWithoutComment { get; set; }
        public string RejectedCustomerRequirements { get; set; }
        public string RejectedCustomerRequirementsWithoutComment { get; set; }
        public string ReviewElicitationStatus { get; set; }
        public string SafetyRelatedCustomerRequirements { get; set; }
        public string UnlinkedCustomerRequirements { get; set; }
        public string SpecialReports { get; set; }

        public Tooltip10()
        {
            AcceptedCustomerRequirements = Tooltips.C10AcceptedCustomerRequirements;
            CustomerRequirementsInClarification = Tooltips.C10CustomerRequirementsInClarification;
            CustomerRequirementsInClarificationWithoutClarificationRole = Tooltips.C10CustomerRequirementsInClarificationWithoutClarificationRole;
            CustomerRequirementsWithClarificationRoleCustomer = Tooltips.C10CustomerRequirementsWithClarificationRoleCustomer;
            CustomerRequirementsWithCustomerAction = Tooltips.C10CustomerRequirementsWithCustomerAction;
            CustomerRequirementsWithStatusNo = Tooltips.C10CustomerRequirementsWithStatusNo;
            CustomerRequirementsWithStatusOpen = Tooltips.C10CustomerRequirementsWithStatusOpen;
            CustomerRequirementsWithStatusMaybeAccepted = Tooltips.C10CustomerRequirementsWithStatusMaybeAccepted;
            CustomerRequirementsWithSupplierAction = Tooltips.C10CustomerRequirementsWithSupplierAction;
            CyberSecurityRelatedCustomerRequirements = Tooltips.C10CyberSecurityRelatedCustomerRequirements;
            DeletedCustomerRequirements = Tooltips.C10DeletedCustomerRequirements;
            IncorrectlyLinkedCustomerRequirements = Tooltips.C10IncorrectlyLinkedCustomerRequirements;
            LinkageTo20ElementRequirements = Tooltips.C10LinkageTo20ElementRequirements;
            LinkedCustomerRequirements = Tooltips.C10LinkedCustomerRequirements;
            PartlyAcceptedCustomerRequirements = Tooltips.C10PartlyAcceptedCustomerRequirements;
            PartlyAcceptedCustomerRequirementsWithoutComment = Tooltips.C10PartlyAcceptedCustomerRequirementsWithoutComment;
            RejectedCustomerRequirements = Tooltips.C10RejectedCustomerRequirements;
            RejectedCustomerRequirementsWithoutComment = Tooltips.C10RejectedCustomerRequirementsWithoutComment;
            ReviewElicitationStatus = Tooltips.C10ReviewElicitationStatus;
            SafetyRelatedCustomerRequirements = Tooltips.C10SafetyRelatedCustomerRequirements;
            UnlinkedCustomerRequirements = Tooltips.C10UnlinkedCustomerRequirements;
            SpecialReports = Tooltips.C10SpecialReports;
        }
    }
}