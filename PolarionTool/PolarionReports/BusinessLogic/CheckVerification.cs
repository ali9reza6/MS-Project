using PolarionReports.Models.Database;
using PolarionReports.Models.TableRows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using PolarionReports.Custom_Helpers;

namespace PolarionReports.BusinessLogic
{
    public class CheckVerification
    {
        public void Check (DatareaderP dr, Project p, Document d, Workitem w)
        {
            Document testcaseDocument;

            if (w.Id == "RT-2594")
            {
                Debug.WriteLine(w.Id);
            }

            Workitem tempWorkitem;

            if (w.Type == "requirement")
            {
                if (w.Downlinks == null) return;
                if (w.Uplinks == null) return;

                List<string> verificationMethods = dr.GetWorkitemCustomFieldString(w.C_pk, "verificationMethod", out string error);
                List<string> verificationDisciplins = dr.GetWorkitemCustomFieldString(w.C_pk, "verificationDiscipline", out string error2);

                RequTestcase requTestcase = new RequTestcase();
                requTestcase.Requirement = w;
                requTestcase.Testcases = new List<Workitem>();
                requTestcase.VerificationDiscipline = "";
                requTestcase.VerificationMethod = "";

                RequTestcaseError requTestcaseError = new RequTestcaseError();
                requTestcaseError.Requirement = w;
                requTestcaseError.Testcases = new List<Workitem>();
                requTestcaseError.VerificationDiscipline = "";
                requTestcaseError.VerificationMethod = "";

                RequTestcase missingverificationProperties = new RequTestcase();
                missingverificationProperties.Testcases = new List<Workitem>();

                if (verificationMethods.Count == 0)
                {
                    missingverificationProperties.Requirement = w;
                }

                if (verificationMethods.Count == 0 && verificationDisciplins.Count == 0)
                {
                    missingverificationProperties.Requirement = w;
                }

                if (verificationDisciplins.Count == 0 && verificationMethods.Contains("none"))
                {
                    missingverificationProperties.Requirement = w;
                    missingverificationProperties.VerificationMethod = "none";
                }

                foreach (Uplink link in w.Downlinks)
                {
                    tempWorkitem = p.Workitems.FirstOrDefault(x => x.C_pk == link.WorkitemId);
                    if (tempWorkitem != null && tempWorkitem.Type == "testcase")
                    {
                        // Document des Testcases suchen
                        testcaseDocument = p.Documents.FirstOrDefault(x => x.C_pk == tempWorkitem.DocumentId);
                        if (testcaseDocument != null)
                        {
                            // Links in das 30 Review Document ignorieren (sind bereits in anderer Liste)
                            if (testcaseDocument.DocName.OriginalDocumentname.Contains("Review"))
                            {
                                continue;
                            }
                            tempWorkitem.DocPrefix = testcaseDocument.DocumentLevel + testcaseDocument.DucumentIndex;
                            tempWorkitem.PolarionDocumentLink = Custom_Helper_Class.GetPolarionDocumentLink(p.C_id, testcaseDocument.C_modulefolder, testcaseDocument.C_id) + tempWorkitem.Id;
                        }

                        requTestcase.Testcases.Add(tempWorkitem);

                        if (missingverificationProperties.Requirement != null)
                        {
                            missingverificationProperties.Testcases.Add(tempWorkitem);
                        }
                    }
                    Debug.WriteLine(tempWorkitem.Id + " " + tempWorkitem.Type);
                }

                foreach (Uplink link in w.Uplinks)
                {
                    tempWorkitem = p.Workitems.FirstOrDefault(x => x.C_pk == link.UplinkId);
                    if (tempWorkitem != null && tempWorkitem.Type == "testcase")
                    {
                        // Document des Testcases suchen
                        testcaseDocument = p.Documents.FirstOrDefault(x => x.C_pk == tempWorkitem.DocumentId);
                        if (testcaseDocument != null)
                        {
                            tempWorkitem.DocPrefix = testcaseDocument.DocumentLevel + testcaseDocument.DucumentIndex;
                            tempWorkitem.PolarionDocumentLink = Custom_Helper_Class.GetPolarionDocumentLink(p.C_id, testcaseDocument.C_modulefolder, testcaseDocument.C_id) + tempWorkitem.Id;
                        }
                        requTestcaseError.Testcases.Add(tempWorkitem);
                        requTestcaseError.Error += "Link Dir";
                    }
                }

                // Erzeugte Elemente in verschiedene Listen eintragen:

                foreach(string s in verificationDisciplins)
                {
                    if (requTestcase.VerificationDiscipline.Length > 0) requTestcase.VerificationDiscipline += ", ";
                    requTestcase.VerificationDiscipline += s;

                    if (requTestcaseError.VerificationDiscipline.Length > 0) requTestcaseError.VerificationDiscipline += ", ";
                    requTestcaseError.VerificationDiscipline += s;
                }

                foreach (string s in verificationMethods)
                {
                    if (requTestcase.VerificationMethod.Length > 0) requTestcase.VerificationMethod += ", ";
                    requTestcase.VerificationMethod += s;

                    if (requTestcaseError.VerificationMethod.Length > 0) requTestcaseError.VerificationMethod += ", ";
                    requTestcaseError.VerificationMethod += s;
                }

                // 1.) Unlinked (Element) Requirements
                if (requTestcase.Testcases.Count == 0) // && missingverificationProperties.Requirement == null)
                {
                    // in Liste Unlinked: keine Testcases (2019-11-25 Properties in getrennter Fehlerliste (und nicht in missing Verification)
                    RequTestcaseUnlinked requTestcaseUnlinked = new RequTestcaseUnlinked();
                    requTestcaseUnlinked.Requirement = requTestcase.Requirement;
                    requTestcaseUnlinked.VerificationDiscipline = "";
                    requTestcaseUnlinked.VerificationMethod = "";
                    foreach (string s in verificationDisciplins)
                    {
                        if (requTestcaseUnlinked.VerificationDiscipline.Length > 0) requTestcaseUnlinked.VerificationDiscipline += ", ";
                        requTestcaseUnlinked.VerificationDiscipline += s;
                    }
                    foreach (string s in verificationMethods)
                    {
                        if (requTestcaseUnlinked.VerificationMethod.Length > 0) requTestcaseUnlinked.VerificationMethod += ", ";
                        requTestcaseUnlinked.VerificationMethod += s;
                    }
                    d.UnlinkedElementRequirements.Add(requTestcaseUnlinked);
                }

                // 2.) Incorrectly linked (Element) Requirements 
                if (requTestcaseError.Testcases.Count > 0)
                {
                    d.IncorrectlyLinkedElementRequirementsToTestcases.Add(requTestcaseError);
                }

                if (verificationMethods.Contains("none") && requTestcase.Testcases.Count > 0)
                {
                    // verification Method = "none" und es sind Testcases verknüpft
                    RequTestcaseError linkedTC = new RequTestcaseError();
                    linkedTC.Requirement = requTestcase.Requirement;
                    linkedTC.Testcases = requTestcase.Testcases;
                    linkedTC.Error = "linked TC";
                    linkedTC.VerificationMethod = "";
                    linkedTC.VerificationDiscipline = "";
                    foreach (string s in verificationDisciplins)
                    {
                        if (linkedTC.VerificationDiscipline.Length > 0) linkedTC.VerificationDiscipline += ", ";
                        linkedTC.VerificationDiscipline += s;
                    }
                    foreach (string s in verificationMethods)
                    {
                        if (linkedTC.VerificationMethod.Length > 0) linkedTC.VerificationMethod += ", ";
                        linkedTC.VerificationMethod += s;
                    }
                    d.IncorrectlyLinkedElementRequirementsToTestcases.Add(linkedTC);
                }

                // 3.) Linked (Element) Requirements 
                if (requTestcase.Testcases.Count > 0)
                {
                    d.LinkedElementRequirementsToTestcases.Add(requTestcase);
                }

                // 4.) (Element) Requirements with missing Verification Properties
                if (missingverificationProperties.Requirement != null)
                {
                    if (missingverificationProperties.Requirement.IsHSM())
                    {
                        d.ElementRequirementsWithMissingVerificationProperties.Add(missingverificationProperties);
                    }
                }

            }
        }
    }
}