using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByShelfStatus;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation;
using KCSG.Web.Attributes;
using KCSG.Web.Controllers;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize(new[] { "TCFC031F", "TCFC033F", "TCFC035F" })]
    public class InquiryByShelfStatusController : InquiryBaseController
    {
        #region Properties
        private readonly ILoginDomain _loginDomain;
        private readonly IInquiryCommonDomain _inquiryCommonDomain;
        private readonly IStockTakingOfMaterialDomain _iStockTakingOfMaterialDomain;

        #endregion
        #region Constructors

        public InquiryByShelfStatusController(ILoginDomain loginDomain,
            IMaterialShelfStatusDomain materialShelfStatusDomain,
            IInquiryCommonDomain inquiryCommonDomain,
            IStockTakingOfMaterialDomain iStockTakingOfMaterialDomain)
        {
            this._loginDomain = loginDomain;
            this._inquiryCommonDomain = inquiryCommonDomain;
            this._iStockTakingOfMaterialDomain = iStockTakingOfMaterialDomain;
        }
        #endregion
        // GET: InquiryByMaterialShelfStatus/Index
        public ActionResult Index(Constants.InquirySearchConditionWarehouseLocation type = Constants.InquirySearchConditionWarehouseLocation.Material)
        {
            if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                ViewBag.Title = "Inquiry by Raw Material Shelf Status";
                ViewBag.ScreenId = "TCFC031F";
                
            }
            if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                ViewBag.Title = "Inquiry by Pre-Product Shelf Status";
                ViewBag.ScreenId = "TCFC033F";
                
            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                ViewBag.Title = "Inquiry by Product Shelf Status";
                ViewBag.ScreenId = "TCFC035F";
                
            }
            var model = new InquiryByShelfStatusSearchViewModel
            {
                SearchCondition = Constants.InquirySearchConditionShelfStatus.FirstRow,
                ExecutingClassification = Constants.ExecutingClassification.Search,
                Type = type
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult PickShelf(Constants.ExecutingClassification executingClassification, Constants.InquirySearchConditionWarehouseLocation type,string row,string bay,string level)
        {
            if (executingClassification == Constants.ExecutingClassification.Update)
            {
                var checkPermission = CheckPermission(type);
                if (!checkPermission.Item1)
                    return Json(new
                    {
                        Success = false,
                        Message = checkPermission.Item2
                    }, JsonRequestBehavior.AllowGet);

                var shelfStatus = _inquiryCommonDomain.GetShelfStatus(type, row,
                    bay, level);
                var shelfNo = string.Format("{0}-{1}-{2}", row, bay, level);
                var localtionMaterialStatusModelView = new InquiryByLocaltionMaterialStatusModelView
                {
                    ShelfNo = shelfNo,
                    ShelfStatus = shelfStatus,
                    ShelfRow = row,
                    ShelfBay = bay,
                    ShelfLevel = level,
                    SearchCondition = type
                };
                if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    var sheftType = _inquiryCommonDomain.GetShelftypeTx51(row, bay,
                        level);
                    if (sheftType == ((int)Constants.F51_ShelfType.BadUse).ToString())
                    {
                        localtionMaterialStatusModelView.ProductShelfType = Constants.F51_ShelfType.BadUse;
                    }
                    else
                    {
                        localtionMaterialStatusModelView.ProductShelfType = Constants.F51_ShelfType.Normal;
                    }
                }
                
                var result = RenderViewToString("_LocationMaterialStatusModal", localtionMaterialStatusModelView);
                return Json(new
                {
                    Success = true,
                    Data = result
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    var shelfStatus =
                        _inquiryCommonDomain.GetShelfStatus(Constants.InquirySearchConditionWarehouseLocation.Material,
                            row, bay, level);

                    if (string.IsNullOrEmpty(shelfStatus))
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = Constants.Messages.SystemManagement_TCSS000041
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (shelfStatus == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Mtr).ToString()) //3
                    {
                        //Inquiry of Raw Material Shelf Status Picture2[TCFC032F] 
                        #region TCFC032F
                        var result = CreateSubScreenTCF032F(executingClassification,type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion 
                    }

                    if (shelfStatus == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_SplPlt).ToString()) //2
                    {
                        //Inquiry by Supplier Pallet [TCFC038F] 
                        #region TCFC038F

                        var result = CreateSubScreenTCF038F(executingClassification, type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion

                    }

                    return Json(new
                    {
                        Success = false,
                        Message = Constants.Messages.SystemManagement_TCSS000041
                    }, JsonRequestBehavior.AllowGet);

                }
                if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    var shelfStatus = _inquiryCommonDomain.GetShelfStatus(type, row,
                        bay, level);

                    if (shelfStatus == ((int)Constants.TX37SheflStatus.TX37_ShfSts_Stk).ToString()) 
                    {
                        //Pre-product Shelf Status Picture [TCFC034F] to continue
                        var result = CreateSubscreenTCFC034F(executingClassification, type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else if (shelfStatus == ((int)Constants.TX37SheflStatus.TX37_ShfSts_EpyCtn).ToString()) 
                    {
                        //Pre-product Shelf Status Picture [Empty Container][TCFC03AF] 
                        #region TCFC03AF

                        var result = CreateSubScreenTCF03AF(executingClassification, type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }

                    return Json(new
                    {
                        Success = false,
                        Message = Constants.Messages.SystemManagement_TCSS000041
                    }, JsonRequestBehavior.AllowGet);
                }

                if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    var shelfStatus = _inquiryCommonDomain.GetShelfStatus(type, row,
                        bay, level);
                    if (shelfStatus == Constants.TX51SheflStatus.TX51_ShfSts_Pdt.ToString("D")) 
                    {
                        //Product Shelf Status Picture2[TCFC036F]
                        #region TCFC036F

                        var result = CreateSubScreenTCF036F(executingClassification, type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }

                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_ExtPrePdt).ToString()) 
                    {
                        //product Shelf Status Picture (External Pre-product) [TCFC037F]  
                        #region TCFC037F

                        var result = CreateSubScreenTCF037F(executingClassification, type, row, bay, level, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        
                        #endregion
                    }

                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_WhsPlt).ToString())
                    {
                        var shelfType = _inquiryCommonDomain.GetShelftypeTx51(row, bay, level);
                        if (shelfType == Constants.F51_ShelfType.Normal.ToString("D"))
                        {
                            //Product Shelf Status Picture (Empty Pallet) [TCFC03BF] 
                            #region TCFC03BF

                            var result = CreateSubScreenTCF03BF(executingClassification, type, row, bay, level, shelfStatus);
                            return Json(new
                            {
                                Success = true,
                                Data = result
                            }, JsonRequestBehavior.AllowGet);

                            
                            #endregion
                        }
                        if (shelfType == Constants.F51_ShelfType.BadUse.ToString("D"))
                        {
                            var data = new InquiryOutOfSignPictureModelView
                            {
                                Grid = GenerateGrid()
                            };
                            var result = RenderViewToString("_OutOfSignPicture", data);
                            return Json(new
                            {
                                Success = true,
                                Data = result
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_BadPrePdt).ToString()) 
                    {
                        var data = new InquiryOutOfSignPictureModelView
                        {
                            Grid = GenerateGrid()
                        };
                        var result = RenderViewToString("_OutOfSignPicture", data);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                    }

                   

                    return Json(new
                    {
                        Success = false,
                        Message = Constants.Messages.SystemManagement_TCSS000041
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    Success = true,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ShowButonCommand(InquiryByShelfStatusSearchViewModel model)
        {
            var result = this.RenderViewToString("_ShowButtonCommand", model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}