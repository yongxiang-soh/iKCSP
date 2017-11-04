using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Enumerations;
using KCSG.Domain.Domains;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.Web.Areas.PreProductManagement.ViewModels;
using KCSG.Web.Attributes;
using log4net;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.Controllers
{
    [MvcAuthorize("TCIP021F")]
    public class StorageOfPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Domain which handles Pre-Product  operations.
        /// </summary>
        private readonly IPreProductManagementDomain _preProductManagementDomain;

        /// <summary>
        // Domain provides functions to analyze claim identity.
        /// </summary>
        private readonly IIdentityService _identityDomain;

        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;

        private readonly IExportReportDomain _exportReportDomain;

        private readonly ILabelPrintService _labelPrintService;

        private readonly ILog _log;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate controller with dependency injections.
        /// </summary>
        /// <param name="preProductManagementDomain"></param>
        /// <param name="identityDomain"></param>
        /// <param name="commonDomain"></param>
        /// <param name="configurationService"></param>
        /// <param name="labelPrintService"></param>
        /// <param name="exportReportDomain"></param>
        /// <param name="log"></param>
        public StorageOfPreProductController(
            IPreProductManagementDomain preProductManagementDomain,
            IIdentityService identityDomain, 
            ICommonDomain commonDomain, 
            IConfigurationService configurationService,
            ILabelPrintService labelPrintService,
            IExportReportDomain exportReportDomain,
            ILog log)
        {
            _identityDomain = identityDomain;
            _preProductManagementDomain = preProductManagementDomain;
            _identityDomain = identityDomain;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
            _labelPrintService = labelPrintService;
            _exportReportDomain = exportReportDomain;
            _log = log;
        }

        #endregion

        /// <summary>
        /// This function is for rendering index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This function is for finding empty containers and return the information to client-side.
        /// Please refer BR9 for more information.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindEmptyContainers(FindEmptyContainerViewModel condition)
        {
            try
            {
                var emptyContainers = await _preProductManagementDomain.FindEmptyContainers(condition.KneadingLine);
                if (!emptyContainers.Any())
                {
                    return Json(new {Success = false, Message = PreProductManagementResources.MSG32});
                }
                return Json(new
                {
                    container = emptyContainers.FirstOrDefault()
                });
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult Retrieval(string containerType, Constants.KndLine kneadingLine, string colorClass)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var deviceCode = _configurationService.PreProductDeviceCode;
            var isCheckedConveyor = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!isCheckedConveyor)
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG13
                });


            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
                return Json(new {Success = false, Message = PreProductManagementResources.MSG14});


            var result = _preProductManagementDomain.Retrieval(containerType, terminalNo, kneadingLine, colorClass);
            if (result == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG21
                });
            }

            return Json(new
            {
                Success = true,
                result
            });
        }

        /// <summary>
        /// This function is for searching for conveyor by using kneading line information.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindConveyor(FindConveyorViewModel condition)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
                var colourClass = condition.Colour.ToString("D");

                // Find the list of conveyor by using specific conditions.
                var conveyors = await _preProductManagementDomain.FindConveyors(condition.KneadingLine, terminalNo,
                    colourClass);

                // Find the first conveyor in the list.
                var conveyor = conveyors.FirstOrDefault();
                return Json(new
                {
                    conveyor
                });
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Find pre-product warehouse command by using terminal number.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FindPreProductWarehouseCommand()
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

                // Find the list of pre-product warehouse commands.
                var preProductWarehouseCommands =
                    _preProductManagementDomain.FindPreProductWarehouseCommandAsync(terminalNo);

                var command = preProductWarehouseCommands.FirstOrDefault();
                return Json(new
                {
                    command
                });
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Find product shelf status by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindPreProductShelfStatus(FindPrePdtShfStsViewModel condition)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
                var item = await _preProductManagementDomain.FindPrePdtShfSts(condition.ContainerType, terminalNo);

                return Json(new
                {
                    prePdtShfSts = item
                });
            }
            catch (Exception exception)
            {
                if (exception.Message.Equals("MSG21"))
                {
                    //Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    return Json(new
                    {
                        Success = false,
                        Message = PreProductManagementResources.MSG21
                    });
                }

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is related to BR16 - Please refer BR16
        /// Submit storage item.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PreProductStorageOk(StoragePreProductIntoWarehouseViewModel item)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
                await
                    _preProductManagementDomain.PreProductStorageOk(item.LsRow, item.LsBay, item.LsLevel,
                        item.ContainerNo, item.ContainerCode, item.KneadingLine,
                        terminalNo, item.ColorClass);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Please refer BR18.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PreProductStorageNg(RestoreContainerViewModel item)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
                _preProductManagementDomain.ConfirmStorageNg(item.LsRow, item.LsBay, item.LsLevel, item.ContainerNo,
                    item.ContainerCode, item.Conveyor, terminalNo);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Please refer UC-4 : SRS 1.1 Sign off.
        /// </summary>
        /// <returns></returns>
        public ActionResult UC4(string preProductCode)
        {
            // Find terminal which is currently sending request to server.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            // Analyze task after messages sent from C2.
            var response = _preProductManagementDomain.InitiateInformationFromC2(terminalNo, preProductCode);

            return Json(response);
        }

        /// <summary>
        /// Analyze business of UC-6
        /// </summary>
        /// <param name="isChecked"></param>
        /// <param name="preProductCode"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        public ActionResult UC6(SubmitContainerStorageViewModel info)
        {
            if (info == null)
            {
                info = new SubmitContainerStorageViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find which terminal that the request comes from
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var response = _preProductManagementDomain.AnalyzeC2MessageUc6(info.IsChecked, info.PreProductCode,
                info.ContainerCode, terminalNo);

            return Json(response);
        }

        public ActionResult UC7(string preProductCode, string row, string bay, string level, string containerCode,
            string containerNo, string containerType, string commandNo, string preProductLotNo)
        {
            // Find which terminal that the request comes from
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var response = _preProductManagementDomain.ReceiveMessageFromC2WhenClickStorage(terminalNo, preProductCode,row,bay,level,containerCode,containerNo,containerType,commandNo,preProductLotNo);

            return Json(response);

        }

        [HttpPost]
        public ActionResult CheckValidateInMarkRetrievedContainer(string temperature, string preProductCode)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var deviceCode = _configurationService.PreProductDeviceCode;
            var isCheckedConveyor = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!isCheckedConveyor)
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG13
                });

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
                return Json(new { Success = false, Message = PreProductManagementResources.MSG14 });

            var isCheckedTx37 = _preProductManagementDomain.UpdateTX37(temperature, terminalNo, preProductCode);
            if(!isCheckedTx37)
                return Json(new { Success = false, Message = PreProductManagementResources.MSG24 });
            return Json(new{Success=true});
        }

        [HttpPost]
        public ActionResult Storage(StorePreProductViewModel info)
        {
            if (info == null)
            {
                info = new StorePreProductViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            _preProductManagementDomain.Storage(info.CommandNo, info.ContainerCode, info.PreProductLotNo, info.PreProductCode, info.Quantity,
                info.ContainerNo, info.ContainerType, terminalNo, info.LotEndFlag, info.Row, info.Bay, info.Level, info.ColorClass);
            return Json(new {Success = true});
        }

        /// <summary>
        /// Print label.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ActionResult> Print(PrintLabelViewModel parameters)
        {
            try
            {
                #region Parmeters validation

                if (parameters == null)
                {
                    parameters = new PrintLabelViewModel();
                    TryValidateModel(parameters);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                #endregion

                // Export the template.
                var data = await _exportReportDomain.ExportToFlatFileAsync(parameters, _labelPrintService.PreProductLabelOrignalText);
                foreach (var preProductPrinter in _labelPrintService.PreProductPrinters)
                {
                    if (!preProductPrinter.IsEnabled)
                        continue;

                    try
                    {
                        if (preProductPrinter.IsUsbPrinter)
                        {
                            _exportReportDomain.ExportLabelPrint(preProductPrinter,
                                _configurationService.ApplicationDataPath, data);
                        }
                        else
                        {
                            _labelPrintService.Print(
                                new IPEndPoint(IPAddress.Parse(preProductPrinter.Ip), preProductPrinter.Port),
                                data);
                        }
                    }
                    catch
                    {
                        // Suppress printing error.
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
        }
    }
}