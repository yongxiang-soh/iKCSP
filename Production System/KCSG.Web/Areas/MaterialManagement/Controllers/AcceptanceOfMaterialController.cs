using System.Net;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.AcceptanceOfMaterial;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM041F")]
    public class AcceptanceOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        ///     Instance which provides functions to access AcceptanceMaterialRepository.
        /// </summary>
        private readonly IAcceptanceOfMaterialDomain _acceptanceOfMaterialDomain;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="acceptanceOfMaterialDomain"></param>
        public AcceptanceOfMaterialController(IAcceptanceOfMaterialDomain acceptanceOfMaterialDomain)
        {
            _acceptanceOfMaterialDomain = acceptanceOfMaterialDomain;
        }

        #endregion

        #region Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("AcceptanceOfMaterial")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetDefaultSorting("F30_ExpectDate", SortOrder.Asc)
                .SetSearchUrl(Url.Action("Search", "AcceptanceOfMaterial", new {Area = "MaterialManagement"}))
                .SetFields(
                    new Field("")
                        .SetWidth(20)
                        .SetTitle("")
                        .SetItemTemplate("gridHelper.generateRadiobox"),
                    new Field("F30_ExpectDate")
                        .SetTitle("Delivery Date")
                        .SetItemTemplate("gridHelper.displayDate"),
                    new Field("F30_PrcOrdNo")
                        .SetWidth(100)
                        .SetTitle("P.O. No ")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F30_PrtDvrNo")
                        .SetWidth(100)
                        .SetTitle("P.D.")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F30_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F30_ExpectAmount")
                        .SetTitle("Delivery Quantity")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("F30_StoragedAmount")
                        .SetTitle("Delivered Quantity")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false)
                );
        }

        /// <summary>
        ///     This function is for rendering Acceptance of Material page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            // This model contains search input information and Grid view of materials list.
            var acceptanceOfMaterialViewModel = new AcceptanceOfMaterialViewModel
            {
                Grid = GenerateGrid()
            };

            return View(acceptanceOfMaterialViewModel);
        }

        /// <summary>
        ///     Search materials by using P.O number and partial delivery.
        /// </summary>
        /// <param name="gridSettings">Setting of grid (pagination)</param>
        /// <param name="pNo">Reference of F30_PrcOrdNo in TX30_Reception</param>
        /// <param name="partialDelivery">Reference of F30_PrtDvrNo in TX30_Reception</param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Search(GridSettings gridSettings, string pNo, string partialDelivery)
        {
            // Find the raw material with conditions and pagination information.
            //if (string.IsNullOrEmpty(pNo) || string.IsNullOrEmpty(partialDelivery))
            //{
            //    return Json(null);
            //}
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F30_ExpectDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _acceptanceOfMaterialDomain.SearchRawMaterial(gridSettings, pNo, partialDelivery);

            // Error occurs while searching for raw material.
            if (!result.IsSuccess)
                return Json(null);

            return Json(result.Data);
        }

        /// <summary>
        ///     Accept a raw material by using P.O Number and partial delivery.
        /// </summary>
        /// <param name="searchMaterialViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Accept(AcceptanceOfMaterialViewModel searchMaterialViewModel)
        {
            // Call accept raw material from domain.
            _acceptanceOfMaterialDomain.AcceptRawMaterial(searchMaterialViewModel.PNo,
                searchMaterialViewModel.PartialDelivery);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Reject a raw material by using P.O Number and partial delivery.
        /// </summary>
        /// <param name="searchMaterialViewModel"></param>
        /// <returns></returns>
        public ActionResult Reject(AcceptanceOfMaterialViewModel searchMaterialViewModel)
        {
            // Call reject material from domain.
            _acceptanceOfMaterialDomain.RejectRawMaterial(searchMaterialViewModel.PNo,
                searchMaterialViewModel.PartialDelivery);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #endregion
    }
}