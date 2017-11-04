using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace KCSG.Core.Models
{
    public class SignalrMessageViewModel
    {
        /// <summary>
        /// List of terminals.
        /// </summary>
        public string [] Terminals { get; set; }

        /// <summary>
        /// Name of screen.
        /// </summary>
        [Required]
        public string ScreenName { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [Required]
        public string Message { get; set; }
    }
}