using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Domains.PreProductManagement;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.MaterialManagement.ViewModels;
using KCSG.Web.Areas.MaterialManagement.ViewModels.ForcedRetrievalOfRejectedMaterial;
using KCSG.Web.Controllers;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM123F")]
    public class FontStorageOfSupplierPalletController : BaseController
    {
        private readonly IFontStorageOfSupplierPalletDomain _fontStorageOfSupplierPalletDomain;
        private readonly IIdentityService _identityService;
        public FontStorageOfSupplierPalletController(IFontStorageOfSupplierPalletDomain iStorageOfSupplierPalletDomain,IIdentityService identityService) 
        {
            _fontStorageOfSupplierPalletDomain = iStorageOfSupplierPalletDomain;
            _identityService = identityService;
        }
        // GET: MaterialManagement/FontStorageOfSupplierPallet
        public ActionResult Index()
        {
            return View();
        }
         [HttpPost]
        public ActionResult Storage(FontStorageOfSupplierPalletViewModel model)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
           var result =  _fontStorageOfSupplierPalletDomain.Storage(model.SupplierCode, model.StorageQuantity, terminalNo);
           return Json(result,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
         public ActionResult PostProcessMaterial(FontStorageOfSupplierPalletViewModel parameter)
        {
            var termialNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _fontStorageOfSupplierPalletDomain.CommutionC1(termialNo);
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}