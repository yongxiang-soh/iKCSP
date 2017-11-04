using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StockTakingOfMaterial;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM081F")]
    public class StockTakingOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public StockTakingOfMaterialController(
            IStockTakingOfMaterialDomain iStockTakingOfMaterialDomain, IMaterialDomain iMaterialDomain,
            IIdentityService identityDomain, IRetrieveSupplierPalletDomain retrieveSupplierPalletDomain,
            IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain
        )
        {
            _stockTakingOfMaterialDomain = iStockTakingOfMaterialDomain;
            _materialDomain = iMaterialDomain;
            _identityDomain = identityDomain;
            _retrieveSupplierPalletDomain = retrieveSupplierPalletDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCRM081F;
            var model = new StockTakingOfMaterialViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

#if DEBUG

        public ActionResult IndexC1()
        {
            return View(new RestoreMaterialViewModel());
        }
#endif
        [HttpPost]
        public ActionResult IndexC1(RequestRestoreMaterialViewModel parameters)
        {
            if (parameters == null)
            {
                parameters = new RequestRestoreMaterialViewModel();
                TryValidateModel(parameters);
            }

            if (!ModelState.IsValid)
            {
                var httpResponseViewModel = new MessageResponseViewModel();
                httpResponseViewModel.HttpStatusCode = HttpStatusCode.BadRequest;
                httpResponseViewModel.Message = "Invalid parameters submitted to service";
                return Json(httpResponseViewModel);
            }

            var restoreMaterialViewModel = new RestoreMaterialViewModel();
            restoreMaterialViewModel.MaterialCode = parameters.MaterialCode;
            restoreMaterialViewModel.MaterialDsp = parameters.MaterialDsp;
            restoreMaterialViewModel.ShelfNo = parameters.ShelfNo;
            restoreMaterialViewModel.PalletNo = parameters.PalletNo;

            //var materialShelfStocks = _stockTakingOfMaterialDomain.GetMaterialShelfStocks(model.ShelfNo, model.MaterialCode);
            var items = _stockTakingOfMaterialDomain.GetMaterialShelfStocks(parameters.ShelfNo, parameters.MaterialCode);
            var materials = new List<FinalStockTakingMaterialItem>();
            foreach (var item in items)
            {
                var result = Math.Abs(item.F33_Amount / item.F01_PackingUnit - 1);

                var material = new FinalStockTakingMaterialItem();
                if (result < 0.01)
                {
                    material.PackQuantity = 1;
                    material.Fraction = 0.00;
                }
                else
                {
                    material.PackQuantity = Convert.ToInt32(item.F33_Amount / item.F01_PackingUnit);
                    material.Fraction = item.F33_Amount -
                                           item.F01_PackingUnit * Convert.ToDouble(item.F33_Amount / item.F01_PackingUnit);
                }
                material.MaterialLotNo = item.F33_MaterialLotNo;
                material.PackUnit = item.F01_PackingUnit;
                material.Total = item.F33_Amount;
                restoreMaterialViewModel.GrandTotal += item.F33_Amount;

                materials.Add(material);
            }

            restoreMaterialViewModel.Materials = materials.ToArray();
            return View(restoreMaterialViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Retrieve(string firstRowShelfNo, string firstRowPalletNo, string firstRowMaterialCode,
            string firstRowMaterialName, string currentRowPalletNo)
        {
            //Get current terminalNo
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var materialShelfStatusItem =
                _stockTakingOfMaterialDomain.SearchMaterialShelfStatus(firstRowShelfNo.Substring(0, 2),
                    firstRowShelfNo.Substring(2, 2), firstRowShelfNo.Substring(4, 2));
            if (materialShelfStatusItem == null)
                return Json(new { Success = false, Message = "Could not find records in TX31_MtrShfSts table!" },
                    JsonRequestBehavior.AllowGet);

            var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
            if (!checkConveyorStatus)
                return Json(new { Success = false, Message = MaterialResource.MSG15 });

            // Check whether device is valid or not.
            var isValidDevice = _retrieveSupplierPalletDomain.IsValidDevice();
            if (!isValidDevice)
            {
                return Json(new { Success = false, Message = MaterialResource.MSG16 });
            }


            _stockTakingOfMaterialDomain.Retrieve(firstRowShelfNo, firstRowPalletNo, firstRowMaterialCode,
                firstRowMaterialName, currentRowPalletNo, terminalNo);

            //When this action is implemented, there is no solution for handling C1 messages. So in order for "Stock-taking of Raw Material [TCRM082F]" screen
            //to be implemented, the Shelf No, Material Code and Material Name of the first row of the table of "Stock-taking of Raw Material [TCRM081F]" screen
            //will be temporary saved to a session. When the solution to handle C1 messages is implemented, this session shall be removed.
            Session["stockTakingOfMaterialShelfNo"] = firstRowShelfNo;
            Session["stockTakingOfMaterialMaterialCode"] = firstRowMaterialCode;
            Session["stockTakingOfMaterialMaterialName"] = firstRowMaterialName;
            Session["stockTakingOfMaterialCurrentPalletNo"] = currentRowPalletNo;

            return Json(new { Success = true, Message = "Finish retrieving!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult ReStore(RestoreMaterialViewModel model)
        {
            if (model == null)
            {
                model = new RestoreMaterialViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return Json(new {Success = false, Message = "Parameters are invalid"});
            
            //Get current terminalNo
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            #region Additional validations

            //If all of [Material Lot No.] fields are blank, the system will show error message using template MSG 8.
            if (model.Materials.Any( x => string.IsNullOrEmpty(x.MaterialLotNo)))
                return Json(new { Success = false, Message = MessageResource.MSG8_Material }, JsonRequestBehavior.AllowGet);

            //There is at least one of [Material Lot No.] fields which are inputted and their corresponding [Pack Qty]
            //fields are not “0.0” (Valid item), if not, the system will show error message using template MSG 12.
            var hasValidItem = false;
            var validItems = new List<string>();

            foreach (var item in model.Materials)
            {
                if (!string.IsNullOrEmpty(item.MaterialLotNo) && (item.PackQuantity != 0))
                {
                    hasValidItem = true;
                    validItems.Add(item.MaterialLotNo);
                }
               
            }
            
            if (!hasValidItem)
                return Json(new { Success = false, Message = MessageResource.MSG12_Material },
                    JsonRequestBehavior.AllowGet);
            //If there any [Material Lot No.] of “Valid items” are duplicated, the system will show error message using template MSG 13.
            var duplicateItems = validItems.GroupBy(x => x).Where(group => group.Count() > 1).Select(group => group.Key);
            if (duplicateItems.Count() > 0)
                return Json(new { Success = false, Message = MessageResource.MSG13_Material },
                    JsonRequestBehavior.AllowGet);

            //If [Unit] of corresponding Material Master DB record (based on [Material Code]) is not “K” and there any [Fraction]
            //is greater than or equal to corresponding [Pack Unit], the system will show error message using template MSG 14.
            var material = _materialDomain.GetById(model.MaterialCode);
            var unitOK = true;
            if (!material.F01_Unit.Equals("K"))
                unitOK = !model.Materials.Any(x => x.Fraction >= x.PackUnit);
            
            if (!unitOK)
                return Json(new { Success = false, Message = MessageResource.MSG14_Material },
                    JsonRequestBehavior.AllowGet);

            //ToDo: If there is no [tm05_conveyor] record whose [f05_terminalno] is current application terminal or [f05_strrtrsts]
            //is “9” (Error), the system will show error message using template MSG 15.
            //If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” or [f14_devicestatus] is “1” (Offline),
            //[f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1” (Prohibited), the system will show error message using template MSG 16.

            #endregion

            //var modelItem = Mapper.Map<StockTakingOfMaterialC1ViewModelItem>(model);
            _stockTakingOfMaterialDomain.RestoreMaterialStocks(model, terminalNo);
            return Json(new { Success = true, Message = "Finish Re-store material stocks!" });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchMaterial(StockTakingOfMaterialViewModel model, GridSettings gridSettings)
        {
            //if (!string.IsNullOrEmpty(model.ShelfNoFrom) && !string.IsNullOrEmpty(model.ShelfNoTo))
            //{
            var results = _stockTakingOfMaterialDomain.SearchMaterialStock(model.ShelfNoFrom, model.ShelfNoTo,
                gridSettings);
            if (!results.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(results.Data, JsonRequestBehavior.AllowGet);
            //}
            //return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetMaterialStock(string palletNo)
        {
            var lstResult = _stockTakingOfMaterialDomain.GetStockByPalletNo(palletNo.Trim());
            if (lstResult != null)
                return Json(lstResult, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Post retrieval material.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostRetrieveMaterial(StockMaterialComResponseViewModel parameters)
        {
            if (parameters == null)
                return Json(null);

            if (!ModelState.IsValid)
                return Json(null);

            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var response = _stockTakingOfMaterialDomain.PostRetrieveMaterial(terminalNo, parameters.MaterialCode);
            return Json(response);
        }

        [HttpPost]
        public ActionResult CompleteStoraging(StockMaterialComResponseViewModel info)
        {
            if (info == null)
                return Json(null);


            if (!ModelState.IsValid)
                return Json(null);

            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var response = _stockTakingOfMaterialDomain.CompleteStoraging(terminalNo, info.MaterialCode);
            return Json(response);
        }

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("MaterialShelfGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetSelected(true)
                .SetPageLoading(true)
                .SetAutoload(false)
                .OnDataLoaded("loadDataSuccess")
                .SetSearchUrl(urlHelper.Action("SearchMaterial", "StockTakingOfMaterial",
                    new { Area = "MaterialManagement" }))
                .SetDefaultSorting("F03_PreProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F33_PalletNo")
                        .SetWidth(50)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F33_MaterialCode")
                        .SetTitle("Material Code")
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F32_PrcOrdNo")
                        .SetTitle("P.O. No.")
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F32_PrtDvrNo")
                        .SetTitle("PD")
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("AcceptClass")
                        .SetTitle("Accept")
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F33_PalletNo")
                        .SetVisible(false)
                );
        }

        #region Domain Declaration

        private readonly IStockTakingOfMaterialDomain _stockTakingOfMaterialDomain;
        private readonly IMaterialDomain _materialDomain;
        private readonly IIdentityService _identityDomain;
        private readonly IRetrieveSupplierPalletDomain _retrieveSupplierPalletDomain;

        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;

        #endregion
    }
}