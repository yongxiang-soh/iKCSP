using System;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Web.Areas.Common.ViewModels;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using AuthorizeAttribute = Microsoft.AspNet.SignalR.AuthorizeAttribute;

namespace KCSG.Web.Attributes
{
    public class SignalrCookieAuthenticateAttribute : AuthorizeAttribute
    {
        #region Constructors

        /// <summary>
        /// Initiate attribute with list of accessible terminals
        /// </summary>
        /// <param name="accessibleTerminals"></param>
        public SignalrCookieAuthenticateAttribute(string[] accessibleTerminals)
        {
            _accessibleTerminals = accessibleTerminals;

            // Find logging instance.
            Logger = LogManager.GetLogger(typeof(SignalrCookieAuthenticateAttribute));

            // Find the domain from the dependency first.
            _loginDomain = GlobalHost.DependencyResolver.Resolve<ILoginDomain>();
        }
        #endregion

        #region Properties

        /// <summary>
        ///     Domain which provides functions to access account repository.
        /// </summary>
        private ILoginDomain _loginDomain;

        /// <summary>
        /// Logging instance.
        /// </summary>
        private ILog _log;

        /// <summary>
        ///     List of terminals which can access to the current hub.
        ///     If this value is set to null, all terminal can access the hub.
        /// </summary>
        private readonly string[] _accessibleTerminals;

        /// <summary>
        ///     Dependency injection which is used for accessing Account function.
        /// </summary>
        public ILoginDomain LoginDomain
        {
            get { return _loginDomain; }
            set { _loginDomain = value; }
        }

        /// <summary>
        /// Logging instance.
        /// </summary>
        public ILog Logger
        {
            get { return _log; }
            set { _log = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Whether hub allows anonymous access or not.
        /// </summary>
        /// <param name="hubDescriptor"></param>
        /// <returns></returns>
        private bool IsAnonymousAllowed(HubDescriptor hubDescriptor)
        {
            return hubDescriptor.HubType.IsDefined(typeof(AllowAnonymousAttribute), true);
        }
        
        /// <summary>
        ///     This function is for parsing cookie, querying database and decide whether user can access the function or not.
        /// </summary>
        /// <param name="hubDescriptor"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
#if !UNAUTHORIZED_DEBUG
            var  result = IsAccessible(hubDescriptor, request);
            return true;
#elif UNAUTHORIZED_DEBUG
            return true;
#endif
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
#if !UNAUTHORIZED_DEBUG
            try
            {
                var hub = hubIncomingInvokerContext.Hub;
                var request = hub.Context.Request;
                var hubDescriptor = hubIncomingInvokerContext.MethodDescriptor.Hub;

                if (request.User == null)
                {
                    Logger.Error("No user identity has been attached to the request.");
                    return false;
                }

                if (request.User.Identity == null)
                {
                    Logger.Error("No user identity has been attached to the request.");
                    return false;
                }

                //return IsAccessible(hubDescriptor, request);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
#endif
            return true;
        }

        private bool IsAccessible(HubDescriptor hubDescriptor, IRequest request)
        {
            try
            {
                #region Connection validity check

                // Find request principle.
                var principle = request.User;

                // Request has been authenticated before.
                if (principle != null && principle.Identity != null && principle.Identity.IsAuthenticated)
                    return true;

                #endregion

                #region Authentication cookie analyze

                // Find authentication cookie from the request.
                var formAuthenticationCookie = request.Cookies[FormsAuthentication.FormsCookieName];

                // Invalid form authentication cookie.
                if (formAuthenticationCookie == null)
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("Invalid authentication cookie");
                    return false;
                }

                //Cookie value is invalid
                if (string.IsNullOrWhiteSpace(formAuthenticationCookie.Value))
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("Invalid authentication cookie value");
                    return false;
                }

                #endregion

                #region Form authentication ticket

                // Decrypt the authentication cookie value to authentication ticket instance.
                var formAuthenticationTicket = FormsAuthentication.Decrypt(formAuthenticationCookie.Value);

                // Ticket is invalid.
                if (formAuthenticationTicket == null)
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("Invalid authentication cookie ticket");
                    return false;
                }

                // User data is invalid.
                if (string.IsNullOrWhiteSpace(formAuthenticationTicket.UserData))
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("Invalid authentication cookie user data");
                    return false;
                }

                #endregion

                #region IP Address validation

                // Find the user data in the ticket.
                var loginViewModel = JsonConvert.DeserializeObject<LoginItem>(formAuthenticationTicket.UserData);

                // User data is invalid.
                if (loginViewModel == null)
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("Authentication ticket information is invalid.");
                    return false;
                }

                // Find IP Address of request.
                var requestIpAddress = _loginDomain.FindRequestIpAddress(request.GetHttpContext());

                // Cookie doesn't come from the same origin.
                if (string.IsNullOrEmpty(requestIpAddress) || !requestIpAddress.Equals(loginViewModel.IpAddress))
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error(string.Format("Cookie doesn't come from the same origin as the request (Source: {0} - Target: {1})", loginViewModel.IpAddress, loginViewModel.Password));
                    return false;
                }

                #endregion

                #region Passsword

                // No password is included in cookie.
                if (string.IsNullOrEmpty(loginViewModel.Password))
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error("No password is included in the cookie.");
                    return false;
                }

                // Find password setting.
                var passwordSetting = _loginDomain.FindPasswordSetting(loginViewModel.Password);
                if (passwordSetting == null)
                {
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error(string.Format("Password {0}", loginViewModel.Password));
                    return false;
                }

                #endregion

                #region Terminal 

                // Analyze client ip address.
                var ips = LoginDomain.AnalyzeIpAddress(requestIpAddress);

                // Find terminal by searching ip address.
                var terminal = _loginDomain.FindTerminalFromIpAddress(ips);

                // No terminal has been found in the request.
                if (terminal == null)
                {
                    // Unauthenticated request is allowed to access function.
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error(string.Format("No terminal has been found with IP : {0}", requestIpAddress));
                    return false;
                }

                // Accessible terminals are defined.
                
                // Terminal cannot access to the sensitive hub.
                if (_accessibleTerminals != null &&  !_accessibleTerminals.Any(x => x.Equals(terminal.F06_TerminalNo)))
                {
                    // Unauthenticated request is allowed to access function.
                    if (IsAnonymousAllowed(hubDescriptor))
                        return true;

                    Logger.Error(string.Format("No terminal has been found with IP : {0}", requestIpAddress));
                    return false;
                }

                #endregion

                var claimIdentity = new ClaimsIdentity(null, _loginDomain.AuthenticationClaimName);
                claimIdentity.AddClaim(new Claim(ClientIdentities.TerminalNo, terminal.F06_TerminalNo));
                claimIdentity.AddClaim(new Claim(ClientIdentities.IpAddress, requestIpAddress));

                var httpContext = request.GetHttpContext();
                httpContext.User = new ClaimsPrincipal(claimIdentity);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                return false;
            }
        }
        #endregion
    }
}