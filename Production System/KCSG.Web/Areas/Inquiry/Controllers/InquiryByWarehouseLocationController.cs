using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation;
using KCSG.Web.Attributes;
using KCSG.Web.Infrastructure;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC021F")]
    public class InquiryByWarehouseLocationController : InquiryBaseController
    {
        #region Properties
        private readonly ILoginDomain _loginDomain;
        private readonly IInquiryCommonDomain _inquiryCommonDomain;
        private readonly IStockTakingOfMaterialDomain _iStockTakingOfMaterialDomain;
        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByMaterialNameDomain _inquiryByMaterialNameDomain;
        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByPreProductNameDomain _inquiryByPreProductNameDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion
        #region Constructors

        public InquiryByWarehouseLocationController(ILoginDomain loginDomain,
            IMaterialShelfStatusDomain materialShelfStatusDomain,
            IInquiryCommonDomain inquiryCommonDomain,
            IInquiryByMaterialNameDomain inquiryByMaterialNameDomain,
            IExportReportDomain exportReportDomain,
            IInquiryByPreProductNameDomain inquiryByPreProductNameDomain,
            IStockTakingOfMaterialDomain iStockTakingOfMaterialDomain)
        {
            this._loginDomain = loginDomain;            
            this._inquiryCommonDomain = inquiryCommonDomain;
            this._inquiryByMaterialNameDomain = inquiryByMaterialNameDomain;
            this._exportReportDomain = exportReportDomain;
            this._inquiryByPreProductNameDomain = inquiryByPreProductNameDomain;
            this._iStockTakingOfMaterialDomain = iStockTakingOfMaterialDomain;
        }

        #endregion

        // GET: InquiryByWarehouseLocation/Index
        public ActionResult Index(Constants.InquirySearchConditionWarehouseLocation? searchCondition, Constants.ExecutingClassification? executingClassification)
        {
            var model = new InquiryByWarehouseLocationSearchViewModel
            {
                SearchCondition = searchCondition.HasValue ? searchCondition.Value : Constants.InquirySearchConditionWarehouseLocation.Material,
                ExecutingClassification = executingClassification.HasValue ? executingClassification.Value : Constants.ExecutingClassification.Search,
                ListPrintOption = new List<SelectListItem>(),
                ListShelfNoBay = new List<SelectListItem>(),
                ListShelfNoLevel = new List<SelectListItem>(),
                ListShelfNoRow = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = "01",
                            Selected = true,
                            Text = "01"
                        },
                        new SelectListItem
                        {
                            Value = "02",
                            Text = "02"
                        }
                    }
            };
            return View(setUpModel(model));
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Search")]
        public ActionResult Search(InquiryByWarehouseLocationSearchViewModel model)
        {
            #region SearchCondition = Product & shelftype = 1
            if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                var shelftype = _inquiryCommonDomain.GetShelftypeTx51(model.ShelfNoRow, model.ShelfNoBay,
                    model.ShelfNoLevel);
                if (shelftype == ((int)Constants.F51_ShelfType.BadUse).ToString())
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
            #endregion
            #region ExecutingClassification = Update
            if (model.ExecutingClassification == Constants.ExecutingClassification.Update)
            {
                var checkPermission = CheckPermission(model.SearchCondition);
                if (!checkPermission.Item1)
                    return Json(new
                    {
                        Success = false,
                        Message = checkPermission.Item2
                    }, JsonRequestBehavior.AllowGet);

                var shelfStatus = _inquiryCommonDomain.GetShelfStatus(model.SearchCondition, model.ShelfNoRow,
                    model.ShelfNoBay, model.ShelfNoLevel);
                var shelfNo = string.Format("{0}-{1}-{2}", model.ShelfNoRow, model.ShelfNoBay, model.ShelfNoLevel);
                var localtionMaterialStatusModelView = new InquiryByLocaltionMaterialStatusModelView
                {
                    ShelfNo = shelfNo,
                    ShelfStatus = shelfStatus,
                    ShelfRow = model.ShelfNoRow,
                    ShelfBay = model.ShelfNoBay,
                    ShelfLevel = model.ShelfNoLevel,
                    SearchCondition = model.SearchCondition
                };
                if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    var sheftType = _inquiryCommonDomain.GetShelftypeTx51(model.ShelfNoRow, model.ShelfNoBay,
                        model.ShelfNoLevel);
                    if (sheftType == Constants.F51_ShelfType.BadUse.ToString())
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
            #endregion
            #region ExecutingClassification = Search
            if (model.ExecutingClassification == Constants.ExecutingClassification.Search)
            {
                if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    
                    var shelfStatus = _inquiryCommonDomain.GetShelfStatus(0, model.ShelfNoRow,
                        model.ShelfNoBay, model.ShelfNoLevel);
                    if (shelfStatus == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Mtr).ToString()) //3
                    {
                        #region TCFC032F
                        //Inquiry of Raw Material Shelf Status Picture2[TCFC032F] 
                        var result = CreateSubScreenTCF032F(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,
                            model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        
                        
                    }
                    #endregion 
                    else if (shelfStatus == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_SplPlt).ToString()) //2
                    {
                        //Inquiry by Supplier Pallet [TCFC038F] 
                        #region TCFC038F

                        var result = CreateSubScreenTCF038F(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,
                            model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result,
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                }
                if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    var shelfStatus = _inquiryCommonDomain.GetShelfStatus(model.SearchCondition, model.ShelfNoRow,
                        model.ShelfNoBay, model.ShelfNoLevel);
                    #region TCFC034F
                    if (shelfStatus == ((int)Constants.TX37SheflStatus.TX37_ShfSts_Stk).ToString()) //"3"
                    {
                        //Pre-product Shelf Status Picture [TCFC034F] to continue
                        var result = CreateSubscreenTCFC034F(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow,
                               model.ShelfNoBay, model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);

                    }
                    #endregion
                    else if (shelfStatus == ((int)Constants.TX37SheflStatus.TX37_ShfSts_EpyCtn).ToString()) //"1"
                    {
                        //Pre-product Shelf Status Picture [Empty Container][TCFC03AF] 
                        #region TCFC03AF

                        var result = CreateSubScreenTCF03AF(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        
                        #endregion
                    }

                }
                if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    var shelfStatus = _inquiryCommonDomain.GetShelfStatus(model.SearchCondition, model.ShelfNoRow,
                        model.ShelfNoBay, model.ShelfNoLevel);
                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_Pdt).ToString()) // "2"
                    {
                        //Product Shelf Status Picture2[TCFC036F]
                        #region TCFC036F

                        var result = CreateSubScreenTCF036F(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,
                            model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }

                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_ExtPrePdt).ToString()) // "8"
                    {
                        //product Shelf Status Picture (External Pre-product) [TCFC037F]  
                        #region TCFC037F

                        var result = CreateSubScreenTCF037F(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,
                            model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }

                    if (shelfStatus == ((int)Constants.TX51SheflStatus.TX51_ShfSts_WhsPlt).ToString()) //1
                    {
                        //Shelf Status Picture (Empty Pallet) [TCFC03BF] 
                        #region TCFC03BF

                        var result = CreateSubScreenTCF03BF(model.ExecutingClassification, model.SearchCondition, model.ShelfNoRow, model.ShelfNoBay,
                            model.ShelfNoLevel, shelfStatus);
                        return Json(new
                        {
                            Success = true,
                            Data = result
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                }

                return Json(new
                {
                    Success = false,
                    Message = Constants.Messages.SystemManagement_TCSS000041
                }, JsonRequestBehavior.AllowGet);
            }

            #endregion
            return Json(new
            {
                Success = false,
                Message = Constants.Messages.SystemManagement_TCSS000041
            }, JsonRequestBehavior.AllowGet);
        }
        #region Ultils

        private InquiryByWarehouseLocationSearchViewModel setUpModel(InquiryByWarehouseLocationSearchViewModel model)
        {
            if (model == null)
                model = new InquiryByWarehouseLocationSearchViewModel
                {
                    SearchCondition = Constants.InquirySearchConditionWarehouseLocation.Material,
                    ExecutingClassification = Constants.ExecutingClassification.Search,
                    ListPrintOption = new List<SelectListItem>(),
                    ListShelfNoBay = new List<SelectListItem>(),
                    ListShelfNoLevel = new List<SelectListItem>(),
                    ListShelfNoRow = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = "01",
                            Selected = true,
                            Text = "01"
                        },
                        new SelectListItem
                        {
                            Value = "02",
                            Text = "02"
                        }
                    }
                };

            #region setListShelfNoBay  and setListShelfNoLevel

            if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                for (var i = 1; i <= 13; i++)
                    model.ListShelfNoBay.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                for (var i = 1; i <= 8; i++)
                    model.ListShelfNoLevel.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                model.ListPrintOption = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "0",
                        Selected = true,
                        Text = "Normal"
                    },
                    new SelectListItem
                    {
                        Value = "1",
                        Text = "Bailment"
                    },
                    new SelectListItem
                    {
                        Value = "2",
                        Text = "All"
                    }
                };
            }
            else if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                for (var i = 1; i <= 8; i++)
                    model.ListShelfNoBay.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                for (var i = 1; i <= 7; i++)
                    model.ListShelfNoLevel.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                model.ListPrintOption = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "0",
                        Selected = true,
                        Text = "Normal"
                    },
                    new SelectListItem
                    {
                        Value = "1",
                        Text = "External"
                    },
                    new SelectListItem
                    {
                        Value = "2",
                        Text = "All"
                    }
                };
            }
            else if (model.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                for (var i = 1; i <= 9; i++)
                    model.ListShelfNoBay.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                for (var i = 1; i <= 8; i++)
                    model.ListShelfNoLevel.Add(new SelectListItem
                    {
                        Value = string.Format("{0:00}", i),
                        Selected = i == 1,
                        Text = string.Format("{0:00}", i)
                    });

                model.ListPrintOption = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "0",
                        Selected = true,
                        Text = "Certified"
                    },
                    new SelectListItem
                    {
                        Value = "1",
                        Text = "Not Certified"
                    },
                    new SelectListItem
                    {
                        Value = "2",
                        Text = "All"
                    }
                };
            }

            #endregion

            return model;
        }

        #endregion
    }
}