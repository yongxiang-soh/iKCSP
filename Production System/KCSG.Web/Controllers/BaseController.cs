using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Web.Controllers
{
    public class BaseController : Controller
    {
        #region Properties

        /// <summary>
        /// Service which is handle identity from request.
        /// </summary>
        private IIdentityService _identityService;

        private IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (_unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>()); }
            set { _unitOfWork = value; }
        }
        /// <summary>
        /// Service which is handle identity from request.
        /// </summary>
        public IIdentityService IdentityService
        {
            get
            {
                return _identityService ?? (_identityService = DependencyResolver.Current.GetService<IIdentityService>());
            }
            set { _identityService = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate controller without IoC.
        /// </summary>
        public BaseController()
        {
            
        }

        /// <summary>
        /// Initiate controller with IoC.
        /// </summary>
        /// <param name="identityService"></param>
        public BaseController(
            IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// How to use: this.RenderRazorViewToString("ViewName");
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public string RenderRazorViewToString(string viewName)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// How to use: this.RenderViewToString(this.ControllerContext, "ViewtName", null);
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderViewToString(string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            using (var sw = new StringWriter())
            {
                var result = PartialView(viewName);
                result.View = ViewEngines.Engines.FindPartialView(ControllerContext, viewName).View;
                if (model != null)
                {
                    result.ViewData = new ViewDataDictionary(model);
                }

                ViewContext vc = new ViewContext(ControllerContext, result.View, result.ViewData, result.TempData, sw);

                result.View.Render(vc, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        public string RenderViewToString(ControllerContext context, string viewName, object model, TempDataDictionary tempData)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        ///     Find validation errors and construct 'em as json object.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        public IDictionary<string, string[]> FindValidationErrors(ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Keep the screen for the request terminal as it reaches the screen first.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> LockScreen()
        {
            if (IdentityService == null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            
            // Find terminal number.
            var terminalNo = IdentityService.FindTerminalNo(HttpContext.User.Identity);

            // Find the accessing screens in identity.
            var accessingScreens = IdentityService.FindAccessingScreens(HttpContext.User.Identity);

            // No screen is specified.
            if (accessingScreens == null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            // Take the first one in the list.
            var accessingScreen = accessingScreens.FirstOrDefault();
            if (accessingScreen == null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            // Initiate terminal status.
            var terminalStatus = new TM17_TermStatus();
            terminalStatus.F17_TermNo = terminalNo;
            terminalStatus.F17_InUsePictureNo = accessingScreen;
            terminalStatus.F17_LastRequest = DateTime.Now;

            // Add or update terminal status.
            UnitOfWork.TerminalRepository.AddOrUpdate(terminalStatus);
            await UnitOfWork.CommitAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion
    }
}