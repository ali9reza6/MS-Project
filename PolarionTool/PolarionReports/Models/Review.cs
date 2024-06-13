using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Models
{
    public class Review
    {
        // Approved WorkItems without Approver: 
        // Filter: Approvals = NONE AND Status != Draft AND Status != Deferred AND Status != Rejected” 
        // AND Status != “In Clarification” AND != “In Review”
        public List<Workitem> WorkitemWithoutApprover { get; set; }

        // WorkItems with Status “In Review” without Approvers
        public List<Workitem> WorkitemInReviewWithoutApprover { get; set; }

        // WorkItems with Approval State “Disapproved” 
        public List<Workitem> WorkitemApprovalStateDisapproved { get; set; }
        public List<Approver> ApproverApprovalStateDisapproved { get; set; }

        // WorkItems with Status “Disapproved”
        public List<Workitem> WorkitemDisapproved { get; set; }
        public List<Approver> ApproverDisapprovedList { get; set; }

        // WorkItems in Clarification
        public List<Workitem> WorkitemInClarification { get; set; }
        public List<Workitem> WorkitemInClarificationWithNoRole { get; set; }
        public List<WorkitemClarification> WorkitemPerClarificationList { get; set; }

        // WorkItems waiting for Approval
        public List<Approver> ApproverWaitings { get; set; }
        public List<Workitem> WorkitemReviewWaitings { get; set; }

        // WorkItems with Approval State = Waiting and not in Status “In Review”
        public List<Workitem> WorkitemApprovalStateWaitingStatusNotInReview { get; set; }

        // WorkItems ready to be approved 
        public List<Workitem> WorkitemReadyToApproved { get; set; }

        // Workitems in ApprovalState=Approved and Status not valid draft, clarification, disapproved
        public List<Workitem> WorkitemsApprovalStateApprovedStatusWrong { get; set; }

        // WorkItems with Status “Rejected” with Approvers 
        public List<Workitem> WorkitemRejectedWithApprovers { get; set; }

        //WorkItems with Status "Rejected" with no Approvers OR any Approvers not "Waiting"
        public List<Workitem> WorkitemRejectedApproversNotWaiting { get; set; }

        //WorkItems with Status "Rejected" withiut Approvers
        public List<Workitem> WorkitemRejectedWithoutApprovers { get; set; }

        // WorkItems not "In Clarification" with Clarification Role
        public List<Workitem> WorkItemsNotInClarificationWithClarificationRole { get; set; }

        // WorkItems with Clarification Role = "Customer" (only) 
        public List<Workitem> WorkItemsWithClarificationRoleCustomer { get; set; }

        // Workitems - Requirements ready for Review    2019-03-28
        public List<Workitem> RequirementsReadyForReview { get; set; }

        public List<Workitem> WorkitemApproved { get; set; }
        public List<User> Users { get; set; }

        public Review(List<User> users)
        {
            WorkitemWithoutApprover = new List<Workitem>();
            WorkitemInReviewWithoutApprover = new List<Workitem>();

            WorkitemApprovalStateDisapproved = new List<Workitem>();
            ApproverApprovalStateDisapproved = new List<Approver>();

            WorkitemDisapproved = new List<Workitem>();
            ApproverDisapprovedList = new List<Approver>();

            WorkitemInClarification = new List<Workitem>();
            WorkitemInClarificationWithNoRole = new List<Workitem>();
            WorkitemPerClarificationList = new List<WorkitemClarification>();

            ApproverWaitings = new List<Approver>();
            WorkitemReviewWaitings = new List<Workitem>();

            WorkitemApprovalStateWaitingStatusNotInReview = new List<Workitem>();

            WorkitemReadyToApproved = new List<Workitem>();

            WorkitemsApprovalStateApprovedStatusWrong = new List<Workitem>();

            WorkitemRejectedWithApprovers = new List<Workitem>();
            WorkitemRejectedApproversNotWaiting = new List<Workitem>();
            WorkitemRejectedWithoutApprovers = new List<Workitem>();

            WorkItemsNotInClarificationWithClarificationRole = new List<Workitem>();
            WorkItemsWithClarificationRoleCustomer = new List<Workitem>();

            RequirementsReadyForReview = new List<Workitem>();

            WorkitemApproved = new List<Workitem>();
            Users = users;
        }

        public void CheckWorkitem(Workitem w)
        {
            bool AllApproved = true;
            bool InWaitingList = false;
            bool InDisapprovedList = false;

            // Debugging Help:
            if (w.Id == "E19003-3751")
            {
                Debug.WriteLine(w.Id);
            }

            // Ready for Review hat keine weiteren Abhängigkeiten 2019-04-05 
            if (w.ClarificationRoles != null && w.ClarificationRoles.Any(x => x == "toReview"))
            {
                // Requirements ready for Review!
                RequirementsReadyForReview.Add(w);
            }

            // WorkitemWithoutReviewers: Wenn keine Approvals => in Liste keine weiteren Schritte notwendig
            if (w.Approvals.Count == 0)
            {
                if (w.Status == "rejected")
                {
                    WorkitemRejectedWithoutApprovers.Add(w);
                    return;
                }

                if (w.Status == "reviewNeeded" || w.Status == "inreview")
                {
                    WorkitemInReviewWithoutApprover.Add(w);
                }
                if (w.Status != "draft" &&
                    w.Status != "deferred" &&
                    w.Status != "clarificationNeeded" &&
                    w.Status != "reviewNeeded" &&
                    w.Status != "inreview")
                {
                    WorkitemWithoutApprover.Add(w);
                }

                // Clarification Liste auch ohne Approver füllen
                if (w.Status == "clarificationNeeded")
                {
                    WorkitemInClarification.Add(w);
                    if (w.ClarificationRoles == null)
                    {
                        WorkitemInClarificationWithNoRole.Add(w);
                    }
                    else
                    {
                        AddWorkitemInClarification(w.ClarificationRole, w);
                    }
                }
                return; // keine Approver -> keine weiteren Prüfungen
            }


            // Approver sind vorhanden: 
            if (w.Status == "rejected")
            {
                foreach (WorkitemApproval wa in w.Approvals)
                {
                    if (wa.Status != "waiting") 
                    {
                        if (!WorkitemRejectedApproversNotWaiting.Contains(w))
                        {
                            WorkitemRejectedApproversNotWaiting.Add(w);
                        }
                    }
                }
                    WorkitemRejectedWithApprovers.Add(w);
                return;
            }

            foreach (WorkitemApproval wa in w.Approvals)
            {
                if (wa.Status == "disapproved")
                {
                    // Status != Deferred AND Status != “In Clarification” AND Status != “Disapproved”
                    if (w.Status != "deferred" &&
                        w.Status != "clarificationNeeded" &&
                        w.Status != "disapproved")
                    {
                        if (!WorkitemApprovalStateDisapproved.Contains(w))
                        {
                            WorkitemApprovalStateDisapproved.Add(w);
                            InDisapprovedList = true;
                        }
                        AddApprovalStateDisapprovedUser(wa, w);
                    }


                    if (w.Status == "disapproved")
                    {
                        if (!WorkitemDisapproved.Contains(w))
                        {
                            WorkitemDisapproved.Add(w);
                        }
                        AddDisapprovedUser(wa, w);
                    }

                    AllApproved = false;
                }
                else if (wa.Status == "waiting")
                {
                    if (w.Status == "reviewNeeded" || w.Status == "inreview")
                    {
                        // Nur Workitems mit dem Status "In Review" werden in die Liste aufgenommen
                        if (!InWaitingList)
                        {
                            WorkitemReviewWaitings.Add(w);
                            InWaitingList = true;
                        }
                        AddWaitingUser(wa, w);
                    }
                    else
                    {
                        if (w.Status == "draft")
                        {
                            // Fehlerliste: WorkItems with Approval State = Waiting and not in Status “In Review”
                            if (!WorkitemApprovalStateWaitingStatusNotInReview.Contains(w))
                            {
                                WorkitemApprovalStateWaitingStatusNotInReview.Add(w);
                            }
                        }
                    }
                    AllApproved = false;
                }
                else if (wa.Status == "approved")
                {

                }
                else
                {
                    // unbekannter Status
                }
            }
            
            if (AllApproved)
            {
                if (w.Status == "reviewNeeded" || w.Status == "inreview")
                {
                    WorkitemReadyToApproved.Add(w);
                }
                else if (w.Status == "clarificationNeeded" ||
                         w.Status == "draft" ||
                         w.Status == "disapproved")
                {
                    WorkitemsApprovalStateApprovedStatusWrong.Add(w);
                }
                else
                {
                    WorkitemApproved.Add(w);
                }
            }

            if (w.Status == "clarificationNeeded")
            {
                WorkitemInClarification.Add(w);
                if (w.ClarificationRoles == null)
                {
                    WorkitemInClarificationWithNoRole.Add(w);
                }
                else
                {
                    AddWorkitemInClarification(w.ClarificationRole, w);
                }
            }
            else
            {
                // Not in clarification but with clarification role
                if (w.ClarificationRoles != null && w.ClarificationRoles.Count > 0)
                {
                    if (w.ClarificationRoles.Count == 1 && w.ClarificationRoles.Contains("Customer"))
                    {

                    }
                    else
                    {
                        WorkItemsNotInClarificationWithClarificationRole.Add(w);
                    }
                }
            }

            if (w.ClarificationRoles != null && w.ClarificationRoles.Count == 1 && w.ClarificationRoles.Contains("Customer"))
            {
                WorkItemsWithClarificationRoleCustomer.Add(w);
            }

        } // Ende CheckWorkitem

        void AddWaitingUser(WorkitemApproval wa, Workitem w)
        {
            Approver rw = ApproverWaitings.FirstOrDefault(u => u.User.C_pk == wa.UserId);
            if (rw == null)
            {
                ApproverWaitings.Add(new Approver(wa.UserId, Users, w, ApproverWaitings.Count));
            }
            else
            {
                rw.WorkitemApproverPerUser.Add(w);
            }
        }

        void AddDisapprovedUser(WorkitemApproval wa, Workitem w)
        {
            Approver ad = ApproverDisapprovedList.FirstOrDefault(u => u.User.C_pk == wa.UserId);
            if (ad == null)
            {
                ApproverDisapprovedList.Add(new Approver(wa.UserId, Users, w, ApproverDisapprovedList.Count));
            }
            else
            {
                ad.WorkitemApproverPerUser.Add(w);
            }
        }

        void AddApprovalStateDisapprovedUser(WorkitemApproval wa, Workitem w)
        {
            Approver ad = ApproverApprovalStateDisapproved.FirstOrDefault(u => u.User.C_pk == wa.UserId);
            if (ad == null)
            {
                ApproverApprovalStateDisapproved.Add(new Approver(wa.UserId, Users, w, ApproverApprovalStateDisapproved.Count));
            }
            else
            {
                ad.WorkitemApproverPerUser.Add(w);
            }
        }

        void AddWorkitemInClarification(String ClarificatioRole, Workitem w)
        {
            WorkitemClarification wc = WorkitemPerClarificationList.FirstOrDefault(c => c.ClarificationRole == ClarificatioRole);
            if (wc == null)
            {
                wc = new WorkitemClarification(ClarificatioRole, w, WorkitemPerClarificationList.Count());
                WorkitemPerClarificationList.Add(wc);
            }
            else
            {
                wc.WorkitemPerClarificationRole.Add(w);
            }

        }
    }

    public class Approver
    {
        public int Index { get; set; }
        public User User { get; set; }
        public List<Workitem> WorkitemApproverPerUser { get; set; }

        public Approver(int UserID, List<User> Users, Workitem w, int Index)
        {
            this.Index = Index;
            User = Users.FirstOrDefault(u => u.C_pk == UserID);
            WorkitemApproverPerUser = new List<Workitem>();
            WorkitemApproverPerUser.Add(w);
        }
    }

    public class WorkitemClarification
    {
        public int Index { get; set; }
        public int SortOrder { get; set; }
        public string ClarificationRole { get; set; }
        public List<Workitem> WorkitemPerClarificationRole { get; set; }

        public WorkitemClarification(String ClarificatioRole, Workitem w, int Index)
        {
            this.Index = Index;
            if (ClarificatioRole.Contains("later"))
            {
                this.SortOrder = 1;
            }
            else
            {
                this.SortOrder = 0;
            }
            ClarificationRole = ClarificatioRole;
            WorkitemPerClarificationRole = new List<Workitem>();
            WorkitemPerClarificationRole.Add(w);
        }
    }
}