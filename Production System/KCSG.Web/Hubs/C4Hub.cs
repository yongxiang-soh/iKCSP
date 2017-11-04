using System.Web.Mvc;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace KCSG.Web.Hubs
{
    [SignalrCookieAuthenticate(null)]
    [HubName("Communication-C4")]
    public class C4Hub : Hub
    {
        
    }
}