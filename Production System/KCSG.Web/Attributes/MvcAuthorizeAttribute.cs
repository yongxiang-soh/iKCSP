using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;
using KCSG.Web.Areas.Common.ViewModels;
using log4net;
using Newtonsoft.Json;


namespace KCSG.Web.Attributes
{
    public class MvcAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region Constructor

        /// <summary>
        ///     Initiate attribute without setting.
        /// </summary>
        public MvcAuthorizeAttribute()
        {
            _screens = null;

            // Domain which handles authentication process.
            //_loginDomain = DependencyResolver.Current.GetService<ILoginDomain>();
            Log = LogManager.GetLogger(typeof(MvcAuthorizeAttribute));
        }

        /// <summary>
        ///     Initialize attribute with default settings.
        /// </summary>
        public MvcAuthorizeAttribute(string screen) : this()
        {
            _screens = new[] {screen};
        }

        /// <summary>
        ///     Initiate attribute with list of accessible screens.
        /// </summary>
        /// <param name="screens"></param>
        public MvcAuthorizeAttribute(string[] screens) : this()
        {
            _screens = screens;
        }

        #endregion

        #region Properties
        
        /// <summary>
        ///     Terminal no which is allowed to access the action/controller.
        /// </summary>
        private readonly string[] _screens;
        
        /// <summary>
        ///     Instance which serves logging operation.
        /// </summary>
        public ILog Log { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for parsing cookie, querying database and decide whether user can access the function or not.
        /// </summary>
        /// <param name="authorizationContext"></param>
        public void OnAuthorization(AuthorizationContext authorizationContext)
        {
#if !UNAUTHORIZED_DEBUG
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                try
                {
                    var loginDomain = new LoginDomain(unitOfWork);

                    // Initiate authentication result.
                    var authenticationResult = new AuthenticationResult();

                    #region Authentication cookie & ticket validation

                    var formAuthenticationCookie =
                        authorizationContext.RequestContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (formAuthenticationCookie == null)
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        FormsAuthentication.SignOut();

                        authorizationContext.Result = new HttpUnauthorizedResult();
                        Log.Error("Authentication cookie is invalid.");

                        return;
                    }

                    //Cookie value is invalid
                    if (string.IsNullOrWhiteSpace(formAuthenticationCookie.Value))
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();
                        Log.Error("Authentication cookie value is invalid.");
                        return;
                    }

                    // Decrypt the authentication cookie value to authentication ticket instance.
                    var formAuthenticationTicket = FormsAuthentication.Decrypt(formAuthenticationCookie.Value);

                    // Ticket is invalid.
                    if (formAuthenticationTicket == null)
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error("Authentication ticket is not valid.");
                        return;
                    }

