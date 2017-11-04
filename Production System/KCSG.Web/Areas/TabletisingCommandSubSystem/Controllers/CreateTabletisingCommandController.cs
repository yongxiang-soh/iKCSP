using System;
using System.Data.Entity.Validation;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models.Tabletising;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.TabletisingCommandSubSystem.ViewModels;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using Resources;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Controllers
{
    [Route("Default")]
    [MvcAuthorize("TCMD011F")]
    public class CreateTabletisingCommandController : KCSG.Web.Controllers.BaseController
    {

        #region Properties

        /// <summary>
        /// Domain which handle business of Tabletising command create.
        /// </summary>
        private readonly ICreateTabletisingCommandDomain _createTabletisingCommandDomain;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize controller with dependency injections.
        /// </summary>
        /// <param name="createTabletisingCommandDomain"></param>
        public CreateTabletisingCommandController(ICreateTabletisingCommandDomain createTabletisingCommandDomain)
        {
            _createTabletisingCommandDomain = createTabletisingCommandDomain;
        }

        #endregion


        #region Methods

        /// <summary>
        /// This function is for rendering index page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // This model contains search input information and Grid view of materials list.
            var tabletisingKneadingCommandListViewModel = new TabletisingKneadingCommandListViewModel()
            {
                KneadingCommands = InitializeKneadingCommandsList(),
                TabletisingCommands = InitiateProductInformationGrid(),
                ProductInformation = InitiateProductDetailInformationGrid(),
            };

            return View(tabletisingKneadingCommandListViewModel);
        }

        /// <summary>
        /// This function is for searching tabletising commands from database and display 'em to screen.
        /// TODO: Remove this function.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Search(GridSettings gridSettings)
        {
            try
            {
                // Find the list of kneading commands in tabletising function.
                var kneadingCommands = await _createTabletisingCommandDomain.SearchTabletisingKneadingCommands(gridSettings);

                return Json(kneadingCommands.Data);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is for searching list of kneading commands.
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchKneadingCommands(Pagination pagination)
        {
            try
            {
                // Find the list of kneading commands in tabletising function.
                var gridSettings = Mapper.Map<GridSettings>(pagination);
                var kneadingCommands = await _createTabletisingCommandDomain.SearchTabletisingKneadingCommands(gridSettings);

                //return Json(kneadingCommands.Data);
                return Json(kneadingCommands.Data);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is for searching tabletising commands from database and display 'em to screen.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchProductInformation(GridSettings gridSettings, string preproductCode)
        {
            try
            {
                var tabletisingCommandsList = await _createTabletisingCommandDomain.SearchProductInformation(gridSettings,
                    preproductCode);
                return Json(tabletisingCommandsList.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is for searching product details from database and display 'em to screen.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchProductDetails(GridSettings gridSettings, string kneadingNo, string lotNo, string preproductCode)
        {
            try
            {
                var productDetails = await _createTabletisingCommandDomain.SearchProductDetails(gridSettings,
                    kneadingNo, lotNo, preproductCode);

                if (productDetails == null)
                    return null;

                return Json(productDetails.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult GetProductDetails(string productCode, string productName)
        {
            var result = _createTabletisingCommandDomain.GetProductDetails(productCode, productName);
            return Json(new
            {
                results=result
            });
        }
        
        /// <summary>
        /// This function is for retrieving kneading commands list in database.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> FindKneadingCommandsList(FindTabletisingKneadingCommandViewModel findTabletisingKneadingCommandViewModel)
        {
            try
            {
                // Find the list of kneading commands in tabletising function.
                var kneadingCommands = await _createTabletisingCommandDomain.RetrieveTabletisingKneadingCommand(
                    findTabletisingKneadingCommandViewModel.Page, findTabletisingKneadingCommandViewModel.Records);

                return Json(new
                {
                    findResult = kneadingCommands
                });
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        
        /// <summary>
        /// Check whether the tabletising command is valid for deletion or not.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        public async Task<ActionResult> IsDeleteItemValid(TabletisingKneadingCommandItem tabletisingKneadingCommandItem)
        {
            bool results = await _createTabletisingCommandDomain.IsValidDeleteItem(tabletisingKneadingCommandItem);
            // The deleted item is valid.

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This function is for searching and deleting a tabletising kneading command item.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        public ActionResult DeleteKneadingCommand(
            TabletisingKneadingCommandItem tabletisingKneadingCommandItem)
        {

            try
            {
                //this wont happend because already validation on view
                //// It's worth double checking the item whether it is valid for the deletion or not.
                //if (!await _createTabletisingCommandDomain.IsValidDeleteItem(tabletisingKneadingCommandItem))
                //{
                //    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                //    return Json(new
                //    {
                //        Message = TabletisingResources.MSG11
                //    });
                //}

                // Call delete function from domain.
                _createTabletisingCommandDomain.DeleteTabletisingKneadingCommand(tabletisingKneadingCommandItem);
                return Json(new {Success = true, Message = TabletisingResources.MSG33});
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is for saving production planning when Go button is clicked.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Go(InitiateTabletisingCommandViewModel item)
        {
            var result = await _createTabletisingCommandDomain.InitiateTabletisingCommand(item);
            return Json(new {result});
        }

        /// <summary>
        /// This function is for validating product planning.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ValidateProductPlanning(ProductPlanningValidationItem item)
        {
            var validationMessage = await _createTabletisingCommandDomain.ValidateProductPlanning(item.KneadingNo, item.LotNo, item.Quantity);
            return Json(new
            {
                message = validationMessage
            });
        }

        /// <summary>
        /// This function is for updating the lower table.
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(UpdateLowerTableViewModel updateLowerTableViewModel)
        {
            try
            {
                _createTabletisingCommandDomain.Update(updateLowerTableViewModel.ProductCode,
                    updateLowerTableViewModel.Quantity);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Search product shelf statuses.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchProductShelfStatuses(GridSettings gridSettings,string productCode,string lotNo)
        {
            try
            {
                var productShelfStatuses =
                    await _createTabletisingCommandDomain.SearchProductShelfStatuses(gridSettings, productCode, lotNo);

                if (productShelfStatuses == null)
                    return Json(null);

                return Json(productShelfStatuses.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Search pre product shelf statuses.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchPreProductShelfStatuses(GridSettings gridSettings,string productCode, string lotNo)
        {
            try
            {
                var preProductShelfStatuses = await _createTabletisingCommandDomain.SearchPreProductShelfStatuses(gridSettings, productCode, lotNo);

                if (preProductShelfStatuses == null)
                    return Json(null);

                return Json(preProductShelfStatuses.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult Details(string preProductCode,string lotNo)
        {
            var firstChar = preProductCode[0];
            var model = new TabletisingKneadingCommandListViewModel();
            if (firstChar == 'X')
            {
                model.Detail = InitializeProductShelfStatuses(preProductCode, lotNo);
                return View("_PartialDetailTabletising", model);
            }
            else
            {
                model.Detail = InitializePreProductShelfStatuses(preProductCode, lotNo);
                return View("_PartialDetailTabletising",model);
            }
        }

        /// <summary>
        /// This function is for generating kneading commands grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitializeKneadingCommandsList()
        {
            return new Grid("TabletisingKneadingCommand")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetDefaultSorting("KneadingNo", SortOrder.Asc)
                .SetSearchUrl(Url.Action("Search", "CreateTabletisingCommand", new { Area = "TabletisingCommandSubSystem" }))
                .OnDataLoaded("addFunction")
                .SetFields(
                new Field("KneadingNo")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("KneadingNo")
                        .SetTitle("Kneading No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("PreproductCode")
                        .SetWidth(100)
                        .SetTitle("Pre-Product Code ")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("PreproductName")
                        .SetWidth(100)
                        .SetTitle("Pre-Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("LotNo")
                        .SetTitle("Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("Quantity")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetSorting(false)
                );
        }
        
        /// <summary>
        /// Initiate product general information grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateProductInformationGrid()
        {
            return new Grid("ProductInformation")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetDefaultSorting("F09_ProductCode", SortOrder.Asc)
                .SetSearchUrl(Url.Action("SearchProductInformation", "CreateTabletisingCommand",
                    new {Area = "TabletisingCommandSubSystem"}))
                .SetFields(
                 new Field("F09_ProductCode")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F09_ProductCode")
                        .SetWidth(150)
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F09_ProductDesp")
                        .SetWidth(150)
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("CommandQty")
                        .SetWidth(100)
                        .SetTitle("Command Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false),
                   new Field("Yieldrate")
                        .SetVisible(false),
                    new Field("StorageQty")
                        .SetTitle("Storage Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)
                        .SetSorting(false));

        }

        /// <summary>
        /// Initiate product detail information grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateProductDetailInformationGrid()
        {
            return new Grid("ProductDetailInformation")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("SearchProductDetails", "CreateTabletisingCommand",
                    new { Area = "TabletisingCommandSubSystem" }))
                .SetFields(
                 new Field("ProductCode")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ProductName")
                        .SetWidth(120)
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("TabletisingQuantity")
                        .SetWidth(120)
                        .SetTitle("Tablet Qty")
                        .SetItemTemplate("gridHelper.generateNumericBox")
                        .SetSorting(false),
                    new Field("UsedPreProduct")
                        .SetTitle("UsedPreProd")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(120)
                        .SetSorting(false),
                    new Field("Yieldrate")
                        .SetVisible(false),
                    new Field("LotNo")
                        .SetTitle("Lot No.")
                        .SetItemTemplate("gridHelper.generateTextBox")
                        .SetWidth(120)
                        .SetSorting(false));

        }

        /// <summary>
        /// Initiate product detail information grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitializeProductShelfStatuses(string productCode,string lotNo)
        {
            return new Grid("ProductShelfStatuses")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(5)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("SearchProductShelfStatuses", "CreateTabletisingCommand",
                    new { Area = "TabletisingCommandSubSystem",productCode = productCode ,lotNo= lotNo }))
                .SetFields(
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("PalletNo")
                        .SetWidth(100)
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Quantity")
                        .SetWidth(100)
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false));
        }
        /// <summary>
        /// Initiate product detail information grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitializePreProductShelfStatuses(string productCode, string lotNo)
        {
            return new Grid("PreProductShelfStatuses")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(5)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("SearchPreProductShelfStatuses", "CreateTabletisingCommand",
                    new { Area = "TabletisingCommandSubSystem",productCode = productCode, lotNo = lotNo }))
                .SetFields(
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ContainerCode")
                        .SetWidth(100)
                        .SetTitle("Container Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Quantity")
                        .SetWidth(100)
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false));
        }
        


        #endregion

    }
}