using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Lists
{
    public class Check10Lists
    {
        // 1. Coverage Report
        // Linkage to 20 Element Requirements
        public CustReqViewModel UnlinkedCustomerRequirements { get; set; }
        public CustReqErrViewModel IncorrectlyLinkedCustomerRequirements { get; set; }
        public CustReqViewModel LinkedCustomerRequirements { get; set; }

        // 2. Status Report
        // Review & Elicitation Status
        public CustReqViewModel PartlyAcceptedCustomerRequirementsWithoutComment { get; set; }
        public CustReqViewModel RejectedCustomerRequirementsWithoutComment { get; set; }
        public CustReqViewModel CustomerRequirementsWithStatusOpen { get; set; }
        public CustReqViewModel CustomerRequirementsWithStatusMaybeAccepted { get; set; }
        public CustReqCRoleViewModel CustomerRequirementsInClarification { get; set; }
        public CustReqCRoleViewModel CustomerRequirementsInClarificationWithoutClarificationRole { get; set; }
        public CustReqCRoleViewModel CustomerRequirementsWithClarificationRoleCustomer { get; set; }
        public CustReqViewModel CustomerRequirementsWithStatusNo { get; set; }
        public CustReqViewModel RejectedCustomerRequirements { get; set; }
        public CustReqViewModel DeletedCustomerRequirements { get; set; }
        public CustReqViewModel PartlyAcceptedCustomerRequirements { get; set; }
        public CustReqViewModel AcceptedCustomerRequirements { get; set; }

        // Special Reports
        public CustReqCustActionViewModel CustomerRequirementsWithCustomerAction { get; set; }
        public CustReqSuppActionViewModel CustomerRequirementsWithSupplierAction { get; set; }
        public CustReqSecSafViewModel CyberSecurityRelatedCustomerRequirements { get; set; }
        public CustReqSecSafViewModel SafetyRelatedCustomerRequirements { get; set; } 

        public Check10Lists(Check10ViewModel cv)
        {
            UnlinkedCustomerRequirements = new CustReqViewModel();

            IncorrectlyLinkedCustomerRequirements = new CustReqErrViewModel();
            LinkedCustomerRequirements = new CustReqViewModel();
            PartlyAcceptedCustomerRequirementsWithoutComment = new CustReqViewModel();
            RejectedCustomerRequirementsWithoutComment = new CustReqViewModel();
            CustomerRequirementsWithStatusOpen = new CustReqViewModel();
            CustomerRequirementsWithStatusMaybeAccepted = new CustReqViewModel();
            CustomerRequirementsInClarification = new CustReqCRoleViewModel();
            CustomerRequirementsInClarificationWithoutClarificationRole = new CustReqCRoleViewModel();
            CustomerRequirementsWithClarificationRoleCustomer = new CustReqCRoleViewModel();
            CustomerRequirementsWithStatusNo = new CustReqViewModel();
            RejectedCustomerRequirements = new CustReqViewModel();
            DeletedCustomerRequirements = new CustReqViewModel();
            PartlyAcceptedCustomerRequirements = new CustReqViewModel();
            AcceptedCustomerRequirements = new CustReqViewModel();
            CustomerRequirementsWithCustomerAction = new CustReqCustActionViewModel();
            CustomerRequirementsWithSupplierAction = new CustReqSuppActionViewModel();
            CyberSecurityRelatedCustomerRequirements = new CustReqSecSafViewModel();
            SafetyRelatedCustomerRequirements = new CustReqSecSafViewModel();
        }
    }
}