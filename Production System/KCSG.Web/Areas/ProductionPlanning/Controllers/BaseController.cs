using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Domains;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    public class BaseController : KCSG.Web.Controllers.BaseController
    {
        public ISupplierDomain _SupplierDomain ;
        public IMaterialDomain _materialDomain;

        public BaseController(ISupplierDomain iSupplierDomain,IMaterialDomain materialDomain)
        {
            _SupplierDomain = iSupplierDomain;
            _materialDomain = materialDomain;
           
        }


        public JsonResult SupplierCode(string searchText)
        {
            var result = _SupplierDomain.GetSuppliers(searchText);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string f01_MaterialCode, bool isCreate)
        {
            if (!isCreate) { return Json(true, JsonRequestBehavior.AllowGet); }
            var result = !_materialDomain.CheckUnique(f01_MaterialCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetMaterialName(string materialCode)
        {
            var material = _materialDomain.GetById(materialCode);
            if (material != null)
            {
                return Json(new { MaterialName = material.F01_MaterialDsp, MaterialLiquid = material.Liquid }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
        }

       
    }
}