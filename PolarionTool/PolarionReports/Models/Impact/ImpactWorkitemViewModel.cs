using PolarionReports.Models.Database;
using PolarionReports.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactWorkitemViewModel
    {
        public ProjectDB ProjectDB { get; set; }
        public List<WorkitemAnalyze> WorkitemsToAnalyse { get; set; }
        public string ErrorMsg { get; set; }
        public string Polarionlink { get; set; }

        public WorkitemView WorkitemView { get; set; }
        public RequAnalyzeViewModel RequList { get; set; }
        public RequAnalyzeViewModel RequAnalyzeList { get; set; }
        public CustReqViewModel CustRequList { get; set; }
        public FeatureViewModel FeatureList { get; set; }
        public TcViewModel TCList { get; set; }
        public CIViewModel CIList { get; set; }
        public WorkPackageViewModel WPList { get; set; }

        public ImpactWorkitemViewModel(string ProjectID)
        {
            DatareaderP dr;
    
            dr = new DatareaderP();
            ProjectDB = dr.GetProjectByID(ProjectID, out string Error);
            ErrorMsg = Error + ", ";

            this.WorkitemsToAnalyse = new List<WorkitemAnalyze>();

            WorkitemView = new WorkitemView();
            RequList = new RequAnalyzeViewModel();
            RequAnalyzeList = new RequAnalyzeViewModel();
            CustRequList = new CustReqViewModel();
            FeatureList = new FeatureViewModel();
            TCList = new TcViewModel();
            CIList = new CIViewModel();
            WPList = new WorkPackageViewModel();
        }
    }
}