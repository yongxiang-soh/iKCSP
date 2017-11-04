using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialPostReceptionInput;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM021F")]
    public class MaterialPostReceptionInputController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly IMaterialPostReceptionInputDomain _materialPostReceptionInputDomain;

        #endregion

        #region Constructors

        public MaterialPostReceptionInputController(IMaterialPostReceptionInputDomain materialPostReceptionInputDomain)
        {
            _materialPostReceptionInputDomain = materialPostReceptionInputDomain;
        }

        #endregion

        #region Methods

        //
        // GET: /MaterialManagement/MaterialPostReceptionInput/
        public ActionResult Index()
        {
            var model = new MaterialPostReceptionInputViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F33_MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _materialPostReceptionInputDomain.SearchCriteria(gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<MaterialPostReceptionInputItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult PostReception(string materialCode, string materialName, string shelfNo, string palletNo)
        {
            var model = new MaterialPostReceptionInputViewModel
            {
                F33_MaterialCode = materialCode,
                F01_MaterialDsp = materialName,
                ShelfNo = shelfNo,
                F33_PalletNo = palletNo
            };
            return PartialView("MaterialPostReceptionInput/_PartialViewPostReception", model);
        }
        
        [HttpPost]
        public ActionResult SavePostReception(MaterialPostReceptionInputViewModel model)
        {
            var item = Mapper.Map<MaterialPostReceptionInputItem>(model);
            //Check total quantity
            var checkedTotal = _materialPostReceptionInputDomain.CheckTotalQuantity(item);
            if(!checkedTotal)
                return Json(new{Success=false,Message=Resources.MaterialResource.MSG5});
            var result = _materialPostReceptionInputDomain.SavePostReception(item);
            if (result.IsSuccess)
                return Json(new{Success=true,Message=MessageResource.MSG9}, JsonRequestBehavior.AllowGet);
            return Json(new{Success=false});
        }

        /// <summary>
        /// Find post reception input.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindPostReceptionInputMaterial(FindPostReceptionInputMaterialViewModel parameter)
        {
            #region Parameter validation

            if (parameter == null)
            {
                parameter = new FindPostReceptionInputMaterialViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return Json(null);

            #endregion

            var items = await _materialPostReceptionInputDomain.FindMaterialShelfStocks(parameter.MaterialCode,
                parameter.PalletNo);

            return Json(items);
        }

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("MaterialPostReception")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("Search", "MaterialPostReceptionInput", new {Area = "MaterialManagement"}))
                .SetDefaultSorting("F33_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F33_MaterialCode")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F33_MaterialCode")
                        .SetWidth(100)
                        .SetTitle("Material Code ")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F33_PalletNo")
                        .SetTitle("Pallet No.")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F31_StorageDate")
                        .SetTitle("Storage Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate")
                );
        }

        #endregion
    }
}