using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models.PreProductCharging;
using KCSG.Web.Attributes;
using KCSG.Web.Controllers;
using Resources;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Controllers
{
    [MvcAuthorize("TCMD051F")]
    public class PreProductChargingController : BaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IConfigurationService _configurationService;
        private readonly IPreProductCharging _preProductCharging;

        public PreProductChargingController(IConfigurationService configurationService, IIdentityService identityService,
            IPreProductCharging preProductCharging)
        {
            _configurationService = configurationService;
            _preProductCharging = preProductCharging;
            _identityService = identityService;
        }

        // GET: TabletisingCommandSubSystem/PreProductCharging
        //public ActionResult Index(string kndcmdno="", string prepdtlotno="")
        //{
        //    if (!string.IsNullOrWhiteSpace(kndcmdno) && !string.IsNullOrWhiteSpace(prepdtlotno))
        //    {
        //        var preProductChargingItem = _preProductCharging.GetPreProductChargingItem(kndcmdno.Trim(),
        //            prepdtlotno.Trim(), _configurationService.CompanyName);
        //        var preProductChargingModel = Mapper.Map<PreProductChargingViewModel>(preProductChargingItem);
        //        return View(preProductChargingModel);
        //    }
        //    var model = new PreProductChargingViewModel();
        //    return View(model);
        //}

        public ActionResult Index(string containerCode = "")
        {
            //if (!string.IsNullOrWhiteSpace(containerCode))
            //{
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var preProductChargingItem = _preProductCharging.GetPreProductChargingItemByContainerCode(
                containerCode, terminalNo);
            var preProductChargingModel = Mapper.Map<PreProductChargingViewModel>(preProductChargingItem);
            return View(preProductChargingModel);
            //}
            //var model = new PreProductChargingViewModel();
            //return View(model);
        }

        public JsonResult GetPreProductChargingItemByContainerCode([Required]string containerCode)
        {
            if (!ModelState.IsValid)
                return Json(new { Success = false, Message = "Container does not exist!" },
                        JsonRequestBehavior.AllowGet);

            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var preProductChargingItem = _preProductCharging.GetPreProductChargingItemByContainerCode(containerCode,
                terminalNo);
            var preProductChargingModel = Mapper.Map<PreProductChargingViewModel>(preProductChargingItem);
            if (preProductChargingModel.IsError)
                if (preProductChargingModel.ErrorCode == "MSG38")
                    return Json(new {Success = false, Message = "Container is not retrieved yet!"},
                        JsonRequestBehavior.AllowGet);
                else if (preProductChargingModel.ErrorCode == "MSG33")
                    return Json(new {Success = false, Message = "Cannot get tablet line from toshiba.ini."},
                        JsonRequestBehavior.AllowGet);
                else
                    return Json(new {Success = false, Message = "Container does not exist!"},
                        JsonRequestBehavior.AllowGet);
            return Json(new {Success = true, Data = preProductChargingModel},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePreProductCharging(PreProductChargingViewModel model)
        {
            if (ModelState.IsValid)
                try
                {
                    var item = Mapper.Map<PreProductChargingItem>(model);
                    _preProductCharging.UpdatePreProductCharging(item);
                    return Json(
                        new {Success = true, Message = MessageResource.MSG9},
                        JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new {Success = false},
                        JsonRequestBehavior.AllowGet);
                }
            return Json(new {Success = false},
                JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult CheckContainerExist(string ContainerCode2)
        {
            var tx49_PrePdtShfStk = _preProductCharging.GetTX49_PrePdtShfStkByCode(ContainerCode2);
            if (tx49_PrePdtShfStk.Any())
                return Json(true, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult CheckTX41_TbtCmd(string KneadingCmdNo)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var tx41_TbtCmd = _preProductCharging.GetTX41_TbtCmdByCmd(KneadingCmdNo);
            if (tx41_TbtCmd != null)
                if ((tx41_TbtCmd.F41_Status == "2") && (tx41_TbtCmd.F41_TabletLine.Trim() == terminalNo.Trim()))
                    if (tx41_TbtCmd.F41_RtrEndCntAmt == tx41_TbtCmd.F41_ChgCntAmt)
                        return Json("Container is not retrieved yet!", JsonRequestBehavior.AllowGet);
                    else
                        return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json("Cannot get tablet line from toshiba.ini.", JsonRequestBehavior.AllowGet);
            return Json("Cannot get tablet line from toshiba.ini.", JsonRequestBehavior.AllowGet);
        }
    }
}