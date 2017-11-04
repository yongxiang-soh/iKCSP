using System;
using System.Net;
using System.Web.Mvc;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.PreProductManagement.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.Controllers
{
    [MvcAuthorize("TCIP011F")]
    public class StorageOfEmptyContainerController : KCSG.Web.Controllers.BaseController
    {
        #region Constructor

        /// <summary>
        ///     Initiate controller with IoC
        /// </summary>
        /// <param name="preProductManagementDomain"></param>
        /// <param name="identityService"></param>
        public StorageOfEmptyContainerController(IPreProductManagementDomain preProductManagementDomain,
            IIdentityService identityService, IConfigurationService configurationService, ICommonDomain commonDomain)
        {
            _preProductManagementDomain = preProductManagementDomain;
            _identityService = identityService;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles pre-product management.
        /// </summary>
        private readonly IPreProductManagementDomain _preProductManagementDomain;

        /// <summary>
        ///     Service which handles identity in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        private readonly IConfigurationService _configurationService;
        private readonly ICommonDomain _commonDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Display index page of storage of empty container.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     Get Container No when click button Assign
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStorageShelfNo()
        {
            var result = _preProductManagementDomain.GetStorageShelfNo();
            return Json(result);
        }

        /// <summary>
        ///     Insert and update records to database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertAndUpdate(StorageOfEmptyContainerViewModel model)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var deviceCode = _configurationService.PreProductDeviceCode;

            try
            {
                var checkContainerNo = _preProductManagementDomain.CheckedValidation(model.ContainerNo.ToString("D3"));
                if (!checkContainerNo)
                {
                    return Json(new
                    {
                        Success=false,
                        Message = PreProductManagementResources.MSG12
                    });
                }
                var checkConveyorCode = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
                if (!checkConveyorCode)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = PreProductManagementResources.MSG13
                    });
                }

                var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
                if (!checkedDeviceStatus)
                    return Json(new {Success = false, Message = PreProductManagementResources.MSG14});

                _preProductManagementDomain.InsertAndUpdate(model.ContainerNo,model.StorageShelfNo, model.ContainerType,
                    terminalNo);
                return Json(new {Success = true});
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    Success = false,
                    exception.Message
                });
            }
        }

        /// <summary>
        ///     Handle handle process after message is sent from C2.
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiveMessageFromC2(StorageEmptyContainerC2Reply parameters)
        {
            // Parameters haven't initialized.
            if (parameters == null)
            {
                parameters = new StorageEmptyContainerC2Reply();
                TryValidateModel(parameters);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var result = _preProductManagementDomain.ReceiveMessageFromC2(terminalNo, parameters.ContainerType,
                parameters.ContainerNo.ToString("D3"));
            return Json(result);
        }


        [HttpOptions]
        public ActionResult CheckedExistsTX50()
        {
            var isChecked = _preProductManagementDomain.CheckExistsTX50();
            if (!isChecked)
                return Json(new {Success = false, Message = PreProductManagementResources.MSG15});
            return Json(new {Success = true});
        }

        #endregion
    }
}