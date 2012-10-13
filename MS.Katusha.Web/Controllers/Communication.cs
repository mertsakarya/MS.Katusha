using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;
using SignalR.Hubs;

namespace MS.Katusha.Web.Controllers
{
    [HubName("Communication")]
    public class CommunicationHub : KatushaBaseHub
    {

        public void Send(string message)
        {
            // Call the addMessage method on all clients
            Clients.addMessage(KatushaProfile.Name + " : " + message);
        }
    }
}
