using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace KCSG.Web.Areas.Common.ViewModels
{
    public class LoginItem
    {
        /// <summary>
        /// Ip address of client. (Only for displaying)
        /// </summary>
        [BindNever]
        [DisplayName("IP Address")]
        public string IpAddress { get; set; }

        /// <summary>
        ///     Password which is used for logging into system.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Password { get; set; }
    }
}