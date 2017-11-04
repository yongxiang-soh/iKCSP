using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfMaterial;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR.Messaging;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM031F")]
    public class StorageOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public StorageOfMaterialController(
            IStorageOfMaterialDomain iStorageOfMaterialDomain,
            IMaterialDomain materialDomain,
            IMaterialReceptionDomain materialReceptionDomain,
            ICommonDomain commonDomain,
            IIdentityService identityService, IConfigurationService configurationService)
        {
            _storageOfMaterialDomain = iStorageOfMaterialDomain;
            _materialDomain = materialDomain;
            _materialReceptionDomain = materialReceptionDomain;
            _commonDomain = commonDomain;
            _identityService = identityService;
            _configurationService = configurationService;
        }

        #endregion

        #region Methods
        public ActionResult Index()
        {
            // Screen ID - which will be displayed on the navigation bar.
            ViewBag.ScreenId = Constants.PictureNo.TCRM031F;

            var model = new StorageOfMaterialViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public async Task<JsonResult> Store(StorageOfMaterialViewModel model)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var deviceCode = _configurationService.MaterialDeviceCode;
            try
            {
                // Parameters haven't been initialized.
                if (model == null)
                {
                    model = new StorageOfMaterialViewModel();
                    TryValidateModel(model);
                }

                // Parameter is invalid.
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var validationErrors = ModelState.Values.SelectMany(e => e.Errors);
                    return Json(new { Success = false, Errors = validationErrors });
                }

                // Find list of material shelf stocks by using pallet no.
                var materialShelfStocks = await _storageOfMaterialDomain.FindMaterialShelfStockByPalletNo(model.PalletNo.Trim());

                // Convert enumeration to string.
                var notInStockFlag = ((int)Constants.TX33_MtrShfStk.NotInStock).ToString();


                /*
                 * 	If there is no Material Reception record whose [P.O. No.] = current [P.O. No.], 
                 * [Partial Delivery] = current [Partial Delivery] and [Material Code] = current [Material Code], 
                 * the system will show an error message using template MSG 7
                 * */
                if (!String.IsNullOrEmpty(model.F30_PrcOrdNo))
                {
                    var checkMaterialCode = _storageOfMaterialDomain.CheckMaterialCode(model.F01_MaterialCode, model.F30_PrcOrdNo, model.F30_PrtDvrNo);
                    if (!checkMaterialCode)
                        return Json(new { Success = false, Message = Resources.MaterialResource.MSG7 });
                }
                
                // There is record in the returned list.
                if (materialShelfStocks.Count > 0)
                {
                    if (!materialShelfStocks.Any(m => m.F33_StockFlag.Equals(notInStockFlag)))
                        return Json(new {Success = false, Message = Constants.Messages.StoreMaterial_MSG10});
                }


                // Find material reception by using its primary keys.
                if (!String.IsNullOrEmpty(model.F30_PrcOrdNo) && !String.IsNullOrEmpty(model.F30_PrtDvrNo))
                {
                    var reception = await _materialReceptionDomain.SearchByPrimaryKeys(model.F30_PrcOrdNo.Trim(), model.F30_PrtDvrNo.Trim());
                    if (reception == null)
                        return Json(new { Success = false, Message = Constants.Messages.StoreMaterial_MSG11 });


                    if (reception.F30_ExpectAmount < Convert.ToDouble(model.GrandTotal) + reception.F30_StoragedAmount)
                        return Json(new { Success = false, Message = Constants.Messages.StoreMaterial_MSG11 });
                }

                //Check Unit 
                var isChecked = _storageOfMaterialDomain.CheckUnitOfMaterialDB(model.F01_MaterialCode.Trim(),
                    model.Fraction01, model.Fraction02, model.Fraction03, model.Fraction04, model.Fraction05);
                if (!isChecked)
                    return Json(new {Success = false, Message = Resources.MaterialResource.MSG14});

                //check conveyor code
                var chechConveyorCode = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
                if (!chechConveyorCode)
                    return Json(new {Success = false, Message = Resources.MaterialResource.MSG15});

                //Check device status
                var checkDeviceStatus = _commonDomain.CheckStatusOfDeviceRecord(deviceCode);
                if (!checkDeviceStatus)
                    return Json(new {Success = false, Message = Resources.MaterialResource.MSG16});

                var material = _materialDomain.GetById(model.F01_MaterialCode);
                if (material == null)
                    return Json(new { Success = false, Message = Constants.Messages.StoreMaterial_MSG17 });

                if (!_storageOfMaterialDomain.CheckStorageMaterialShelfStatus(material.F01_LiquidClass))
                    return Json(new { Success = false, Message = Constants.Messages.StoreMaterial_MSG18 });

                //loop to send message to c1
                var storageItem = Mapper.Map<StorageOfMaterialItem>(model);
                _storageOfMaterialDomain.Store(storageItem, terminalNo);
                return Json(new { Success = true, Message = "Create data and send command to C1 successfully!" });
               
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var item in ex.EntityValidationErrors)
                {
                    //... inspect here 
                }
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(null);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(null);
            }

        }


        /// <summary>
        /// process data when receive message for C1
        /// </summary>
        /// <returns></returns>
        public ActionResult PostStoreMaterial()
        {
            var pictureNo = Constants.PictureNo.TCRM031F;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var proceededRecords = _storageOfMaterialDomain.PostStoreMaterial(terminalNo, pictureNo);
            return Json(proceededRecords);
        }


        /// <summary>
        /// Find list material warehouse command
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListMaterialWarehouseCommand()
        {
            var result = _storageOfMaterialDomain.GetListMaterialWarehouseCommand();
            return Json(new
            {
                result
            });
        }

        

        /// <summary>
        /// Check materialCode, PONO,PartialDelivery
        /// </summary>
        /// <returns></returns>       

        #endregion

        #region Domain Declaration

        private readonly IStorageOfMaterialDomain _storageOfMaterialDomain;
        private readonly IMaterialDomain _materialDomain;

        private readonly IMaterialReceptionDomain _materialReceptionDomain;
        private readonly IIdentityService _identityService;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;

        #endregion
    }
}