using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.ProductManagement.ViewModels.StorageOfExternalPreProduct;
using KCSG.Web.Attributes;
using log4net;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR091F")]
    public class StorageOfExternalPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IStorageOfExternalPreProductDomain _storageOfExternalPreProductDomain;
        private readonly IIdentityService _identityService;
        private readonly IConfigurationService _configurationService;

        private readonly ILog _log;

        #endregion

        #region Constructor

        public StorageOfExternalPreProductController(
            IStorageOfExternalPreProductDomain storageOfExternalPreProductDomain, IIdentityService identityService,
            IConfigurationService configurationService,
            ILog log)
        {
            _storageOfExternalPreProductDomain = storageOfExternalPreProductDomain;
            _identityService = identityService;
            _configurationService = configurationService;
            _log = log;
        }

        #endregion

        #region Methods

        //
        // GET: /ProductManagement/StorageOfExternalPreProduct/
        public ActionResult Index()
        {
            var model = new StorageOfExternalPreProductViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Storage(StorageOfExternalPreProductViewModel model)
        {
            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

            var result = _storageOfExternalPreProductDomain.StoringExternalPreProduct(model.LotNo.Trim(),
                model.PreProductCode.Trim(), model.Quantity, model.PalletNo.Trim(), terminalNo, deviceCode);
            if (!result.IsSuccess)
            {
                return Json(new {Success = false, Message = result.ErrorMessages});
            }

            return Json(new { Success = true, KndNo = result.Data });
        }

        [HttpPost]
        public ActionResult LotEnd(string lotNo, string preProductCode)
        {
            try
            {
                var result = _storageOfExternalPreProductDomain.CompletingKneadingCommand(lotNo, preProductCode);
                if (result)
                    return Json(new {Success = true, Message = ProductManagementResources.MSG49});
                return Json(new {Success = false, Message = ProductManagementResources.MSG48});
            }
            catch (Exception)
            {
                return Json(new {Success = false});
            }
        }

        [HttpPost]
        public ActionResult CheckKneadingClassAndStatus(string preProductCode, string lotNo, string palletNo)
        {
            var isChecked =
                _storageOfExternalPreProductDomain.CheckKneadingClassAndStatus(preProductCode, lotNo);
            if (!isChecked)
                return Json(new {Success = false, Message = "MSG44"});

            var isCheckedPalletNo = _storageOfExternalPreProductDomain.CheckPalletNo(palletNo);
            if (!isCheckedPalletNo)
                return Json(new { Success = false, Message = "MSG45" });

            return Json(new {Success = true});
        }

        [HttpPost]
        public ActionResult GetRemainingAmount(string palletNo)
        {
            var result = _storageOfExternalPreProductDomain.GetRemainingAmount(palletNo);
            return Json(result);
        }


        [HttpPost]
        public ActionResult CheckOutSidePreProductStockStatus(string palletNo)
        {
            var isChecked = _storageOfExternalPreProductDomain.CheckOutSidePreProductStockStatus(palletNo);
            if(!isChecked)
                return Json(new{Success=false,Message=ProductManagementResources.MSG45});
            return Json(new{Success=true});
        }
        [HttpPost]
        public ActionResult DeleteProductShelfStock(string palletNo)
        {
            _storageOfExternalPreProductDomain.DeteteProductShelfStock(palletNo);
            return Json(new {Success = true});
        }

        /// <summary>
        /// Respond message from C3.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RespondMessageC3(StorageOfExternalPreProductC3ViewModel items)
        {
            try
            {
                if (items == null)
                {
                    items = new StorageOfExternalPreProductC3ViewModel();
                    TryValidateModel(items);
                }
                if (string.IsNullOrEmpty(items.LotNo))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                // Find terminal no.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                var result = _storageOfExternalPreProductDomain.RespondingReplyFromC3(terminalNo, items.LotNo,
                    items.PreProductCode, items.Kndcmdno);

                return Json(result);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return Json(null);
            }
            
        }

        #endregion
    }
}