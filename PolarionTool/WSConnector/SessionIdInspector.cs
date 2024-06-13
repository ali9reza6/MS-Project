using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace net.seabay.polarion
{
    public class SessionIdInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        private long SessionID { get; set; }

        #region IDispatchMessageInspector Members
        object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
        {
        }
        #endregion

        #region IClientMessageInspector Members
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
            int index= reply.Headers.FindHeader("sessionID", "http://ws.polarion.com/session");
            if (index != -1)
            {
                XmlDictionaryReader reader = reply.Headers.GetReaderAtHeader(0);
                string sessionId = reader.ReadInnerXml();
                SessionID = Convert.ToInt64(sessionId);
            }
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (SessionID != 0)
            {
                request.Headers.Add(MessageHeader.CreateHeader("sessionID", "http://ws.polarion.com/session", SessionID));
            }
            return null;
        }
        #endregion
    }
}
