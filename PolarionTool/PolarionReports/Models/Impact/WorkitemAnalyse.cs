using PolarionReports.Models.Impact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class WorkitemAnalyze
    {
        public Workitem WorkitemStartingPoint { get; set; }
        public Workitem Workitem { get; set; }
        public DocumentDB OriginalDocument { get; set; }
        /// <summary>
        /// Text für Listenausgabe "Element Requirement"|"Requirement"| ...
        /// </summary>
        public string WorkitemHeader { get; set; }
        public string TooltipWorkitem { get; set; }
        public List<Workitem> ChildWorkitems { get; set; }
        public string TooltipChildWorkitems { get; set; }

        public List<ImpactChild> ChildDocs { get; set; }
        public List<ImpactDoc10> Docs10 { get; set; }
        public List<ImpactDoc20> Docs20 { get; set; }
        public List<ImpactDoc30> Docs30 { get; set; }
        public List<ImpactDoc30i> Docs30i { get; set; }
        public List<ImpactDoc40sw> Docs40sw { get; set; }
        public List<ImpactDoc50sw> Docs50sw { get; set; }
        public List<ImpactDoc40hw> Docs40hw { get; set; }
        public List<ImpactDoc50hw> Docs50hw { get; set; }
        public List<ImpactTestcase> Testcases { get; set; }
        public List<ImpactFeature> Features { get; set; }
        public List<ImpactWorkPackage> WorkPackages { get; set; }

        public WorkitemAnalyze(string WorkitemID)
        {
            // Fill all Lists with data
            ChildWorkitems = new List<Workitem>();
            ChildDocs = new List<ImpactChild>();
            Docs10 = new List<ImpactDoc10>();
            Docs20 = new List<ImpactDoc20>();
            Docs30 = new List<ImpactDoc30>();
            Docs30i = new List<ImpactDoc30i>();
            Docs40sw = new List<ImpactDoc40sw>();
            Docs40hw = new List<ImpactDoc40hw>();
            Docs50sw = new List<ImpactDoc50sw>();
            Docs50hw = new List<ImpactDoc50hw>();
            Testcases = new List<ImpactTestcase>();
            Features = new List<ImpactFeature>();
            WorkPackages = new List<ImpactWorkPackage>();
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "Linked Customer Requirements"
        /// </summary>
        public int GetCounterLinkedCustomerRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc10 doc10 in Docs10)
                {
                    counter += doc10.LinkedCustomerRequirements.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "Element Requirements"
        /// </summary>
        public int GetCounterLinkedElementRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc20 doc20 in Docs20)
                {
                    counter += doc20.ElemetRequirements.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Liste aller Workitems aus verschiedenen Dokumenten
        /// </summary>
        public List<Workitem> GetAllElementRequirements
        {
            get
            {
                List<Workitem> AllElementRequirements = new List<Workitem>();
                foreach (ImpactDoc20 doc20 in Docs20)
                {
                    AllElementRequirements.AddRange(doc20.ElemetRequirements);
                }
                return AllElementRequirements;
            }
        }




        /// <summary>
        /// Gesamtzahl der Wokitems in "Child Requirements"
        /// </summary>
        public int GetCounterChildRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactChild child in ChildDocs)
                {
                    counter += child.Childs.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Liste aller Childs aus verschiedenen Dokumenten
        /// </summary>
        public List<Workitem> GetAllChilds
        {
            get
            {
                List<Workitem> AllChilds = new List<Workitem>();
                foreach (ImpactChild child in ChildDocs)
                {
                    AllChilds.AddRange(child.Childs);
                }
                return AllChilds;
            }
        }

        /// <summary>
        /// Gesamtzahl der Wokitems in "Test Cases"
        /// </summary>
        public int GetCounterTestCases
        {
            get
            {
                int counter = 0;
                foreach (ImpactTestcase testcase in Testcases)
                {
                    counter += testcase.Testcases.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Wokitems in "Features"
        /// </summary>
        public int GetCounterFeatures
        {
            get
            {
                int counter = 0;
                foreach (ImpactFeature feature in Features)
                {
                    counter += feature.Features.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Liste aller Features aus verschiedenen Dokumenten
        /// </summary>
        public List<Workitem> GetAllFeatures
        {
            get
            {
                List<Workitem> AllFeatures = new List<Workitem>();
                foreach (ImpactFeature feature in Features)
                {
                    AllFeatures.AddRange(feature.Features);
                }
                return AllFeatures;
            }
        }

        /// <summary>
        /// Gesamtzahl der Wokitems in "Workpackeges"
        /// </summary>
        public int GetCounterWorkpackages
        {
            get
            {
                int counter = 0;
                foreach (ImpactWorkPackage workpackage in WorkPackages)
                {
                    counter += workpackage.WorkPackages.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Liste aller Workpackeges aus verschiedenen Dokumenten
        /// </summary>
        public List<Workitem> GetAllWP
        {
            get
            {
                List<Workitem> AllWP = new List<Workitem>();
                foreach (ImpactWorkPackage workpackage in WorkPackages)
                {
                    AllWP.AddRange(workpackage.WorkPackages);
                }
                return AllWP;
            }
        }


        /// <summary>
        /// Gesamtzahl der Workitems in "Components"
        /// </summary>
        public int GetCounterComponents
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc30 doc30 in Docs30)
                {
                    counter += doc30.Components.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "Components"
        /// </summary>
        public int GetCounterInterfaces
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc30 doc30 in Docs30)
                {
                    counter += doc30.Interfaces.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "Components"
        /// </summary>
        public int GetCounterInterfaceRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc30i doc30i in Docs30i)
                {
                    counter += doc30i.InterfaceRequirements.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "40 Software Requirements"
        /// </summary>
        public int GetCounter40SoftwareRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc40sw doc40sw in Docs40sw)
                {
                    counter += doc40sw.SoftwareRequirements.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Interface-Workitems in "50 Software Requirements"
        /// </summary>
        public int GetCounter50SWInterfaces
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc50sw doc50sw in Docs50sw)
                {
                    counter += doc50sw.Interfaces.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Component-Workitems in "50 Software Requirements"
        /// </summary>
        public int GetCounter50SWComponent
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc50sw doc50sw in Docs50sw)
                {
                    counter += doc50sw.Components.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Workitems in "40 Hardware Requirements"
        /// </summary>
        public int GetCounter40HardwareRequirements
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc40hw doc40hw in Docs40hw)
                {
                    counter += doc40hw.HardwareRequrirements.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Interface-Workitems in "50 Software Requirements"
        /// </summary>
        public int GetCounter50HWInterfaces
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc50hw doc50hw in Docs50hw)
                {
                    counter += doc50hw.Interfaces.Count;
                }
                return counter;
            }
        }

        /// <summary>
        /// Gesamtzahl der Component-Workitems in "50 Software Requirements"
        /// </summary>
        public int GetCounter50HWComponent
        {
            get
            {
                int counter = 0;
                foreach (ImpactDoc50hw doc50hw in Docs50hw)
                {
                    counter += doc50hw.Components.Count;
                }
                return counter;
            }
        }

    }
}