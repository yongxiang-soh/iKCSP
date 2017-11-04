using System.Security.Principal;

namespace KCSG.Domain.Interfaces.Services
{
    public interface IIdentityService
    {
        #region Methods

        /// <summary>
        /// Find terminal number from identity.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        string FindTerminalNo(IIdentity identity);

        /// <summary>
        /// Check the identity whether it is in the target terminals or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="terminals"></param>
        /// <returns></returns>
        bool IsInTerminals(IIdentity identity, string[] terminals);

        /// <summary>
        /// Check whether terminal no is in terminals list or not.
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="terminals"></param>
        /// <returns></returns>
        bool IsInTerminals(string terminal, string[] terminals);

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        bool IsScreenAccessible(IIdentity identity, string screen);

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="availableScreens"></param>
        /// <param name="screens"></param>
        /// <returns></returns>
        bool IsScreenAccessible(string[] availableScreens, string[] screens);

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="screens"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        bool IsScreenAccessible(string[] screens, string screen);

        /// <summary>
        /// Check whether the screen can be accessed or not.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        string[] FindAccessibleScreens(IIdentity identity);

        /// <summary>
        /// Find screens which are being accessed by the request.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        string[] FindAccessingScreens(IIdentity identity);

        #endregion
    }
}