                    // User data is invalid.
                    if (string.IsNullOrWhiteSpace(formAuthenticationTicket.UserData))
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error("Authentication ticket's user data is invalid.");
                        return;
                    }

                    #endregion

                    #region IP Address validation

                    // Find the user data in the ticket.
                    var loginViewModel = JsonConvert.DeserializeObject<LoginItem>(formAuthenticationTicket.UserData);

                    // User data is invalid.
                    if (loginViewModel == null)
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error("Authentication ticket information is invalid.");
                        return;
                    }

                    // Find IP Address of request.
                    var requestIpAddress = loginDomain.FindRequestIpAddress(authorizationContext.HttpContext);

                    // Cookie doesn't come from the same origin.
                    if (string.IsNullOrEmpty(requestIpAddress) || !requestIpAddress.Equals(loginViewModel.IpAddress))
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error(string.Format("Cookie doesn't come from the same origin as the request (Source: {0} - Target: {1})", loginViewModel.IpAddress, loginViewModel.Password));
                        return;
                    }

                    #endregion

                    #region Passsword

                    // No password is included in cookie.
                    if (string.IsNullOrEmpty(loginViewModel.Password))
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error("No password is included in the cookie.");
                        return;
                    }

                    // Find password setting.
                    var passwordSetting = loginDomain.FindPasswordSetting(loginViewModel.Password);
                    if (passwordSetting == null)
                    {
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error(string.Format("Password {0}", loginViewModel.Password));
                        return;
                    }

                    // Find the password level.
                    authenticationResult.PasswordLevel = passwordSetting.F16_PswdLevel;

                    #endregion

                    #region Terminal 

                    // Analyze client ip address.
                    var ips = loginDomain.AnalyzeIpAddress(requestIpAddress);

                    // Find terminal by searching ip address.
                    var terminal = loginDomain.FindTerminalFromIpAddress(ips);

                    // No terminal has been found in the request.
                    if (terminal == null)
                    {
                        // Unauthenticated request is allowed to access function.
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Sign the user out to clear the cookie.
                        FormsAuthentication.SignOut();

                        // Treat the request as unauthorized.
                        authorizationContext.Result = new HttpUnauthorizedResult();

                        Log.Error(string.Format("No terminal has been found with IP : {0}", requestIpAddress));
                        return;
                    }

                    // Update authentication result.
                    authenticationResult.TerminalNo = terminal.F06_TerminalNo;

                    #endregion

                    #region Cookie authentication

                    // Find the current system time on the server.
                    var systemTime = DateTime.Now;

                    // Login is successful, save the information in the cookie for future use.
                    formAuthenticationTicket = new FormsAuthenticationTicket(1, loginDomain.AuthenticationTicketName, systemTime,
                        systemTime.AddMinutes(30), true, JsonConvert.SerializeObject(loginViewModel));

                    // Initialize cookie contain the authorization ticket.
                    var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                        FormsAuthentication.Encrypt(formAuthenticationTicket));
                    authorizationContext.HttpContext.Response.Cookies.Add(httpCookie);

                    // Set credential for the HttpContext.
                    var claimIdentity = new ClaimsIdentity(null, loginDomain.AuthenticationClaimName);
                    claimIdentity.AddClaim(new Claim(ClientIdentities.TerminalNo, authenticationResult.TerminalNo));
                    claimIdentity.AddClaim(new Claim(ClientIdentities.IpAddress, requestIpAddress));
                    claimIdentity.AddClaim(new Claim(ClientIdentities.PasswordLevel, authenticationResult.PasswordLevel));

                    #endregion

                    #region Accessible screens

                    // Find list of accessible screens by using terminal functions & functions management.
                    var availableScreens = loginDomain.FindAccessibleScreens(authenticationResult.TerminalNo,
                        authenticationResult.PasswordLevel);

                    // No screen has been found.
                    if (availableScreens == null || availableScreens.Count < 1)
                    {
                        // Unauthenticated request is allowed to access function.
                        if (IsAnonymousAllowed(authorizationContext))
                            return;

                        // Treat the request as forbidden.
                        authorizationContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                        Log.Error(string.Format("No available screen has been found for terminal {0}", authenticationResult.TerminalNo));
                        return;
                    }

                    // Update available screens list to the terminal.
                    authenticationResult.AccessibleScreens = availableScreens;

                    // Identity update.
                    claimIdentity.AddClaim(new Claim(ClientIdentities.AccessibleScreens, string.Join(",", authenticationResult.AccessibleScreens)));

                    if (_screens != null)
                        claimIdentity.AddClaim(new Claim(ClientIdentities.AccessingScreen, string.Join(",", _screens)));

                    var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                    authorizationContext.HttpContext.User = claimsPrincipal;

                    // At least one screen has been specified to the target controller/action.
                    if (_screens != null && _screens.Length > 0)
                    {
                        // Check whether terminal can access to screen or not.
                        var isScreenAccessible = availableScreens.Any(x => _screens.Any(y => x.Equals(y)));
                        if (!isScreenAccessible)
                        {
                            // Treat the request as forbidden.
                            authorizationContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                            Log.Error(string.Format("Terminal {0} cannot access to screens : {1}", authenticationResult.TerminalNo, string.Join(",", _screens)));
                        }
                    }

                    // Access of terminal to screen is locked.
                    if (IsAccessLocked(terminal.F06_TerminalNo))
                    {
                          var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);  
                          authorizationContext.Result = new RedirectResult(urlHelper.Action("Index", "Home", new { Area = "",@isLockScreen = true}));
                    }
                     

                    #endregion
                }
                catch (UnauthorizedAccessException)
                {
                    authorizationContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
                
#elif UNAUTHORIZED_DEBUG

#endif
        }

        /// <summary>
        ///     Whether action/controller allows anonymous access or not.
        /// </summary>
        /// <param name="authorizationContext"></param>
        /// <returns></returns>
        private bool IsAnonymousAllowed(AuthorizationContext authorizationContext)
        {
            return authorizationContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                   authorizationContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                       typeof(AllowAnonymousAttribute), true);
        }

        /// <summary>
        /// Check whether screen is locked by another terminal or not.
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        private bool IsAccessLocked(string terminalNo)
        {
            // No screen is specified, therefore every terminal can access to 'em at any time.
            if (_screens == null)
                return false;

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // More than one screen is specified.
                // If one of them is access by another terminal. Block the access of the current one.
                var terminalStatuses = unitOfWork.TermStatusRepository.GetAll();
                var conflictTerminalStatus =
                    terminalStatuses.FirstOrDefault(
                        x =>
                            !x.F17_TermNo.Equals(terminalNo) && _screens.Contains(x.F17_InUsePictureNo) &&
                            (x.F17_LastRequest != null));
                if (conflictTerminalStatus != null && conflictTerminalStatus.F17_LastRequest != null &&
                    DateTime.Now.Subtract(conflictTerminalStatus.F17_LastRequest.Value) < TimeSpan.FromSeconds(10)) 
                    return true;

                // Screen is not blocked by any terminals at the request time.
                var accesses = unitOfWork.AccessRepository.GetAll();
                accesses = accesses.Where(x => _screens.Contains(x.F18_LockedPicture));
                
                // Whether screen is locked by another terminal.
                return (from terminalStatus in terminalStatuses
                        from access in accesses
                        where terminalStatus.F17_InUsePictureNo.Equals(access.F18_LockPicture)
                              && !terminalStatus.F17_TermNo.Equals(terminalNo)
                        select terminalStatus).Any();
            }  
        }

        #endregion
    }
}