using System.Web.Mvc;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR;

namespace KCSG.Web.Hubs
{
    [SignalrCookieAuthenticate(null)]
    [AllowAnonymous]
    public class NotificationHub : Hub
    {
        
    }
}