using System.Net;
using System.Web.Mvc;
using KCSG.Core.Models;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Web.Areas.Communication.Controllers
{
    public class MessageController : Controller
    {
        #region Properties

        /// <summary>
        /// Notification service.
        /// </summary>
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate controller with injectors.
        /// </summary>
        /// <param name="notificationService"></param>
        public MessageController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send message to notification service.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Send(SignalrMessageViewModel message)
        {
            if (message == null)
            {
                message = new SignalrMessageViewModel();
                TryValidateModel(message);
            }

            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _notificationService.SendNoteInformation(message.Terminals, message.ScreenName, message.Message);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion
    }
}