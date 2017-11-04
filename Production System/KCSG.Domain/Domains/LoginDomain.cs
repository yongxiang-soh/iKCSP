using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Domains
{
    public class LoginDomain : ILoginDomain
    {
        #region Properties

        /// <summary>
        /// Name of authentication ticket.
        /// </summary>
        private string _authenticationTicketName;

        /// <summary>
        /// Name of authentication ticket claim.
        /// </summary>
        private string _authenticationClaimName;

        /// <summary>
        /// How many secs a cookie can live.
        /// </summary>
        private int _duration;

        /// <summary>
        /// Name of authentication ticket.
        /// </summary>
        public string AuthenticationTicketName
        {
            get { return _authenticationTicketName ?? (_authenticationTicketName = "MvcAuthenticationTicketName"); }
            set { _authenticationTicketName = value; }
        }

        /// <summary>
        /// Name of authentication claim name.
        /// </summary>
        public string AuthenticationClaimName
        {
            get { return _authenticationClaimName ?? (_authenticationClaimName = "MvcAuthenticationClaimName"); }
            set { _authenticationClaimName = value; }
        }

        /// <summary>
        /// How many seconds a cookie can live.
        /// </summary>
        public int Duration
        {
            get
            {
                if (_duration < 1)
                    _duration = 3600;

                return _duration;
            }

            set { _duration = value; }
        }

        /// <summary>
        /// Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initiate domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public LoginDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Find terminal by using ip address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public TM06_Terminal FindTerminalFromIpAddress(List<int> ipAddress)
        {
            // Find the terminals in the database.
            var terminals = _unitOfWork.TerminalRepository.GetAll();

            var ip1 = ipAddress[0];
            var ip2 = ipAddress[1];
            var ip3 = ipAddress[2];
            var ip4 = ipAddress[3];

            // Find the terminal by using ip.
            terminals = terminals.Where(x => x.F06_IPAddress1 == ip1);
            terminals = terminals.Where(x => x.F06_IPAddress2 == ip2);
            terminals = terminals.Where(x => x.F06_IPAddress3 == ip3);
            terminals = terminals.Where(x => x.F06_IPAddress4 == ip4);

            // Find the first match terminal in the database.
            var terminal = terminals.FirstOrDefault();

            return terminal;

        }

        /// <summary>
        /// Find terminal information from database by using ip address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public TM06_Terminal FindTerminalFromIpAddressAsync(List<int> ipAddress)
        {
            // Find the terminals in the database.
            var terminals = _unitOfWork.TerminalRepository.GetAll();

            var ip1 = ipAddress[0];
            var ip2 = ipAddress[1];
            var ip3 = ipAddress[2];
            var ip4 = ipAddress[3];

            // Find the terminal by using ip.
            terminals = terminals.Where(x => x.F06_IPAddress1 == ip1);
            terminals = terminals.Where(x => x.F06_IPAddress2 == ip2);
            terminals = terminals.Where(x => x.F06_IPAddress3 == ip3);
            terminals = terminals.Where(x => x.F06_IPAddress4 == ip4);

            // Find the first match terminal in the database.
            var terminal =  terminals.FirstOrDefault();

            return terminal;
        }

        /// <summary>
        /// Find password setting by using password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public TM16_Password FindPasswordSetting(string password)
        {
            var passwordSetting = _unitOfWork.PasswordRepository.GetAll();
            passwordSetting = passwordSetting.Where(x => x.F16_Password.Trim().Equals(password));

            return passwordSetting.FirstOrDefault();
        }

        /// <summary>
        /// Find password setting by using password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TM16_Password> FindPasswordSettingAsync(string password)
        {
            var passwordSetting = _unitOfWork.PasswordRepository.GetAll();
            passwordSetting = passwordSetting.Where(x => x.F16_Password.Trim().Equals(password));

            return await passwordSetting.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Check whether an account can access a function or not.
        /// </summary>
        /// <returns></returns>
        public bool IsAbleAccessArea(string pictureNo, string terminalNo, int passwordLevel)
        {
            // Find all functions.
            var functions = _unitOfWork.FunctionRepository.GetAll();

            // Convert password level to character.
            var szPasswordLevel = passwordLevel.ToString();

            functions =
                functions.Where(
                    x =>
                        x.F13_PictureNo.Equals(pictureNo) &&
                        ("0".Equals(x.F13_PswdLevel) || x.F13_PswdLevel.Equals(szPasswordLevel)));

            if (!functions.Any())
                return false;

            // Find terminal and picture no.
            var terminalPictures = _unitOfWork.TrmPicMgnRepository.GetAll();
            terminalPictures =
                terminalPictures.Where(x => x.F12_TerminalNo.Equals(terminalNo) && x.F12_PictureNo.Equals(pictureNo));

            return terminalPictures.Any();
        }

        /// <summary>
        ///     Split ip address into different parts.
        /// </summary>
        /// <returns></returns>
        public List<int> AnalyzeIpAddress(string ipAddress)
        {
            var ips = ipAddress.Split('.');
            if (ips.Length != 4)
                throw new Exception(string.Format("{0} is not a valid IP Address", ipAddress));
            
            var iIps = ips.Select(x => Convert.ToInt32(x)).ToList();
            return iIps;
        }

        /// <summary>
        /// Find list of available screens by using terminal number and password level.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="passwordLevel"></param>
        /// <returns></returns>
        public IList<string> FindAccessibleScreens(string terminalNo, string passwordLevel)
        {
            try
            {
                // Find terminal functions management.
                var terminalFunctions = _unitOfWork.TrmPicMgnRepository.GetAll();
                terminalFunctions = terminalFunctions.Where(x => x.F12_TerminalNo.Equals(terminalNo));

                // Find functions.
                var functions = _unitOfWork.FunctionRepository.GetAll();
                functions = functions.Where(x => x.F13_PswdLevel.Equals("0") || x.F13_PswdLevel.Equals(passwordLevel));

                var result = from terminalFunction in terminalFunctions
                    from function in functions
                    where terminalFunction.F12_PictureNo.Equals(function.F13_PictureNo)
                    select terminalFunction.F12_PictureNo;

                return result.Select(x => x.Trim()).ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Find ip address from the request sent to server.
        /// </summary>
        /// <param name="httpContextBase"></param>
        /// <returns></returns>
        public string FindRequestIpAddress(HttpContextBase httpContextBase)
        {
            // Find ip address from HTTP_X_FORWARDED_FOR header first.
            var ipAddress = httpContextBase.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                // Parse list of addresses.
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    // Obtain the first address of list.
                    ipAddress = addresses[0];

                    // Is loopback ip address ?
                    if (IsIpLoopback(ipAddress))
                        ipAddress = "127.0.0.1";

                    return addresses[0];
                }
            }

            // Find ip address from REMOTE_ADDR
            ipAddress = httpContextBase.Request.ServerVariables["REMOTE_ADDR"];
            if (IsIpLoopback(ipAddress))
                ipAddress = "127.0.0.1";

            return ipAddress;
        }

        /// <summary>
        /// Check whether ip address comes from the machine on which web is deployed.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private bool IsIpLoopback(string ipAddress)
        {
            // Find IPv4 loopback.
            var ipLoopback = IPAddress.Loopback.ToString();

            // Ip address is the IPv4 loopback.
            if (ipLoopback.Equals(ipAddress))
                return true;

            // Find IPv6 loopback.
            ipLoopback = IPAddress.IPv6Loopback.ToString();

            // IP Address is the IPv6 loopback.
            if (ipLoopback.Equals(ipAddress))
                return true;

            return false;
        }

        #endregion
    }
}