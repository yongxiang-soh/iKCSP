using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Attributes;

namespace KCSG.Web.Controllers
{
    [MvcAuthorize]
    public class HomeController : BaseController
    {
        #region Properties

        /// <summary>
        ///     A service provides functions to handle identity.
        /// </summary>
        private readonly IIdentityService _identityDomain;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initiate controller with dependency injection.
        /// </summary>
        /// <param name="identityDomain"></param>
        public HomeController(
            IIdentityService identityService,
            IUnitOfWork unitOfWork) : base(identityService, unitOfWork)
        {
            _identityDomain = identityService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Renders index page of home.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Render communication C1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CommunicationC1()
        {
            return View();
        }

        public async Task<ActionResult> LockScreen()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion
    }
}