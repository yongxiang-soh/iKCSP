using System;
using System.Web.Configuration;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.CommunicationManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.Web.Areas.Communication.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.Communication.Controllers
{
    [MvcAuthorize("AWT001")]
    public class AWMaterialController : BaseCommucationController
    {
        private static ICommunication1Domain _communication1Domain;

        public AWMaterialController(ICommunication1Domain communication1Domain,
            IConfigurationService configurationService)
            : base(configurationService)
        {
            _communication1Domain = communication1Domain;
        }

        // GET: Communication/AWMaterial
        public ActionResult Index()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            var urlQueue = urlHelper.Action("SearchByQueue", "AWMaterial", new {Area = "Communication"});
            var urlHistory = urlHelper.Action("SearchByHistory", "AWMaterial", new {Area = "Communication"});
            var deviceCode = _configurationService.MaterialDeviceCode;
            var status = _communication1Domain.GetDeviceStatus(deviceCode);
            var model = new GeneralInformatrionViewModel
            {
                DetailInformation = new DetailInformationViewModel
                {
                    CommonQueue = GenerateGridQueue(urlQueue),
                    CommonHistory = GenerateGridHistory(urlHistory)
                },
                Status = status,
                Communcation = 1
            };
            return View(model);
        }

        public ActionResult SearchByQueue(DetailInformationViewModel model, GridSettings gridSettings)
        {
            var date = !string.IsNullOrEmpty(model.Date)
                ? ConvertHelper.ConvertToDateTimeFull(model.Date)
                : new DateTime();
            var result = _communication1Domain.GetQueue(date, model.Terminal, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchByHistory(DetailInformationViewModel model, GridSettings gridSettings)
        {
            var date = !string.IsNullOrEmpty(model.Date)
                ? ConvertHelper.ConvertToDateTimeFull(model.Date)
                : new DateTime();
            var result = _communication1Domain.GetHistory(date, model.Terminal, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Status(int status)
        {
            var deviceCode = _configurationService.MaterialDeviceCode;
            var result = _communication1Domain.SetDeviceStatus(status == 0, deviceCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(string commandNo, string cmdSeqNo)
        {
            var result = _communication1Domain.DeleteMaterialWarehouseCommand(commandNo, cmdSeqNo);
            if (result.IsSuccess)
                return Json(new {Success = true, Message = MessageResource.MSG10},
                    JsonRequestBehavior.AllowGet);
            return
                Json(
                    new
                    {
                        Success = false,
                        Message =
                        result.ErrorMessages[0].Equals("MSG4")
                            ? CommunicationResource.MSG4
                            : ""
                    }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteHistory(DetailInformationViewModel model)
        {
            var result = _communication1Domain.DeleteMaterialWarehouseHistories();
            if (result)
                return Json(new {Success = true, Message = MessageResource.MSG10},
                    JsonRequestBehavior.AllowGet);
            return Json(new {Success = false, Message = CommunicationResource.MSG4}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult End(string commandNo, string cmdSeqNo)
        {
            var result = _communication1Domain.EndMaterialWarehouseCommand(commandNo, cmdSeqNo);
            if (result)
                return Json(new {Success = true, Message = CommunicationResource.MSG13},
                    JsonRequestBehavior.AllowGet);
            return Json(new {Success = false, Message = CommunicationResource.MSG7}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Cancel(string commandNo, string cmdSeqNo)
        {
            var result = _communication1Domain.CancelMaterialWarehouseCommand(commandNo, cmdSeqNo);
            if (result)
                return Json(new {Success = true, Message = CommunicationResource.MSG12},
                    JsonRequestBehavior.AllowGet);
            return Json(new {Success = false, Message = CommunicationResource.MSG9}, JsonRequestBehavior.AllowGet);
        }
    }
}