using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel.Description;

namespace net.seabay.polarion
{
    public class SessionIdBehavior : IEndpointBehavior
    {
        private SessionIdInspector Inspector { get; set; }

        #region Constructor
        public SessionIdBehavior()
        {
            Inspector = new SessionIdInspector();
        }
        #endregion Constructor

        #region public Members
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(Inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(Inspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion public Members
    }
}
