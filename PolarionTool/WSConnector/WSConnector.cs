using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Web.Services.Configuration;

using net.seabay.polarion.Builder;
//added for testing
using net.seabay.polarion.Planning;
using net.seabay.polarion.Project;
using net.seabay.polarion.Security;
using net.seabay.polarion.Session;
using net.seabay.polarion.TestManagement;
using net.seabay.polarion.Tracker;

namespace net.seabay.polarion
{
    public enum WebServiseFactory
    {
        Builder = 0,
        Planning = 1,
        Project = 2,
        Security = 3,
        Session = 4,
        TestManagement = 5,
        Tracker = 6,
    }

    public class WSConnector
    {

        #region Properties
        private Uri PolarionUri { get; set; }
        public Dictionary<WebServiseFactory, dynamic> WebServices { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Constructs the class from an Uri.
        /// </summary>
        /// <param name="uri">The Uri which provides the web services.</param>
        public WSConnector(Uri uri)
        {
            PolarionUri = new Uri(uri, Strings.BASEWSURL);
        }
        /// <summary>
        /// Constructs the class from the protocol and the server.
        /// </summary>
        /// <param name="protocol">The protocol of the Uri like http:// or https://.</param>
        /// <param name="serverUrl">The server of the Uri including the port number.</param>
        public WSConnector(string protocol, string serverUrl)
        {
            StringBuilder url = new StringBuilder(protocol);
            url.Append(serverUrl);
            PolarionUri = new Uri(new Uri(url.ToString()), Strings.BASEWSURL);
        }
        #endregion Constructor

        #region public Methods
        public void Connect()
        {
            //  check if the web serivces are already exits
            if (WebServices != null)
            {
                return;
            }

            WebServices = new Dictionary<WebServiseFactory, dynamic>();

            //  instantiate the behavior class
            SessionIdBehavior behavior = new SessionIdBehavior();

            //  distinguish between http:// and https://
            BasicHttpBinding binding = new BasicHttpBinding();

            if (PolarionUri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped).StartsWith(Strings.SECUREPROTOCOL))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport; // SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            }

            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            AddWebService(WebServiseFactory.Builder, Strings.BUILDERURL, behavior, binding, typeof(BuilderWebServiceClient));
            //added for testing
            AddWebService(WebServiseFactory.Planning, Strings.PLANNINGURL, behavior, binding, typeof(PlanningWebServiceClient));
            AddWebService(WebServiseFactory.Project, Strings.PROJECTURL, behavior, binding, typeof(ProjectWebServiceClient));
            AddWebService(WebServiseFactory.Security, Strings.SECURITYURL, behavior, binding, typeof(SecurityWebServiceClient));
            AddWebService(WebServiseFactory.Session, Strings.SESSIONURL, behavior, binding, typeof(SessionWebServiceClient));
            AddWebService(WebServiseFactory.TestManagement, Strings.TESTMANAGEMENTURL, behavior, binding, typeof(TestManagementWebServiceClient));
            AddWebService(WebServiseFactory.Tracker, Strings.TRACKERURL, behavior, binding, typeof(TrackerWebServiceClient));
        }
        /// <summary>
        /// Tests if the WebService is accessible.
        /// </summary>
        /// <returns></returns>
        public bool Test()
        {
            try
            {
                string ipAddress = WebServices[WebServiseFactory.Security].getProductLicense().getIpAdress();
                return true;
            }
            catch (Exception e)
            {
                //  FaultException is thrown if the web service is reachable, but need authorization
                return e.GetType() == typeof(FaultException);
            }
        }
        #endregion public Methods

        #region private methods
        private void AddWebService(WebServiseFactory service, string webServiceURL, SessionIdBehavior behavior, BasicHttpBinding binding, Type wsType)
        {
            //  get the specific web service Url
            Uri wsUri = new Uri(PolarionUri, webServiceURL);

            //  create a new web service object
            dynamic ws = Activator.CreateInstance(wsType, new Object[] { binding, new EndpointAddress(wsUri.ToString()) });

            //  adding a specfic client behavior programmatically
            ws.Endpoint.Behaviors.Add(behavior);
            WebServices.Add(service, ws);
        }
        #endregion private methods
    }
}
