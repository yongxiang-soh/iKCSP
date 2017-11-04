using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Web.Areas.MaterialManagement.ViewModels.RetrievalOfWarehousePallet;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM111F")]
    public class RetrievalOfWarehousePalletController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public RetrievalOfWarehousePalletController(IRetrievalOfWarehousePalletDomain retrievalOfWarehousePalletDomain,
            IIdentityService identityDomain, IInterFloorMovementOfMaterialDomain interFloorMovementOfMaterialDomain,
            IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain, IRetrieveSupplierPalletDomain retrieveSupplierPalletDomain)
        {
            _retrievalOfWarehousePalletDomain = retrievalOfWarehousePalletDomain;
            _identityDomain = identityDomain;
            _interFloorMovementOfMaterialDomain = interFloorMovementOfMaterialDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
            _retrieveSupplierPalletDomain = retrieveSupplierPalletDomain;
        }

        #endregion

        //
        // GET: /MaterialManagement/RetrievalOfWarehousePallet/
        public ActionResult Index()
        {
            var possibleQuantity = GetPossibleQuantiy();
            var model = new RetrievalOfWarehousePalletViewModel
            {
                PossibleRetrievalQuantity = possibleQuantity,
                RequestRetrievalQuantity =  1
                
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Retrieval(RetrievalOfWarehousePalletViewModel model)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            if (ModelState.IsValid)
            {
                // TODO: 	If there is no [tm05_conveyor] record whose [f05_terminalno] is current application terminal or [f05_strrtrsts] is “9” (Error), the system will show error message using template MSG 15

                var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
                if (!checkConveyorStatus)
                    return Json(new { Success = false, Message = MaterialResource.MSG15 });

                // Check whether device is valid or not.
                var isValidDevice = _retrieveSupplierPalletDomain.IsValidDevice();
                if (!isValidDevice)
                {                    
                    return Json(new { Success = false, Message = MaterialResource.MSG16 });
                }
                //var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
                //if (!checkConveyorStatus)
                //    return Json(new { Success = false, Message = MaterialResource.MSG15 });

                //var checkDeviceStatus = _interFloorMovementOfMaterialDomain.CheckedWarehouseStatus();
                //if (!checkDeviceStatus)
                //    return Json(new { Success = false, Message = MaterialResource.MSG15 }, JsonRequestBehavior.AllowGet);

                var item = Mapper.Map<RetrievalOfWarehousePalletItem>(model);
                var result = _retrievalOfWarehousePalletDomain.Retrieval(item, terminalNo);
                if (!result.IsSuccess)
                    return Json(new {Success = false});
            }
            return Json(new {Success = true});
        }
        [HttpPost]
        public ActionResult RetrieveWarehousePalletMessageC1Reply ()
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var proceededRecords = _retrievalOfWarehousePalletDomain.RetrieveWarehousePalletMessageC1Reply(terminalNo);
            return Json(proceededRecords);
        }

        private int GetPossibleQuantiy()
        {
            return _retrievalOfWarehousePalletDomain.GetPossibleQuantity();
        }

        #region Domain Declaration

        private readonly IRetrievalOfWarehousePalletDomain _retrievalOfWarehousePalletDomain;
        private readonly IIdentityService _identityDomain;
        private readonly IInterFloorMovementOfMaterialDomain _interFloorMovementOfMaterialDomain;
        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;
        private readonly IRetrieveSupplierPalletDomain _retrieveSupplierPalletDomain;

        #endregion
    }
}