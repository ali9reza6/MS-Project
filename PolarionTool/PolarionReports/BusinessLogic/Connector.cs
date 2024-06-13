namespace PolarionReports.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using net.seabay.polarion;
    using net.seabay.polarion.Builder;
    using net.seabay.polarion.Planning;
    using net.seabay.polarion.Project;
    using net.seabay.polarion.Security;
    using net.seabay.polarion.Session;
    using net.seabay.polarion.TestManagement;
    using net.seabay.polarion.Tracker;

    public class Connection
    {
        #region Constructor
        public Connection(Uri baseUri)
        {
            Uri = baseUri;

            this.Factory = new WSConnector(baseUri);
            this.Factory.Connect();

            this.IsLoggedIn = false;
        }

        public Connection(string protocol, string server)
        {
            this.Protocol = protocol;
            this.Server = server;

            this.IsLoggedIn = false;

            this.Factory = new WSConnector(protocol, server);
            this.Factory.Connect();
        }

        #endregion Constructor

        #region public Properties

        public string Protocol
        {
            get;
            private set;
        }

        public string Server
        {
            get;
            private set;
        }

        public string UserID
        {
            get { return this.PolarionUser.id; }
        }

        public string UserName
        {
            get { return this.PolarionUser.name; }
        }

        public bool IsLoggedIn
        {
            get;
            private set;
        }

        public Uri Uri
        {
            get;
            set;
        }

        #endregion public Properties

        // WebServices
        public BuilderWebServiceClient Builder
        {
            get { return this.Factory.WebServices[WebServiseFactory.Builder]; }
        }

        public ProjectWebServiceClient Project
        {
            get { return this.Factory.WebServices[WebServiseFactory.Project]; }
        }

        public PlanningWebServiceClient Planning
        {
            get { return this.Factory.WebServices[WebServiseFactory.Planning]; }
        }

        public SecurityWebServiceClient Security
        {
            get { return this.Factory.WebServices[WebServiseFactory.Security]; }
        }

        public SessionWebServiceClient Session
        {
            get { return this.Factory.WebServices[WebServiseFactory.Session]; }
        }

        public TestManagementWebServiceClient TestManagement
        {
            get { return this.Factory.WebServices[WebServiseFactory.TestManagement]; }
        }

        public TrackerWebServiceClient Tracker
        {
            get { return this.Factory.WebServices[WebServiseFactory.Tracker]; }
        }

        #region private Properties

        private WSConnector Factory
        {
            get;
            set;
        }

        private net.seabay.polarion.Project.User PolarionUser
        {
            get;
            set;
        }

        private string Password { get; set; }

        #endregion private Properties

        public void Login(string user, string password)

        {
            try
            {
                this.Session.logIn(user, password);
                this.PolarionUser = this.Project.getUser(user);
                this.IsLoggedIn = true;
            }
            catch (Exception e)
            {
                // user could not log in
                Console.WriteLine("" + e, ToString());
                this.IsLoggedIn = false;
            }
        }
    }
}
