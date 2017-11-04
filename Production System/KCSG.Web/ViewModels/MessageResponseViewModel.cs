using System.Net;
using Newtonsoft.Json;

namespace KCSG.Web.ViewModels
{
    public class MessageResponseViewModel
    {
        /// <summary>
        /// Whether message is about success or not.
        /// </summary>
        public bool Success
        {
            get { return HttpStatusCode == HttpStatusCode.OK; }
        }

        /// <summary>
        /// Message information.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response status code.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        public MessageResponseViewModel()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }
    }
}