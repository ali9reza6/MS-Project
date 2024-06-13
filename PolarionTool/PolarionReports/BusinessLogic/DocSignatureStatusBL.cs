using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic
{
    public class DocSignatureStatusBL
    {
        /// <summary>
        /// Liste der Documenten Signaturen
        /// </summary>
        public List<DocSignatureStatus> DocSignatureStatusList { get; set; }
        public List<DocSignatureStatus> ActualDocSignatureList { get; set; }
        public List<DocSignatureStatus> LastAcceptDocSignatureList { get; set; }
        public List<DocSignatureStatus> LastApprovDocSignatureList { get; set; }
        public string lastStatus { get; set; }

        public string Line1Color { get; set; }
        public string Line1Text { get; set; }

        public string Line2Color { get; set; }
        public string Line2Text { get; set; }
        public string Line2Collapse { get; set; }

        public DateTime LastApprovelDate { get; set; }
        public DateTime LastAcceptedDate { get; set; }

        public DocSignatureStatusBL()
        {
            ActualDocSignatureList = new List<DocSignatureStatus>();
            LastAcceptDocSignatureList = new List<DocSignatureStatus>();
            LastApprovDocSignatureList = new List<DocSignatureStatus>();
        }

        public void MakeView(Document d)
        {
            // 1. Zeile im Report
            if (d.C_status == "draft")
            {
                Line1Color = "royalblue";
            }
            else if ((d.C_status == "approved" || d.C_status == "accepted") && !OpenSignees())
            {
                Line1Color = "green";
            }
            else if (d.C_status == "inReview")
            {
                Line1Color = "orange";
            }
            else if (d.C_status == "approved" && OpenSignees())
            {
                Line1Color = "orange";
            }
            else
            {
                Line1Color = "red";
            }
            Line1Text = "Document Status: " + d.C_status;

            // 2.Zeile im Report

            if (OnceApproved() && (d.C_status == "draft" || d.C_status == "accepted"))
            {
                Line2Color = "royalblue";
                Line2Text = "Last Approval: " + LastApprovelDate.ToShortDateString();
            }

            if (d.C_status == "inReview")
            {
                if (!OpenSignees() || Declined())
                {
                    Line2Color = "red";
                }
                else
                {
                    Line2Color = "orange";
                }
                Line2Text = "Review";
            }

            if (d.C_status == "approved")
            {
                if (!OpenSignees() || Declined())
                {
                    Line2Color = "red";
                }
                else
                {
                    Line2Color = "orange";
                }
                Line2Text = "Acceptance";
            }

            // Weitere Zeile = userlist
            MakeSubLists();

        }

        private bool OpenSignees()
        {
            if (DocSignatureStatusList == null) return false;

            foreach (DocSignatureStatus dss in DocSignatureStatusList)
            {
                if (string.IsNullOrEmpty(dss.c_signerrole))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool OnceApproved()
        {
            foreach (DocSignatureStatus dss in DocSignatureStatusList)
            {
                if (dss.c_signerrole == "approved")
                {
                    LastApprovelDate = dss.c_verdicttime;
                    return true;
                }
            }
            return false;
        }

        public bool OnceAccepted()
        {
            foreach (DocSignatureStatus dss in DocSignatureStatusList)
            {
                if (dss.c_signerrole == "accepted")
                {
                    LastAcceptedDate = dss.c_verdicttime;
                    return true;
                }
            }
            return false;
        }

        private bool Declined()
        {
            foreach (DocSignatureStatus dss in DocSignatureStatusList)
            {
                if (dss.c_verdict == "declined")
                { 
                    return true;
                }
                if (!string.IsNullOrEmpty(dss.c_signerrole))
                {
                    return false;
                }
            }
            return false;
        }

        private void MakeSubLists()
        {
            bool insert = false;

            if (OnceAccepted())
            {
                LastAcceptDocSignatureList = new List<DocSignatureStatus>();
                foreach (DocSignatureStatus dss in DocSignatureStatusList)
                {
                    if (insert)
                    {
                        if (string.IsNullOrEmpty(dss.c_signerrole))
                        {
                            dss.Color = "royalblue";
                            LastAcceptDocSignatureList.Add(dss);
                        }
                        else
                        {
                            insert = false;
                            break;
                        }
                    }

                    if (dss.c_signerrole == "accepted")
                    {
                        insert = true;
                        dss.Color = "royalblue";
                        LastAcceptDocSignatureList.Add(dss);
                    }
                }
            }

            if (OnceApproved())
            {
                LastApprovDocSignatureList = new List<DocSignatureStatus>();
                insert = false;

                foreach (DocSignatureStatus dss in DocSignatureStatusList)
                {
                    if (insert)
                    {
                        if (string.IsNullOrEmpty(dss.c_signerrole))
                        {
                            dss.Color = "royalblue";
                            LastApprovDocSignatureList.Add(dss);
                        }
                        else
                        {
                            insert = false;
                            break;
                        }
                    }

                    if (dss.c_signerrole == "approved")
                    {
                        dss.Color = "royalblue";
                        insert = true;
                        LastApprovDocSignatureList.Add(dss);
                    }
                }
            }

            ActualDocSignatureList = new List<DocSignatureStatus>();
            foreach (DocSignatureStatus dss in DocSignatureStatusList)
            {
                if (string.IsNullOrEmpty(dss.c_signerrole))
                {
                    dss.SetColor();
                    ActualDocSignatureList.Add(dss);
                }
                else
                {
                    insert = false;
                    break;
                }
            }
        }


    }
} 