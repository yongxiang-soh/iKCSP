using System;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM101F")]
    public class StorageOfWarehousePalletController : KCSG.Web.Controllers.BaseController
    {
        #region Controller constructor

        public StorageOfWarehousePalletController(IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain,
            IIdentityService identityDomain, IInterFloorMovementOfMaterialDomain interFloorMovementOfMaterialDomain)
        {
            _identityDomain = identityDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
            _interFloorMovementOfMaterialDomain = interFloorMovementOfMaterialDomain;
        }

        #endregion
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Edit()
        {

            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var checkedStatusTm05Record = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
            if (!checkedStatusTm05Record)
                return Json(new {Success = false, Message = MaterialResource.MSG15});

            var checkedDeviceStatus = _interFloorMovementOfMaterialDomain.CheckedWarehouseStatus();
            if (!checkedDeviceStatus)
                return Json(new {Success = false, Message = MaterialResource.MSG16});

            var checkStatusForTX31 = _storageOfWarehousePalletDomain.CheckStatusForTX31();
            if (!checkStatusForTX31)
                return Json(new {Success = false, Message = MaterialResource.MSG37});

            var result = _storageOfWarehousePalletDomain.CreateOrUpdate(terminalNo);


            //return Json(new {Success = true, Message = result.ErrorMessages[0]}, JsonRequestBehavior.AllowGet);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessFirstCommunicationData()
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var result  = _storageOfWarehousePalletDomain.PostRetrieveMaterial(terminalNo);
            return Json( result, JsonRequestBehavior.AllowGet);
        }

#region Domain Declare

        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;
        private readonly IIdentityService _identityDomain;
        private readonly IInterFloorMovementOfMaterialDomain _interFloorMovementOfMaterialDomain;

#endregion
    }
}