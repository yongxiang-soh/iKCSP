using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Web.Services
{
    public class IdentityService : IIdentityService
    {
        /// <summary>
        ///     Find terminal number from identity.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public string FindTerminalNo(IIdentity identity)
        {
#if UNAUTHORIZED_DEBUG
            
            // While the app is in authorized debug mode, terminal should always be TM10
            return "TM10";
#else
            var claimIdentity = (ClaimsIdentity)identity;

            // Invalid identity.
            if (claimIdentity == null)
                return null;

            var terminalIdentity = claimIdentity.FindFirst(ClientIdentities.TerminalNo);
            if (terminalIdentity == null)
                return null;

            return terminalIdentity.Value;
#endif
        }

        /// <summary>
        /// Check the identity whether it is in the target terminals or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="terminals"></param>
        /// <returns></returns>
        public bool IsInTerminals(IIdentity identity, string[] terminals)
        {
#if !UNAUTHORIZED_DEBUG
            if (terminals == null)
                return true;

            // Find the terminal.
            var terminal = FindTerminalNo(identity);
            if (terminal == null)
                return false;

            return terminals.Any(x => x.Equals(terminal));
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }

        /// <summary>
        /// Check the identity whether it is in the target terminals or not.
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="terminals"></param>
        /// <returns></returns>
        public bool IsInTerminals(string terminal, string[] terminals)
        {
#if !UNAUTHORIZED_DEBUG
            if (terminals == null || string.IsNullOrWhiteSpace(terminal))
                return false;
            
            return terminals.Any(x => x.Equals(terminal));
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        public bool IsScreenAccessible(IIdentity identity, string screen)
        {
#if !UNAUTHORIZED_DEBUG
            if (identity == null)
                return false;

            var claimIdentity = (ClaimsIdentity)identity;
            
            // Find list of accessible screens.
            var claimAvailableScreens = claimIdentity.FindFirst(ClientIdentities.AccessibleScreens);

            if (claimAvailableScreens == null || string.IsNullOrEmpty(claimAvailableScreens.Value))
                return false;

            // Find list of available screens.
            var availableScreens = claimAvailableScreens.Value.Split(',');

            // No available screen has been found.
            if (availableScreens.Length < 1)
                return false;

            // No available screen has been detected.
            return availableScreens.Any(x => x.Trim().Equals(screen.Trim()));
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="screens"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        public bool IsScreenAccessible(string [] screens, string screen)
        {
#if !UNAUTHORIZED_DEBUG
            if (screens == null)
                return false;
            
            // No available screen has been found.
            if (screens.Length < 1)
                return false;

            // No available screen has been detected.
            return screens.Any(x => x.Trim().Equals(screen.Trim()));
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="availableScreens"></param>
        /// <param name="screens"></param>
        /// <returns></returns>
        public bool IsScreenAccessible(string[] availableScreens, string [] screens)
        {
#if !UNAUTHORIZED_DEBUG
            if (availableScreens == null || screens == null)
                return false;

            // No available screen has been found.
            if (screens.Length < 1 || screens.Length < 1)
                return false;

            // No available screen has been detected.
            return availableScreens.Any(x => screens.Any(y => x.Trim().Equals(y.Trim())));
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }
        
        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public string[] FindAccessibleScreens(IIdentity identity)
        {
#if !UNAUTHORIZED_DEBUG
            if (identity == null)
                return null;

            var claimIdentity = (ClaimsIdentity)identity;

            // Find list of accessible screens.
            var claimAvailableScreens = claimIdentity.FindFirst(ClientIdentities.AccessibleScreens);

            if (claimAvailableScreens == null || string.IsNullOrEmpty(claimAvailableScreens.Value))
                return null;

            // Find list of available screens.
            var availableScreens = claimAvailableScreens.Value.Split(',');

            return availableScreens;
#elif UNAUTHORIZED_DEBUG
            return null;
#endif
        }

        /// <summary>
        /// Find screens which are being accessed by request.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public string[] FindAccessingScreens(IIdentity identity)
        {
#if !UNAUTHORIZED_DEBUG
            if (identity == null)
                return null;

            var claimIdentity = (ClaimsIdentity)identity;

            // Find list of accessible screens.
            var claimAvailableScreens = claimIdentity.FindFirst(ClientIdentities.AccessingScreen);

            if (claimAvailableScreens == null || string.IsNullOrEmpty(claimAvailableScreens.Value))
                return null;

            // Find list of available screens.
            var availableScreens = claimAvailableScreens.Value.Split(',');

            return availableScreens;
#elif UNAUTHORIZED_DEBUG
            return null;
#endif
        }
    }
}