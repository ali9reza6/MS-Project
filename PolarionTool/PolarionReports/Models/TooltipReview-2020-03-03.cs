using PolarionReports.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class TooltipReview
    {
        public string Status;
        public string ApprovedWorkItemsWithoutApprovers;
        public string WorkItemsWaitingForApproval;
        public string WorkItemsWaitingForApprovalAll;
        public string WorkItemsWaitingForApprovalNN;
        public string WorkItemsStatusInReviewWithoutApprovers;
        public string WorkItemsApprovalStateDisapproved;
        public string WorkItemsApprovalStateDisapprovedNN;
        public string WorkItemsStatusDisapproved;
        public string WorkItemsStatusDisapprovedNN;
        public string WorkItemsInClarification;
        public string WorkItemsInClarificationAll;
        public string WorkItemsInClarificationRole;
        public string WorkItemsWithoutClarificationRole;
        public string WorkItemsReadyToBeApproved;
        public string RejectedWorkItemsWithApprovers;
        public string WorkItemsStatusApproved;
        public string WorkitemsApprovalStateApprovedStatusWrong;
        public string WorkitemsApprovalStateWaitingStatusNotInReview;
        public string WorkItemsNotInClarificationWithClarificationRole;
        public string WorkItemsWithClarificationRoleCustomer;
        public string ReviewReadyForReview;

        public TooltipReview()
        {
            Status = Tooltips.ReviewStatus;
            ApprovedWorkItemsWithoutApprovers = Tooltips.ReviewApprovedWorkItemsWithoutApprovers;
            WorkItemsWaitingForApproval = Tooltips.ReviewWorkItemsWaitingForApproval;
            WorkItemsWaitingForApprovalAll = Tooltips.ReviewWorkItemsWaitingForApprovalAll;
            WorkItemsWaitingForApprovalNN = Tooltips.ReviewWorkItemsWaitingForApprovalNN;
            WorkItemsStatusInReviewWithoutApprovers = Tooltips.ReviewWorkItemsStatusInReviewWithoutApprovers;
            WorkItemsApprovalStateDisapproved = Tooltips.ReviewWorkItemsApprovalStateDisapproved;
            WorkItemsApprovalStateDisapprovedNN = Tooltips.ReviewWorkItemsApprovalStateDisapprovedNN;
            WorkItemsStatusDisapproved = Tooltips.ReviewWorkItemsStatusDisapproved;
            WorkItemsStatusDisapprovedNN = Tooltips.ReviewWorkItemsStatusDisapprovedNN;
            WorkItemsInClarification = Tooltips.ReviewWorkItemsInClarification;
            WorkItemsInClarificationAll = Tooltips.ReviewWorkItemsInClarificationAll;
            WorkItemsInClarificationRole = Tooltips.ReviewWorkItemsInClarificationRole;
            WorkItemsWithoutClarificationRole = Tooltips.ReviewWorkItemsWithoutClarificationRole;
            WorkItemsReadyToBeApproved = Tooltips.ReviewWorkItemsReadyToBeApproved;
            RejectedWorkItemsWithApprovers = Tooltips.ReviewRejectedWorkItemsWithApprovers;
            WorkItemsStatusApproved = Tooltips.ReviewWorkItemsStatusApproved;
            WorkitemsApprovalStateApprovedStatusWrong = Tooltips.ReviewWorkitemsApprovalStateApprovedStatusWrong;
            WorkitemsApprovalStateWaitingStatusNotInReview = Tooltips.ReviewWorkitemApprovalStateWaitingStatusNotInReview;
            WorkItemsNotInClarificationWithClarificationRole = Tooltips.ReviewWorkItemsNotInClarificationWithClarificationRole;
            WorkItemsWithClarificationRoleCustomer = Tooltips.ReviewWorkItemsWithClarificationRoleCustomer;
            ReviewReadyForReview = Tooltips.ReviewReadyForReview;
        }
    }
}