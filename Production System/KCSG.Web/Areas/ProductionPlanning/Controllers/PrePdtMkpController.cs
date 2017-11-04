using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP022F")]
    public class PrePdtMkpController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public PrePdtMkpController(IPrePdtMkpDomain prePdtMkpDomain, IPreProductDomain preProductDomain,
            IMaterialDomain materialDomain)
        {
            _prePdtMkpDomain = prePdtMkpDomain;
            _preProductDomain = preProductDomain;
            _materialDomain = materialDomain;
        }

        #endregion

        public ActionResult Edit(string preProductCode, string materialCode, string preProductName)
        {
            var model = new PrePdtMkpViewModel {IsCreate = true};
            var preproduct = _preProductDomain.GetById(preProductCode);
            if (string.IsNullOrEmpty(materialCode.Trim()))
            {
                model.F02_PreProductCode = preProductCode;
                model.F03_PreProductName = preproduct != null ? preproduct.F03_PreProductName : preProductName;
                model.F02_Addtive = "0";
              //  model.F02_ThrawSeqNo = (_prePdtMkpDomain.CountByPreproductCode(preProductCode)+1).ToString();
            }
            else
            {
                var entity = _prePdtMkpDomain.GetById(preProductCode, materialCode);
                if (entity != null)
                {
                    model = Mapper.Map<PrePdtMkpViewModel>(entity);
                    model.F03_PreProductName = preproduct != null ? preproduct.F03_PreProductName : preProductName;
                    var material = _materialDomain.GetById(materialCode);
                    model.F01_MaterialName =material.F01_MaterialDsp;
                    model.F01_LiquidClass = material.Liquid;
                    model.F02_LoadPosition = model.F02_LoadPosition;
                    model.IsCreate = false;
                }
            }

            return PartialView("PreProduct/_PartialViewEditPrePdtMkp", model);
        }

        [HttpPost]
        public ActionResult Edit(PrePdtMkpViewModel model)
        {
            try
            {
                var item = Mapper.Map<PrePdtMkpItem>(model);
                var isSuccess = _prePdtMkpDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new {Success = false, Message = isSuccess.ErrorMessages}, JsonRequestBehavior.AllowGet);
                return Json(
                    new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(string preProductCode, string materialCode, string thrawSeqNo)
        {
            try
            {
                _prePdtMkpDomain.Delete(preProductCode.Trim(), materialCode.Trim(), thrawSeqNo.Trim());
                return Json(new {Success = true, Message = MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Cancel(string preProductCode)
        {
            try
            {
                _prePdtMkpDomain.Delete(preProductCode.Trim());
                return Json(new {Success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

       
        [HttpOptions]
        public JsonResult CheckExistMaterialCode(string f02_PreProductCode, string f02_materialcode, bool isCreate)
        {
            var result = !isCreate;
            if (isCreate)
                result = !_prePdtMkpDomain.CheckUnique(f02_PreProductCode, f02_materialcode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        [HttpOptions]
        public JsonResult CheckPriority(int f02_layinpriority, int f02_Addtive)
        {
            var result = true;
            if (f02_Addtive == (int) Constants.Additive.Additive)
                if (f02_layinpriority != 4)
                    result = false;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

       
        [HttpOptions]
        public JsonResult PotSeqNo(string f02_potseqno, string f02_PreProductCode, bool isCreate)
        {
            var result = !isCreate;
            if (isCreate)
                result = !_prePdtMkpDomain.PotSeqNo(f02_PreProductCode, f02_potseqno);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       
        [HttpOptions]
        public JsonResult MsrSeqNo(string F02_MsrSeqNo, string f02_potseqno)
        {
            var result = string.IsNullOrEmpty(f02_potseqno) ? !string.IsNullOrEmpty(F02_MsrSeqNo) : F02_MsrSeqNo != "1";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Domain Declaration

        private readonly IPrePdtMkpDomain _prePdtMkpDomain;
        private readonly IPreProductDomain _preProductDomain;
        private readonly IMaterialDomain _materialDomain;

        #endregion
        
        [HttpOptions]
        public JsonResult CheckAmount(double? f02_3flayinamount, double? f02_4flayinamount)
        {
            if (f02_3flayinamount >= 0.01 && f02_4flayinamount >=0.01 )
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}