using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.MaterialManagement.ViewModels.InterFloorMovementOfMaterialController;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM091F")]
    public class InterFloorMovementOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Controller constructor

        public InterFloorMovementOfMaterialController(
            IInterFloorMovementOfMaterialDomain interFloorMovementOfMaterialDomain, IIdentityService identityDomain)
        {
            _interFloorMovementOfMaterialDomain = interFloorMovementOfMaterialDomain;
            _identityDomain = identityDomain;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCRM091F;
            var model = new InterFloorMovementOfMaterialViewModel();
            model.TerminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            if (model.TerminalNo == Constants.TerminalNo.A017)
            {
                model.From = 3;
                model.To = 4;
            }
            else
            {
                model.From = 4;
                model.To = 3;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult Edit(int from, int to)
        {
            //Get current terminalNo
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var checkedConveyorexsits = _interFloorMovementOfMaterialDomain.CheckRecordTm05(terminalNo);
            if (!checkedConveyorexsits)
                return Json(new { Success = false, Message = MaterialResource.MSG34 });

            var checkedConveyorStatus = _interFloorMovementOfMaterialDomain.CheckConveyorStatus(terminalNo);
            if (!checkedConveyorStatus)
                return Json(new { Success = false, Message = MaterialResource.MSG15 });

            var checkedWarehouseStatus = _interFloorMovementOfMaterialDomain.CheckedWarehouseStatus();
            if (!checkedWarehouseStatus)
                return Json(new { Success = false, Message = MaterialResource.MSG16 });

            var result = _interFloorMovementOfMaterialDomain.CreateOrUpdate(from, to, terminalNo);
            return Json(new { Success = true, Message = result });
        }
        
        /// <summary>
        /// Complete inter-floor movement.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Complete()
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var records = await _interFloorMovementOfMaterialDomain.Complete(terminalNo);

            return Json(records);
        }

        #endregion

        #region Properties

        private readonly IInterFloorMovementOfMaterialDomain _interFloorMovementOfMaterialDomain;

        private readonly IIdentityService _identityDomain;

        #endregion
    }
}