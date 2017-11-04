using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductShippingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingCommand;
using KCSG.Web.Attributes;
using KCSG.Web.Services;
using KCSG.Web.ViewModels;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR041F")]
    public class ProductShippingCommandController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Domain which handles business of Product shipping command function.
        /// </summary>
        private readonly IProductShippingCommandDomain _productShippingCommandDomain;

        /// <summary>
        /// Service which handles identity of request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Service which handles report export.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        /// <summary>
        /// Domain whose functions are used globally.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        private readonly IConfigurationService _configurationService;        

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with dependency injections.
        /// </summary>
        /// <param name="productShippingCommandDomain"></param>
        /// <param name="identityService"></param>
        /// <param name="exportReportDomain"></param>
        public ProductShippingCommandController(IProductShippingCommandDomain productShippingCommandDomain,
            IIdentityService identityService,
            IExportReportDomain exportReportDomain, ICommonDomain commonDomain, IConfigurationService iConfigurationService)
        {
            _productShippingCommandDomain = productShippingCommandDomain;
            _identityService = identityService;
            _exportReportDomain = exportReportDomain;
            _commonDomain = commonDomain;
            _configurationService = iConfigurationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders product shipping command screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var productShippingCommandViewModel = new ProductShippingCommandViewModel();
            productShippingCommandViewModel.ProductShippingCommandGrid = InitiateProductShippingCommandsGrid();
            productShippingCommandViewModel.ProductShippingCommandDetailsGrid =
                InitiateProductShippingCommandDetailsGrid();
            return View(productShippingCommandViewModel);
        }

        /// <summary>
        /// Find product shipping commands.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindProductShippingCommands(string shippingNo, GridSettings gridSettings)
        {
            try
            {
                // Find a list of shipping commands.
                var shippingCommands = await _productShippingCommandDomain.FindProductShippingCommandsAsync(shippingNo,
                    gridSettings);

                return Json(shippingCommands.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Find product shipping command details.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public  ActionResult FindProductShippingCommandDetails(ProductShippingCommandDetailViewModel parameters,GridSettings gridSettings)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new ProductShippingCommandDetailViewModel();
                    TryValidateModel(parameters);
                }

                if (!string.IsNullOrEmpty(parameters.PalletNo))
                    parameters.PalletNo = parameters.PalletNo.Trim();
                parameters.ProductLotNo = parameters.ProductLotNo.Trim();
                var result =
                    
                        _productShippingCommandDomain.FindProductShippingCommandDetailsAsync(gridSettings,
                            parameters.PalletNo,
                            parameters.ProductLotNo, parameters.ProductCode, double.Parse(parameters.RequestAmount), parameters.ShelfNo);

                return Json(result.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

        }
        /// <summary>
        /// Assign product shipping commands.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult AssignProductShippingCommands(AssignProductShippingViewModel parameters)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new AssignProductShippingViewModel();
                    TryValidateModel(parameters);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                parameters.ProductCode = parameters.ProductCode.Trim();
                parameters.ProductLotNo = parameters.ProductLotNo.Trim();
                parameters.ShippingNo = parameters.ShippingNo.Trim();
                // Find terminal number of current request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                var assignProductResult =  _productShippingCommandDomain.AssignProductAsync(parameters.ShippingNo, parameters.ProductCode,
                    parameters.ProductLotNo, parameters.ShippingQuantity, parameters.ShippedAmount, terminalNo);
                if (assignProductResult == null)
                {
                    return Json(new {Success = false, Message = Resources.ProductManagementResources.MSG1});
                }
                return Json(new
                {
                    assignProductResult,
                    ib_outstk = assignProductResult.Any(i=>i.ib_outstk),
                    ib_instk = assignProductResult.Any(i => i.ib_instk),                    
                });
            }
            catch (Exception exception)
            {
                if ("HTTP-404".Equals(exception.Message))
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Unassign a specific 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UnassignShippingCommands(UnassignShippingCommandViewModel parameters)
        {
            try
            {
                // Parameter hasn't been initialied.
                if (parameters == null)
                {
                    parameters = new UnassignShippingCommandViewModel();
                    TryValidateModel(parameters);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Trim the string.
                parameters.ShippingNo = parameters.ShippingNo.Trim();

                // Find terminal number of request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                await _productShippingCommandDomain.UnassignProduct(parameters.ShippingNo, terminalNo);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Restorage(string palletno)
        {
            try
            {
                // Parameter hasn't been initialied.
                return RedirectToAction("Index", "RestorageOfProduct", new { area = "ProductManagement", palletNo = palletno });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Export product shipping commands by using specific conditions 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ExportProductShippingCommands(ExportProductShippingCommandViewModel parameters)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new ExportProductShippingCommandViewModel();
                    TryValidateModel(parameters);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find results.
                ConfigurationService configurationService = new ConfigurationService();
                var data = new
                {
                    items = await _productShippingCommandDomain.FindProductShippingCommandsForPrinting(parameters.ShippingNo,
                        parameters.ProductCode,parameters.lstPalletNo, parameters.ProductLotNo, parameters.ProductLotNo, parameters.Settings,
                        parameters.ShelfNo),
                    page = parameters.Settings.PageIndex,
                    shippingNo = parameters.ShippingNo,
                    productCode = parameters.ProductCode,
                    productName = parameters.ProductName,
                    currentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                    companyName = configurationService.CompanyName,
                     
                    
                    
                };

                // Template file location detect.
                var file = Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ProductShippingCommand));

                //Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(file);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(data, template);

                return Json(new
                {
                    render
                });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult RetrieveProduct(string shippingNo, string productCode, string productLotNo, string row,
            string bay, string level,string palletNo)
        {
           var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var result = _productShippingCommandDomain.RetrieveProductAsync(shippingNo, productCode, productLotNo,
                    row,
                bay, level, terminalNo, palletNo);

              if (result.IsSuccess)
                {
                return Json(new {Success = true, Message = ""});
                }
              else
              {
                  return
                      Json(
                          new
                          {
                              Success = false,
                              Message =
                                  result.ErrorMessages[0] == "MSG8"
                                      ? Resources.ProductManagementResources.MSG8
                                      : Resources.ProductManagementResources.MSG9
                          });
              }

            }

        [HttpPost]
        public async Task<ActionResult> CheckStatus()
        {
            // Find terminal no from request.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

            // Check whether this terminal has a valid conveyor or not.
            //•	Retrieve [f05_strrtrsts] in TM05_CONVEYOR, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file.
            var conveyor = await _commonDomain.FindConveyorCodeAsync(terminalNo);

            // If retrieval is failed or retrieved status is “Error” (or 9), system shows the message MSG 8, stop the use case.
            if (conveyor == null)
                //throw new Exception("MSG08");
                //return Json(new
                //{
                //    message = "MSG08"                        
                //});
                return Json(new { Success = false, Message = ProductManagementResources.MSG8 });

            // •	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Product Device Code under “Device” section of “toshiba.ini” configurable file.
            //If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case


            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");
            var device = await _commonDomain.FindDeviceAvailabilityAsync(_configurationService.ProductDeviceCode);
            if (device == null)
            {
                return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
            }            
            if (device.F14_DeviceStatus.Equals(offlineStatus) || device.F14_DeviceStatus.Equals(errorStatus) || device.F14_UsePermitClass.Equals(usepermitClass))
            {
                return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
            }
            return Json(new { Success = true });


        }


        /// <summary>
        /// Initate a grid which contains product shipping commands.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateProductShippingCommandsGrid()
        {
            return new Grid("ProductShippingCommandGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("FindProductShippingCommands", "ProductShippingCommand",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F44_ShipCommandNo", SortOrder.Asc)
                .SetFields(
                    new Field("F44_ShipCommandNo")
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false)
                        .SetWidth(20),

                    new Field("F44_ShipCommandNo")
                        .SetTitle("Shipping No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F44_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F44_ProductLotNo")
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F44_ShpRqtAmt")
                        .SetTitle("Shipping Qty")
                         .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F44_ShippedAmount")
                        .SetTitle("Shipped Amt")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100))
                        .OnDataLoaded("onProductShippingDataLoaded");

           
        }

        /// <summary>
        /// Initate a grid which contains product shipping commands.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateProductShippingCommandDetailsGrid()
        {
            return new Grid("ProductShippingCommandDetailsGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("FindProductShippingCommandDetails", "ProductShippingCommand",
                    new { Area = "ProductManagement" }))
                .SetDefaultSorting("F51_ShelfRow", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false)
                        .SetWidth(20),

                    new Field("ShelfNo")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F51_PalletNo")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F40_ProductLotNo")
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F40_Amount")
                        .SetTitle("Assigned Quantity")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("F40_PackageAmount")
                        .SetTitle("Package Amount")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetSorting(false)
                        .SetWidth(100))
                        .OnDataLoaded("loadProductShippingCommandDetails")
                        .SetOnRefreshed("productShippingCommandDetailsRefreshed");
        }

        /// <summary>
        /// Respond message from C3
        /// </summary>
        [HttpPost]
        public ActionResult RespondMessageC3(ReplyThirdCommunicationViewModel parameters)
        {
            if (parameters == null)
            {
                parameters = new ReplyThirdCommunicationViewModel();
                TryValidateModel(parameters);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);    
            

            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _productShippingCommandDomain.RespondingReplyFromC3(terminalNo, parameters.ProductCode);

            return Json(items);
        }

        #endregion
    }
}