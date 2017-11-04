using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KCSG.Domain.Interfaces;
using KCSG.Web.Areas.Common.ViewModels;
using KCSG.Web.Attributes;
using KCSG.Web.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace KCSG.Web.Controllers
{
    /// <summary>
    /// Controller which relates to Account function.
    /// Use MvcAuthorizeAttribute to parse cookie from client-side.
    /// </summary>
    [MvcAuthorize]
    public class AccountController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Domain provides functions to access account repository for account authenticating.
        /// </summary>
        private readonly ILoginDomain _loginDomain;

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate controller with dependency injections.
        /// </summary>
        /// <param name="loginDomain"></param>
        public AccountController(ILoginDomain loginDomain)
        {
            _loginDomain = loginDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Render login page to client.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
#if !UNAUTHORIZED_DEBUG
            // Login function can't be accessed by authenticated account.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var loginViewModel = new LoginItem();
            loginViewModel.IpAddress = _loginDomain.FindRequestIpAddress(HttpContext);
            return View(loginViewModel);
#else
            return RedirectToAction("Index", "Home");
#endif
        }

        /// <summary>
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginItem loginViewModel)
        {
#if !UNAUTHORIZED_DEBUG
            // Login function can't be accessed by authenticated account.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // Invalid submitted parameters.
            if (loginViewModel == null)
            {
                loginViewModel = new LoginItem();
                TryValidateModel(loginViewModel);
            }

            try
            {
                #region Information check

                // Invalid parameters.
                if (!ModelState.IsValid)
                    return View(loginViewModel);

                // Find the client ip address.
                loginViewModel.IpAddress = _loginDomain.FindRequestIpAddress(HttpContext);
                
                // Analyze client ip address.
                var ips = _loginDomain.AnalyzeIpAddress(loginViewModel.IpAddress);

                #endregion

                #region Terminal

                // Find terminal from ip address.
                var terminal =  _loginDomain.FindTerminalFromIpAddressAsync(ips);

                if (terminal == null)
                {
                    ModelState.AddModelError("", "No terminal has been found for this IP Address");
                    return View(loginViewModel); 
                }

                #endregion

                #region Password

                // Find password setting from database.
                var passworSetting = await _loginDomain.FindPasswordSettingAsync(loginViewModel.Password);

                if (passworSetting == null)
                {
                    ModelState.AddModelError("", "Password is incorrect.");
                    return View(loginViewModel);
                }

                #endregion

                #region Authentication processing

                // Find the current system time on the server.
                var systemTime = DateTime.Now;

                // Login is successful, save the information in the cookie for future use.
                var formAuthenticationTicket = new FormsAuthenticationTicket(1, _loginDomain.AuthenticationTicketName, systemTime, systemTime.AddMinutes(30), true, JsonConvert.SerializeObject(loginViewModel));

                // Initialize cookie contain the authorization ticket.
                var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(formAuthenticationTicket));
                Response.Cookies.Add(httpCookie);

                #endregion

                return RedirectToAction("Index", "Home");

            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                // Tell the client his/her credential is not valid.
                ModelState.AddModelError("", "Login information is not correct.");
                return View(loginViewModel);
            }
#elif UNAUTHORIZED_DEBUG
            return RedirectToAction("Index", "Home");
#endif
        }

        /// <summary>
        /// Sign user out of system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}