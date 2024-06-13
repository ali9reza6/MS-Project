using PolarionReports.Models.Database;
using PolarionReports.Models.Impact;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic
{
    public class ImpactBL
    {
        #region List<ImpactDoc10> AnalyzeDoc10(Impact impact, string WorkitemId)
        /// <summary>
        /// Erzeugt die Section Doc10
        /// Für jedes 10 Document wohin Requirements verknüpft sind wird die Liste "Linked Customer Requirements"
        /// erzeugt
        /// </summary>
        /// <param name="impact"></param>
        /// <param name="WorkitemId"></param>
        /// <returns></returns>
        public List<ImpactDoc10> AnalyzeDoc10(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc10> List10Doc = new List<ImpactDoc10>();

            // Uplinks in 10 Documente suchen

            // Uplinks einlesen
            if (WorkitemToAnalyze.FillCheckUplink(impact.AllUplinks) > 0)
            {
                foreach (Uplink u in WorkitemToAnalyze.Uplinks)
                {
                    Workitem tempworkitem = impact.AllWorkitems.FirstOrDefault(x => x.C_pk == u.UplinkId);
                    if (tempworkitem == null)
                    {
                        // Workitem nicht im Projekt @Todo Fehlermeldung ?
                    }
                    else
                    {
                        tempworkitem.FillCheckUplink(impact.AllUplinks);
                        if (tempworkitem.Type == "customerrequirement" && !tempworkitem.InBin)
                        {
                            // Document anlegen wenn noch nicht vorhanden
                            DocumentDB doc = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == tempworkitem.DocumentId);
                            if (doc != null)
                            {
                                ImpactDoc10 impact10 = List10Doc.FirstOrDefault(y => y.Doc10.Doc.C_pk == tempworkitem.DocumentId);
                                if (impact10 == null)
                                {
                                    impact10 = new ImpactDoc10();
                                    impact10.Doc10 = new ImpactDocument(impact.ProjectDb, doc);
                                    impact10.LinkedCustomerRequirements.Add(tempworkitem);
                                    List10Doc.Add(impact10);
                                }
                                else
                                {
                                    // Document schon in Liste -> Workitem einfügen
                                    impact10.LinkedCustomerRequirements.Add(tempworkitem);
                                }
                            }
                        }
                    }
                }
            }

            return List10Doc;
        }
        #endregion

        public List<ImpactDoc20> AnalyzeDoc20(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc20> List20Doc = new List<ImpactDoc20>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            if (WorkitemToAnalyze.CheckUplink())
            {
                documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "20");

                foreach (DocumentDB doc in documentDBs)
                {
                    ImpactDoc20 impactDoc20 = new ImpactDoc20(impact, doc);
                    impactDoc20.ElemetRequirements = impact.GetWorkitemsUp(WorkitemToAnalyze, "20", doc.C_id, "requirement", true, true);

                    if (WorkitemToAnalyze.Type == "customerrequirement")
                    {
                        impactDoc20.ElemetRequirements.AddRange(impact.GetWorkitemsDown(WorkitemToAnalyze, "20", doc.C_id, "requirement", true, true));
                        Debug.WriteLine(doc.C_id + " " + impactDoc20.ElemetRequirements.Count.ToString());
                    }

                    if (impactDoc20.ElemetRequirements.Count > 0)
                    {
                        impactDoc20.Doc20 = new ImpactDocument(impact.ProjectDb, doc);
                        List20Doc.Add(impactDoc20);
                    }
                }
                return List20Doc;
            }
            else
            {
                return List20Doc;
            }
        }

        public List<ImpactDoc30> AnalyzeDoc30(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc30> List30Doc = new List<ImpactDoc30>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();
            Workitem Tempworkitem;
            bool Reference;
            bool HeaderFound = false;

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "30" && x.C_id.Contains("Element"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc30 impactDoc30 = new ImpactDoc30();

                impactDoc30.Interfaces = impact.GetWorkitemsDown(WorkitemToAnalyze, "30", doc.C_id, "interface", true, true);
                impactDoc30.Components = impact.GetWorkitemsDown(WorkitemToAnalyze, "30", doc.C_id, "component", true, true);
                if (WorkitemToAnalyze.CheckUplink())
                {
                    impactDoc30.Interfaces.AddRange(impact.GetWorkitemsUp(WorkitemToAnalyze, "30", doc.C_id, "interface", true, true));
                    impactDoc30.Components.AddRange(impact.GetWorkitemsUp(WorkitemToAnalyze, "30", doc.C_id, "component", true, true));
                }

                if (WorkitemToAnalyze.Interface && impactDoc30.Components.Count > 0)
                {
                    // Wenn das zu analysierende Workitem eine Interface Allocierung hat, dann dürfen keine Components dran hängen
                    // Die eventuell gefundenen Components sind falsch!!
                  
                }

                if (impactDoc30.Interfaces.Count == 0 && impactDoc30.Components.Count == 0 && !HeaderFound)
                {
                    // keine Interfaces oder Components über Links gefunden -> Parent Level 0 untersuchen
                    Tempworkitem = impact.GetParentLevel0(WorkitemToAnalyze, out Reference);
                    if (!Reference)
                    {
                        if (Tempworkitem.Type == "interface")
                        {
                            impactDoc30.Interfaces.Add(Tempworkitem);
                            HeaderFound = true;
                        }
                        if (Tempworkitem.Type == "component")
                        {
                            impactDoc30.Components.Add(Tempworkitem);
                            HeaderFound = true;
                        }
                    }
                }


                if (impactDoc30.Interfaces.Count > 0 || impactDoc30.Components.Count > 0)
                {
                    impactDoc30.Doc30 = new ImpactDocument(impact.ProjectDb, doc);
                    List30Doc.Add(impactDoc30);
                }
            }
            
            return List30Doc;
        }

        public List<ImpactDoc30i> AnalyzeDoc30i(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc30i> List30i = new List<ImpactDoc30i>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "30" && x.C_id.Contains("Interface"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc30i impactDoc30i = new ImpactDoc30i();
                impactDoc30i.InterfaceRequirements = impact.GetWorkitemsDown(WorkitemToAnalyze, "30", doc.C_id, "interface", true, true);
                if (impactDoc30i.InterfaceRequirements.Count > 0)
                {
                    impactDoc30i.Doc30i = new ImpactDocument(impact.ProjectDb, doc);
                    List30i.Add(impactDoc30i);
                }
            }
                
            return List30i;
        }

        public List<ImpactDoc40hw> AnalyzeDoc40hw(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc40hw> List40hw = new List<ImpactDoc40hw>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "40" && x.C_id.Contains("Hardware"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc40hw impactDoc40hw = new ImpactDoc40hw();
                // Hardware Requirements
                impactDoc40hw.HardwareRequrirements = impact.GetWorkitemsDown(WorkitemToAnalyze, "40", doc.C_id, "requirement", false, true);

                if (impactDoc40hw.HardwareRequrirements.Count > 0)
                {
                    impactDoc40hw.Doc40hw = new ImpactDocument(impact.ProjectDb, doc);
                    List40hw.Add(impactDoc40hw);
                }
            }
                
            return List40hw;
        }

        public List<ImpactDoc40sw> AnalyzeDoc40sw(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc40sw> List40sw = new List<ImpactDoc40sw>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "40" && x.C_id.Contains("Software"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc40sw impactDoc40sw = new ImpactDoc40sw();
                // Software Requirements
                impactDoc40sw.SoftwareRequirements = impact.GetWorkitemsDown(WorkitemToAnalyze, "40", doc.C_id, "requirement", true, false);

                if (impactDoc40sw.SoftwareRequirements.Count > 0)
                {
                    impactDoc40sw.Doc40sw = new ImpactDocument(impact.ProjectDb, doc);
                    List40sw.Add(impactDoc40sw);
                }
            }

            return List40sw;
        }

        public List<ImpactDoc50hw> AnalyzeDoc50hw(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc50hw> List50hw = new List<ImpactDoc50hw>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "50" && x.C_id.Contains("Hardware"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc50hw impactDoc50hw = new ImpactDoc50hw();
                // Interfaces
                impactDoc50hw.Interfaces = impact.GetWorkitemsDown(WorkitemToAnalyze, "50", doc.C_id, "interface", false, true);

                // Components
                impactDoc50hw.Components = impact.GetWorkitemsDown(WorkitemToAnalyze, "50", doc.C_id, "component", false, true);

                if (impactDoc50hw.Interfaces.Count > 0 || impactDoc50hw.Components.Count > 0)
                {
                    impactDoc50hw.Doc50hw = new ImpactDocument(impact.ProjectDb, doc);
                    List50hw.Add(impactDoc50hw);
                }
            }

            return List50hw;
        }

        public List<ImpactDoc50sw> AnalyzeDoc50sw(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactDoc50sw> List50sw = new List<ImpactDoc50sw>();
            List<DocumentDB> documentDBs = new List<DocumentDB>();

            documentDBs = impact.DocumentsDB.FindAll(x => x.C_id.Substring(0, 2) == "50" && x.C_id.Contains("Software"));

            foreach (DocumentDB doc in documentDBs)
            {
                ImpactDoc50sw impactDoc50sw = new ImpactDoc50sw();
                // Interfaces
                impactDoc50sw.Interfaces = impact.GetWorkitemsDown(WorkitemToAnalyze, "50", doc.C_id, "interface", true, false);

                // Components
                impactDoc50sw.Components = impact.GetWorkitemsDown(WorkitemToAnalyze, "50", doc.C_id, "component", true, false);

                if (impactDoc50sw.Interfaces.Count > 0 || impactDoc50sw.Components.Count > 0)
                {
                    impactDoc50sw.Doc50sw = new ImpactDocument(impact.ProjectDb, doc);
                    List50sw.Add(impactDoc50sw);
                }
            }

            return List50sw;
        }

        public List<ImpactFeature> AnalyzeFeatures(Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactFeature> features = new List<ImpactFeature>();
            Workitem Tempworkitem;
            List<Workitem> TempWorkitemList = new List<Workitem>();

            ImpactFeature impactFeature = new ImpactFeature();
            impactFeature.FeatureDocument = new ImpactDocument();
            impactFeature.FeatureDocument.Doc.C_id = "Feature";

            // Planning Feature Workitemtype = "feature"
            WorkitemToAnalyze.FillCheckDownlink(impact.AllUplinks);
            foreach (Uplink u in WorkitemToAnalyze.Downlinks)
            {
                Tempworkitem = impact.AllWorkitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem != null)
                {
                    Tempworkitem.FillCheckUplink(impact.AllUplinks);
                    if (Tempworkitem.InBin) continue;

                    if (Tempworkitem.Type == "feature" || Tempworkitem.Type == "productFeature")
                    {
                        if (Tempworkitem.DocumentId == 0)
                        {
                            // Feature ist ein Workitem ohne Dokument
                            impactFeature.Features.Add(Tempworkitem);
                        }
                        else
                        {
                            // PlanedIn in Workitem eintragen
                            impact.PlanWorkitemLinkList.SetPlannedId(Tempworkitem);
                            DocumentDB doc = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == Tempworkitem.DocumentId);
                            if (doc == null)
                            {
                                // Dokument nicht im Project
                            }
                            else
                            {
                                ImpactFeature tf = features.FirstOrDefault(x => x.FeatureDocument.Doc.C_pk == Tempworkitem.DocumentId);
                                if (tf == null)
                                {
                                    // Neues Document:
                                    tf = new ImpactFeature();
                                    tf.FeatureDocument = new ImpactDocument(impact.ProjectDb, doc);
                                    tf.Features = new List<Workitem>();
                                    tf.Features.Add(Tempworkitem);
                                    features.Add(tf);
                                }
                                else
                                {
                                    tf.Features.Add(Tempworkitem);
                                }
                            }
          
                        }

                    }
                }
            }
            if (impactFeature.Features.Count > 0)
            {
                features.Add(impactFeature);
            }

            return features;
        }

        public List<ImpactTestcase> AnalyzeTestcases(Impact impact, Workitem WorkitemToAnalyze)
        {
            DatareaderP dr = new DatareaderP();
            List<ImpactTestcase> testcases = new List<ImpactTestcase>();
            Workitem Tempworkitem;
            List<Workitem> TempWorkitemList = new List<Workitem>();

            ImpactTestcase impactTestcase = new ImpactTestcase();
            impactTestcase.TestcaseDocument = new ImpactDocument();
            impactTestcase.TestcaseDocument.Doc.C_id = "Testcase";

            // Planning Feature Workitemtype = "feature"
            WorkitemToAnalyze.FillCheckDownlink(impact.AllUplinks);
            foreach (Uplink u in WorkitemToAnalyze.Downlinks)
            {
                Tempworkitem = impact.AllWorkitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem != null)
                {
                    Tempworkitem.FillCheckUplink(impact.AllUplinks);
                    if (Tempworkitem.Type == "testcase" && !Tempworkitem.InBin && Tempworkitem.Status != "rejected")
                    {
                        Tempworkitem.TestcaseResults = dr.GetTestcaseResults(Tempworkitem.Id, out string error);
                        if (Tempworkitem.TestcaseResults.Count > 0)
                        {
                            Tempworkitem.TestResult = Tempworkitem.TestcaseResults[0].c_result;
                        }
                        else
                        {
                            Tempworkitem.TestResult = "no testrun";
                        }
                        if (Tempworkitem.DocumentId == 0)
                        {
                            // Feature ist ein Workitem ohne Dokument
                            impactTestcase.Testcases.Add(Tempworkitem);
                        }
                        else
                        {
                            DocumentDB doc = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == Tempworkitem.DocumentId);
                            if (doc == null)
                            {
                                // Dokument nicht im Project
                            }
                            else
                            {
                                ImpactTestcase tt = testcases.FirstOrDefault(x => x.TestcaseDocument.Doc.C_pk == Tempworkitem.DocumentId);
                                if (tt == null)
                                {
                                    // Neues Document:
                                    tt = new ImpactTestcase();
                                    tt.TestcaseDocument = new ImpactDocument(impact.ProjectDb, doc);
                                    tt.Testcases.Add(Tempworkitem);
                                    testcases.Add(tt);
                                }
                                else
                                {
                                    tt.Testcases.Add(Tempworkitem);
                                }
                            }
                        }
                    }
                }
            }
            if (impactTestcase.Testcases.Count > 0)
            {
                // Falls Workitems ohne Document vorhanden waren:
                testcases.Add(impactTestcase);
            }

            dr.CloseConnection();
            return testcases;
        }

        public List<ImpactWorkPackage> AnalyzeWorkPackage (Impact impact, Workitem WorkitemToAnalyze)
        {
            List<ImpactWorkPackage> workPackages = new List<ImpactWorkPackage>();
            Workitem Tempworkitem;
            List<Workitem> TempWorkitemList = new List<Workitem>();

            ImpactWorkPackage impactWorkPackage = new ImpactWorkPackage();
            impactWorkPackage.WorkPackageDocument = new ImpactDocument();
            impactWorkPackage.WorkPackageDocument.Doc.C_id = "Workpackage";

            WorkitemToAnalyze.FillCheckDownlink(impact.AllUplinks);
            foreach (Uplink u in WorkitemToAnalyze.Downlinks)
            {
                Tempworkitem = impact.AllWorkitems.FirstOrDefault(x => x.C_pk == u.WorkitemId);
                if (Tempworkitem != null)
                {
                    Tempworkitem.FillCheckUplink(impact.AllUplinks);
                    if (Tempworkitem.InBin) continue;
                    if (Tempworkitem.Type == "workPackage")
                    {
                        if (Tempworkitem.DocumentId == 0)
                        {
                            // Feature ist ein Workitem ohne Dokument
                            impactWorkPackage.WorkPackages.Add(Tempworkitem);
                        }
                        else
                        {
                            // PlanedIn in Workitem eintragen
                            impact.PlanWorkitemLinkList.SetPlannedId(Tempworkitem);
                            DocumentDB doc = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == Tempworkitem.DocumentId);
                            if (doc == null)
                            {
                                // Dokument nicht im Project
                            }
                            else
                            {
                                ImpactWorkPackage wp = workPackages.FirstOrDefault(x => x.WorkPackageDocument.Doc.C_pk == Tempworkitem.DocumentId);
                                if (wp == null)
                                {
                                    // Neues Document:
                                    wp = new ImpactWorkPackage();
                                    wp.WorkPackageDocument = new ImpactDocument(impact.ProjectDb, doc);
                                    wp.WorkPackages.Add(Tempworkitem);
                                    workPackages.Add(wp);
                                }
                                else
                                {
                                    wp.WorkPackages.Add(Tempworkitem);
                                }
                            }
                        }
                    }
                }
            }
            if (impactWorkPackage.WorkPackages.Count > 0)
            {
                workPackages.Add(impactWorkPackage);
            }
            return workPackages;
        }

        public List<ImpactChild> AnalyzeChilds (Impact impact, List<Workitem> childs)
        {
            List<ImpactChild> ChildDocs = new List<ImpactChild>();
            ImpactChild impactChild = new ImpactChild();
            impactChild.ChildDocument = new ImpactDocument();
            impactChild.ChildDocument.Doc.C_id = "Childs";

            foreach (Workitem child in childs)
            {
                if (child.DocumentId == 0)
                {
                    // Feature ist ein Workitem ohne Dokument
                    impactChild.Childs.Add(child);
                }
                else
                {
                    DocumentDB doc = impact.DocumentsDB.FirstOrDefault(x => x.C_pk == child.DocumentId);
                    if (doc == null)
                    {
                        // Dokument nicht im Project
                    }
                    else
                    {
                        ImpactChild c = ChildDocs.FirstOrDefault(x => x.ChildDocument.Doc.C_pk == child.DocumentId);
                        if (c == null)
                        {
                            // Neues Document:
                            c = new ImpactChild();
                            c.ChildDocument = new ImpactDocument(impact.ProjectDb, doc);
                            c.Childs.Add(child);
                            ChildDocs.Add(c);
                        }
                        else
                        {
                            c.Childs.Add(child);
                        }
                    }
                }
            }
            if (impactChild.Childs.Count > 0)
            {
                ChildDocs.Add(impactChild);
            }
            return ChildDocs;
        }

        public List<Workitem> GetAllChilds(Impact impact, Workitem w)
        {
            List<Workitem> wl = new List<Workitem>();
            Workitem Tempworkitem;

            if (w.Downlinks == null)
            {
                w.FillCheckDownlink(impact.AllUplinks);
            }
            foreach (Uplink d in w.Downlinks)
            {
                if (d.Role == "parent")
                {
                    Tempworkitem = impact.AllWorkitems.FirstOrDefault(x => x.C_pk == d.WorkitemId);
                    wl.Add(Tempworkitem);
                    wl.AddRange(GetAllChilds(impact, Tempworkitem));
                }
            }

            return wl;
        }
    }
}