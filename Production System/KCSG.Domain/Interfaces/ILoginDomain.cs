using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces
{
    public interface ILoginDomain
    {
        #region Properties

        /// <summary>
        /// Name of authentication ticket which should be stored in the authentication cookie.
        /// </summary>
        string AuthenticationTicketName { get; set; }

        /// <summary>
        /// Name of claim identity which should be set to authentication ticket.
        /// </summary>
        string AuthenticationClaimName { get; set; }

        /// <summary>
        /// Duration of authentication cookie (life-time of authentication cookie - in seconds)
        /// </summary>
        int Duration { get; set; }
        
        #endregion

        #region Methods
        
        /// <summary>
        /// Find terminal information by using specific information (ip address)
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        TM06_Terminal FindTerminalFromIpAddress(List<int> ipAddress);

        /// <summary>
        /// Find terminal information by using specific information (ip address)
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        TM06_Terminal FindTerminalFromIpAddressAsync(List<int> ipAddress);


        /// <summary>
        /// Find password setting by using password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        TM16_Password FindPasswordSetting(string password);

        /// <summary>
        /// Find password setting by using password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<TM16_Password> FindPasswordSettingAsync(string password);
        
        /// <summary>
        ///     Split ip address into different parts.
        /// </summary>
        /// <returns></returns>
        List<int> AnalyzeIpAddress(string ipAddress);

        /// <summary>
        /// Find list of available screens by using terminal number and password level.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="passwordLevel"></param>
        /// <returns></returns>
        IList<string> FindAccessibleScreens(string terminalNo, string passwordLevel);

        /// <summary>
        /// Check whether account can access to a function or not.
        /// </summary>
        /// <param name="pictureNo"></param>
        /// <param name="terminalNo"></param>
        /// <param name="passwordLevel"></param>
        /// <returns></returns>
        bool IsAbleAccessArea(string pictureNo, string terminalNo,int passwordLevel);

        /// <summary>
        /// Find ip address inclcuded in the request sent to server.
        /// </summary>
        /// <param name="httpContextBase"></param>
        /// <returns></returns>
        string FindRequestIpAddress(HttpContextBase httpContextBase);

        #endregion
    }
}