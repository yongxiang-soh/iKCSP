using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.Web.Areas.ProductManagement.ViewModels;
using KCSG.Web.Areas.ProductManagement.ViewModels.ReStorageOfProduct;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR071F")]
    public class RestorageOfProductController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IRestorageOfProductDomain _restorageOfProductDomain;
        private readonly IIdentityService _identityService;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructor

        public RestorageOfProductController(IRestorageOfProductDomain restorageOfProductDomain, IIdentityService identityService, ICommonDomain commonDomain,IConfigurationService configurationService)
        {
            _identityService = identityService;
            _restorageOfProductDomain = restorageOfProductDomain;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }
        #endregion
        //
        // GET: /ProductManagement/RestorageOfProduct/
        public ActionResult Index(string palletNo,bool isChecked=false)
        {
            var model = new ReStorageOfProductViewModel();
            model.PalletNo = palletNo;
            model.isChecked = isChecked;
            return View(model);
        }

        [HttpPost]
        public ActionResult DeAssignProduct(string palletNo)
        {
            try
            {
                _restorageOfProductDomain.DeAssignProduct(palletNo.Trim());
                return Json(new { Success = true, Message = MessageResource.MSG10 });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public ActionResult GetData(string palletNo)
        {
            var details = _restorageOfProductDomain.GetListRestorageOfProduct(palletNo.Trim());
            return Json(new
            {
                results = details
            });
        }

        [HttpPost]
        public ActionResult CheckConveyorAndDeviceStatus()
        {
            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var isChecked = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if(!isChecked)
                return Json(new{Success=false,Message=ProductManagementResources.MSG8});

            var checkDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkDeviceStatus)
                return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
            return Json(new { Success = true });
        }


        [HttpPost]
        public ActionResult RestoreProduct(ReStorageOfProductViewModel model)
        {
            try
            {
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                var item = Mapper.Map<RestoreProductItem>(model);
                var result=_restorageOfProductDomain.StoreProduct(item, terminalNo);
                if (result)
                {
                    return Json(new { Success = true,Message=ProductManagementResources.MSG57 }); 
                }
                return Json(new { Success = false, Message = ProductManagementResources.MSG19 });
                
            }
            catch (Exception)
            {
                return Json(new {Success = false, Message = ProductManagementResources.MSG7});
            }
        }

        /// <summary>
        /// Reply messages from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reply(RestoreProductItem item)
        {
            // Find terminal.
            var terminal = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _restorageOfProductDomain.RespondingReplyFromC3(terminal, item);
            return Json(items);
        }
      
    }
}