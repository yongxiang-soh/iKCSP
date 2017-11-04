using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfExternalPreProduct;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR101F")]
    public class RetrievalOfExternalPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with dependency injection.
        /// </summary>
        /// <param name="retrievalOfExternalProductDomain"></param>
        /// <param name="commonDomain"></param>
        /// <param name="identityService"></param>
        public RetrievalOfExternalPreProductController(
            IRetrievalOfExternalProductDomain retrievalOfExternalProductDomain, ICommonDomain commonDomain,
            IIdentityService identityService)
        {
            _retrievalOfExternalProductDomain = retrievalOfExternalProductDomain;
            _commonDomain = commonDomain;
            _identityService = identityService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles business of Stock taking of product.
        /// </summary>
        private readonly IRetrievalOfExternalProductDomain _retrievalOfExternalProductDomain;

        /// <summary>
        ///     Domain whose functions are used globally.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        ///     Service which handles identity information from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Methods

        /// <summary>
        ///     Renders index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var retrievalOfExternalPreProduct = new RetrievalOfExternalPreProductViewModel();
            retrievalOfExternalPreProduct.ExternalPreProductGrid = InitiateRetrievalOfExternaPreProductGrid();
            return View(retrievalOfExternalPreProduct);
        }

        /// <summary>
        ///     Find a list of external pre-products.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> FindExternalPreProducts(GridSettings gridSettings)
        {
            try
            {
                // Find list of external products.
                var externalPreProducts = await _retrievalOfExternalProductDomain.FindExternalPreProductsAsync(gridSettings);
                return Json(externalPreProducts.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        

        /// <summary>
        /// Find table listing by using table listing line.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindTableListing(FindTableListingLineViewModel parameters)
        {
            try
            {
                // Parameters haven't been initialized.
                if (parameters == null)
                {
                    parameters = new FindTableListingLineViewModel();
                    TryValidateModel(parameters);
                }

                // Parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
                var device = await _retrievalOfExternalProductDomain.FindDeviceTableListingLine(parameters.TableListingLine);
                return Json(device);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Retrieval external pre-product.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ActionResult> Retrieval(ExternalPreProductViewModel parameters)
        {
            try
            {
                if (parameters == null)
                {
                    parameters = new ExternalPreProductViewModel();
                    TryValidateModel(parameters);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the conveyor from terminal no.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                
                await _retrievalOfExternalProductDomain.RetrievalExternalPreProductAsync(terminalNo, parameters.F41_KndCmdNo,
                    parameters.F41_PreProductCode, parameters.F41_PrePdtLotNo, parameters.Line);

                var httpMessageResponse = new MessageResponseViewModel();
                httpMessageResponse.HttpStatusCode = HttpStatusCode.OK;
                return Json(httpMessageResponse);
            }
            catch (Exception exception)
            {
                var httpMessageResponse = new MessageResponseViewModel();
                httpMessageResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                httpMessageResponse.Message = exception.Message;
                return Json(httpMessageResponse);
            }
        }
        public async Task<ActionResult> RetrievalTabletisingLineName(string Line)
        {


                // Find the conveyor from terminal no.
                var tabletising = _retrievalOfExternalProductDomain.GetTabletingLine(Line);


                return Json(tabletising);

        }
        /// <summary>
        /// Reply third communication.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reply(CommunicationResponseMessageViewModel parameters)
        {
            if (parameters == null)
            {
                parameters = new CommunicationResponseMessageViewModel();
                TryValidateModel(parameters);
            }
                
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _retrievalOfExternalProductDomain.Reply(terminalNo, parameters.PreProductCode, 
                parameters.KneadingCommandNo, parameters.PreProductLotNo);
            return Json(items);
        }

        /// <summary>
        ///     Initiate stock taking of product grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateRetrievalOfExternaPreProductGrid()
        {
            return new Grid("RetrievalOfExternaPreProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("FindExternalPreProducts", "RetrievalOfExternalPreProduct",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F41_KndCmdNo", SortOrder.Asc)
                .SetFields(

                    new Field("F41_KndCmdNo")
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetWidth(20)
                        .SetSorting(false),

                     new Field("F41_KndCmdNo")
                        .SetTitle("Command No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),

                     new Field("F41_PreProductCode")
                        .SetTitle("Pre-product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),

                    new Field("F03_PreProductName")
                        .SetTitle("Pre-product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("F41_PrePdtLotNo")
                        .SetTitle("Pre-product Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),

                    new Field("F42_ThrowAmount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),

                    new Field("Line")
                        .SetTitle("Line")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),

                    new Field("PalletSeqNo")
                        .SetTitle("Pallet Seq No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                );
        }
        
        #endregion
    }
}