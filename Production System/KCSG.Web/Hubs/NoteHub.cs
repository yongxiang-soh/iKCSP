using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace KCSG.Web.Hubs
{
    [SignalrCookieAuthenticate(null)]
    [HubName("NoteNotification")]
    public class NoteHub : Hub
    {
        
    }
}