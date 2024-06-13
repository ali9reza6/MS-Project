using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.BusinessLogic
{
    /// <summary>
    /// Überprüfung der 30 Review Architectural Design Checklist auf Vollständigkeit
    /// </summary>
    public class Check30Review
    {
        public void Check(Project p, Document d)
        {
            Document Review30Doc;
            List<Workitem> TestCases;
            Workitem LinkedWorkitem;

            d.TestcasesNotLinkedToElementRequirements = new List<Workitem>();
            d.TestcasesLinkedToElementRequirements = new List<WorkitemLinkError>();

            // Review Architectual Design Document suchen
            Review30Doc = p.GetDocument("30" + d.DocName.Prefix, "Review Architectural Design");
            if (Review30Doc == null)
            {
                // Fehler Document nicht gefunden
                return;
            }

            d.Checklist = Review30Doc;
            d.Checklist.PolarionTableLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{p.C_id}/wiki/Verification/"
                + System.Uri.EscapeDataString(d.Checklist.C_id) + "?query=";
            d.PolarionChecklistLink = "http://" + Topol.PolarionServer + $"/polarion/#/project/{p.C_id}/wiki/Verification/{Review30Doc.C_id}?selection=";

            TestCases = p.Workitems.FindAll(w => w.DocumentId == Review30Doc.C_pk && w.Type == "testcase");
            
            // Alle Workitems des Typs "Test Case" dieses Documents untersuchen

            foreach(Workitem w in TestCases)
            {
                // Workitems mit Schlüsselwort xxx ignorieren
                if (IgnoreTestcase(w.Title))
                {
                    continue;
                }
                // Link zu 20x Document überprüfen

                if (w.Id == "E18008-9287")
                {
                    Debug.WriteLine(w.Id);
                }
                
                if (w.InBin)
                {
                    // gelöschte Workitems ignorieren
                    continue;
                }
                LinkedWorkitem = w.CheckLinkFrom(p, "20" + d.DocName.Prefix, "Element Requirements", "relates_to", "requirement");

                if (LinkedWorkitem == null)
                {
                    // kein Link vorhanden
                    d.TestcasesNotLinkedToElementRequirements.Add(w);
                }
                else
                {
                    if (LinkedWorkitem.ESE && LinkedWorkitem.Hardware)
                    {
                        // Link OK
                        WorkitemLinkError wl = new WorkitemLinkError();
                        wl.Workitem = w;
                        wl.LinkedWorkitem = LinkedWorkitem;
                        wl.PolarionLink = p.GetPolarionLink(d.C_id, LinkedWorkitem.Id);
                        d.TestcasesLinkedToElementRequirements.Add(wl);
                    }
                    else
                    {
                        // Link auf falsches Requirement
                        d.TestcasesNotLinkedToElementRequirements.Add(w);
                    }
                }

            }
        }

        #region bool IgnoreTestcase(string Title)
        /// <summary>
        /// Zu ignorierende testcases feststellen
        /// </summary>
        /// <param name="Title"></param>
        /// <returns>
        /// true .... Ignore Testcate
        /// false ... Check Testcase
        /// </returns>
        private bool IgnoreTestcase(string Title)
        {
            string c1 = "BASIC";
            string c2 = "INT REQU";
            string c3 = "GEN REQU";

            Debug.WriteLine(Title);

            if (Title == null)
            {
                return true;
            }

            if (Title.Length < 5 )
            {
                return true;
            }

            if (Title.Substring(0, c1.Length) == c1)
            {
                return true;
            }

            if (Title.Length > 8)
            {
                if (Title.Substring(0, c2.Length) == c2)
                {
                    return true;
                }

                if (Title.Substring(0, c3.Length) == c3)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}