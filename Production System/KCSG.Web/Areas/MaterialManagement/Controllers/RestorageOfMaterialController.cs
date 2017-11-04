using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Web.Areas.MaterialManagement.ViewModels.ReStorageOfMaterial;
using KCSG.Web.Attributes;
using KCSG.Web.Infrastructure;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM051F")]
    public class RestorageOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="restorageOfMaterialDomain"></param>
        /// <param name="identityService"></param>
        public RestorageOfMaterialController(IRestorageOfMaterialDomain restorageOfMaterialDomain,
            IIdentityService identityService, IStorageOfMaterialDomain storageOfMaterialDomain,
            IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain)
        {
            _restorageOfMaterialDomain = restorageOfMaterialDomain;
            _identityDomain = identityService;
            _storageOfMaterialDomain = storageOfMaterialDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which provide functions to handle material storage functions.
        /// </summary>
        private readonly IRestorageOfMaterialDomain _restorageOfMaterialDomain;

        private readonly IIdentityService _identityDomain;
        private readonly IStorageOfMaterialDomain _storageOfMaterialDomain;
        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for rendering Restorage of material index page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(bool? showInRetrivel)
        {
            var model = new UnassignMaterialViewModel() { ShowInRetrivel = showInRetrivel??false };
            return View(model);
        }

        [HttpPost]
        //[SubmitButtonSelector(Name = "Restorage")]
        public ActionResult Restorage(UnassignMaterialViewModel unassignMaterialViewModel)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
                
                if (!_storageOfWarehousePalletDomain.CheckStatusForTX31())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG18
                    }, JsonRequestBehavior.AllowGet);
                }
                if (!_storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG15
                    }, JsonRequestBehavior.AllowGet);
                }
                //	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” 
                //or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) 
                //or [f14_usepermitclass] is “1” (Prohibited), the system will show error message using template MSG 16
                if (!_restorageOfMaterialDomain.IsValidDevice())
                {
                    // This can be confused with the validation error response.
                    //Response.Headers["x-process-error"] = MessageResource.MSG16;
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG16
                    }, JsonRequestBehavior.AllowGet);
                }

                /*
             	If there is no Material Master DB whose [Material Code] is current [Material Code], 
             * the system will show error message using template MSG 17 
            */
                if (!_storageOfWarehousePalletDomain.CheckedRecordInTM01(unassignMaterialViewModel.MaterialCode))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG17
                    }, JsonRequestBehavior.AllowGet);
                }


                /*
                 If there is no [TX31_MtrShfSts] record whose [F31_ShelfStatus] is “0” (empty shelf) 
                 * and [F31_CmnShfAgnOrd] or [F31_LqdShfAgnOrd] are not blank, 
                 * the system will show error message using template MSG 18 "
                 * 
                */
                if (!_storageOfWarehousePalletDomain.CheckStatusForTX31_1())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG18
                    }, JsonRequestBehavior.AllowGet);
                }


                // Find total item inputed from client.

                var restorageMaterialItems = new List<RestorageMaterialItem>();

                restorageMaterialItems.Add(new RestorageMaterialItem()
                {
                    MaterialLotNo = unassignMaterialViewModel.MaterialLotNo01,
                    Fraction = unassignMaterialViewModel.Fraction01,
                    PackQuantity = unassignMaterialViewModel.PackQuantity01,
                    PackUnit = unassignMaterialViewModel.PackUnit01

                });
                if (!string.IsNullOrEmpty(unassignMaterialViewModel.MaterialLotNo02))
                {
                    restorageMaterialItems.Add(new RestorageMaterialItem()
                    {
                        MaterialLotNo = unassignMaterialViewModel.MaterialLotNo02,
                        Fraction = unassignMaterialViewModel.Fraction02,
                        PackQuantity = unassignMaterialViewModel.PackQuantity02,
                        PackUnit = unassignMaterialViewModel.PackUnit02

                    });
                }

                if (!string.IsNullOrEmpty(unassignMaterialViewModel.MaterialLotNo03))
                {
                    restorageMaterialItems.Add(new RestorageMaterialItem()
                    {
                        MaterialLotNo = unassignMaterialViewModel.MaterialLotNo03,
                        Fraction = unassignMaterialViewModel.Fraction03,
                        PackQuantity = unassignMaterialViewModel.PackQuantity03,
                        PackUnit = unassignMaterialViewModel.PackUnit03

                    });
                }


                if (!string.IsNullOrEmpty(unassignMaterialViewModel.MaterialLotNo04))
                {
                    restorageMaterialItems.Add(new RestorageMaterialItem()
                    {
                        MaterialLotNo = unassignMaterialViewModel.MaterialLotNo04,
                        Fraction = unassignMaterialViewModel.Fraction04,
                        PackQuantity = unassignMaterialViewModel.PackQuantity04,
                        PackUnit = unassignMaterialViewModel.PackUnit04

                    });
                }

                if (!string.IsNullOrEmpty(unassignMaterialViewModel.MaterialLotNo05))
                {
                    restorageMaterialItems.Add(new RestorageMaterialItem()
                    {
                        MaterialLotNo = unassignMaterialViewModel.MaterialLotNo05,
                        Fraction = unassignMaterialViewModel.Fraction05,
                        PackQuantity = unassignMaterialViewModel.PackQuantity05,
                        PackUnit = unassignMaterialViewModel.PackUnit05

                    });
                }
                _restorageOfMaterialDomain.RestoreMaterial(unassignMaterialViewModel.PalletNo,
                    unassignMaterialViewModel.MaterialCode, terminalNo, restorageMaterialItems);
                return Json(new
                {
                    Success = true,
                }, JsonRequestBehavior.AllowGet);
                // TODO: Function needed completing due to lack of SRS explainations.
               // return View();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = e.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        ///     This function is for unassign a material pallet.
        ///     Please refer [3.6.2	UC 12: De-assign Material] for more detail.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[SubmitButtonSelector(Name = "Unassign")]
        public ActionResult Unassign(string palletNo, string materialCode)
        {
            try
            {
                _restorageOfMaterialDomain.UnassginMaterial(palletNo,
                    materialCode);
                return Json(new
                {
                    Success = true,
                    Message = "Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Success = false,
                    Message = e.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     This function is for cleaning material information.
        ///     This function uses the same view model as Unassign but only material code is actually used.
        /// </summary>
        /// <param name="palletNo"></param>
        ///<param name="materialCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Empty(string palletNo,string materialCode)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            if (!_storageOfWarehousePalletDomain.CheckStatusForTX31())
            {
                return Json(new
                {
                    Success = false,
                    Message = MaterialResource.MSG18
                }, JsonRequestBehavior.AllowGet);
            }
            if (!_storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo))
            {
                return Json(new
                {
                    Success = false,
                    Message = MaterialResource.MSG15
                }, JsonRequestBehavior.AllowGet);
            }
            //	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” 
            //or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) 
            //or [f14_usepermitclass] is “1” (Prohibited), the system will show error message using template MSG 16
            if (!_restorageOfMaterialDomain.IsValidDevice())
            {
                // This can be confused with the validation error response.
                //Response.Headers["x-process-error"] = MessageResource.MSG16;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new
                {
                    Success = false,
                    Message = MaterialResource.MSG16
                }, JsonRequestBehavior.AllowGet);
            }            

            try
            {
                _restorageOfMaterialDomain.EmptyMaterialStorage(materialCode, terminalNo);

            }
            catch (Exception exception)
            {
                if (exception.Message.Equals("MSG18"))
                {
                    //Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    //Response.Headers["x-process-error"] = MessageResource.MSG18;
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return Json(new
                    {
                        Success = false,
                        Message = MaterialResource.MSG18
                    }, JsonRequestBehavior.AllowGet);
                }

                throw;
            }

            return Json(new
            {
                Success = true,Message=ProductManagementResources.MSG57
            }, JsonRequestBehavior.AllowGet);
           
        }

        /// <summary>
        ///     Find validation errors and construct 'em as json object.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        private IDictionary<string, string[]> FindValidationErrors(ModelStateDictionary modelStateDictionary)
        {
            return ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// process data when receive message for C1
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PostStoreMaterial()
        {
            var pictureNo = Constants.PictureNo.TCRM051F;
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
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
        /// Base on material code, pallet no
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FindMaterialDetails(FindMaterialLotNoViewModel condition)
        {
            if (condition == null)
            {
                condition = new FindMaterialLotNoViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var information = _restorageOfMaterialDomain.FindMaterialShelfStocks(condition.MaterialCode, condition.PalletNo, terminalNo);
            
            return Json(information);
        }

        #endregion
    }
}