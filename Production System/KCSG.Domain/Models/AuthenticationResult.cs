using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    public class AuthenticationResult
    {
        /// <summary>
        /// This information is returned by the server by searching IP for Terminal No.
        /// </summary>
        public string TerminalNo { get; set; }

        /// <summary>
        /// By checking password input from login screen, server knows the password level of account.
        /// </summary>
        public string PasswordLevel { get; set; } 

        /// <summary>
        /// Screen which user is able to access into.
        /// </summary>
        public IList<string> AccessibleScreens { get; set; }
    }
}
