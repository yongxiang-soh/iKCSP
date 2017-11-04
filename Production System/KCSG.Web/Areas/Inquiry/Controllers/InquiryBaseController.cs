using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Data.Repositories;
using KCSG.Domain.Domains;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.Inquiry.ByWareHouseLocation;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Common.ViewModels;
using KCSG.Web.Controllers;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation;
using Newtonsoft.Json;
using Resources;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    public class InquiryBaseController : BaseController
    {
        public ActionResult SearchOutOfSignPicture(GridSettings settings)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var tx51s =
                inquiryCommonDomain.GetAllPdtShfStsByShelfType(
                    ((int) Constants.TX51ShelfType.TX51_ShelfType_BadUse).ToString());
            var a = tx51s.Select(i => new
            {
                ShelfNo = string.Format("{0}-{1}-{2}", i.F51_ShelfRow, i.F51_ShelfBay, i.F51_ShelfLevel),
                ShelfStatus = EnumsHelper.GetEnumDescriptionWithDefaultValue(
                    (Constants.TX51SheflStatusShelfTypeBadFor039)
                    Enum.Parse(typeof(Constants.TX51SheflStatusShelfTypeBadFor039), i.F51_ShelfStatus.Trim()),"")
            });
            var count = a.Count();
            a = a.OrderBy(i => i.ShelfNo).Skip((settings.PageIndex - 1)*settings.PageSize).Take(settings.PageSize);
            var reuslt = new GridResponse<object>(a, count);

            return Json(reuslt, JsonRequestBehavior.AllowGet);
        }

        #region f032f

        protected string CreateSubScreenTCF032F(Constants.ExecutingClassification aimode, Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var iStockTakingOfMaterialDomain = DependencyResolver.Current.GetService<IStockTakingOfMaterialDomain>();
            var rawMaterialShelftStatusItem = inquiryCommonDomain.GetRawMaterialShelftStatusItem(shelfNoRow,
                shelfNoBay, shelfNoLevel);
            var tcf032f = new InquiryRawMaterialShelfStatusModelView();
            if (rawMaterialShelftStatusItem != null)
            {
                tcf032f = Mapper.Map<InquiryRawMaterialShelfStatusModelView>(rawMaterialShelftStatusItem);
                var materialShelfStockItems =
                    iStockTakingOfMaterialDomain.GetStockByPalletNo(rawMaterialShelftStatusItem.PalletNo);
                tcf032f.MaterialLotNo1 = materialShelfStockItems.Count >= 1
                    ? materialShelfStockItems[0].F33_MaterialLotNo.Trim()
                    : string.Empty;
                tcf032f.Quantity1 = materialShelfStockItems.Count >= 1
                    ? materialShelfStockItems[0].F33_Amount
                    : 0;
                tcf032f.MaterialLotNo2 = materialShelfStockItems.Count >= 2
                    ? materialShelfStockItems[1].F33_MaterialLotNo.Trim()
                    : string.Empty;
                tcf032f.Quantity2 = materialShelfStockItems.Count >= 2
                    ? materialShelfStockItems[1].F33_Amount
                    : 0;
                tcf032f.MaterialLotNo3 = materialShelfStockItems.Count >= 3
                    ? materialShelfStockItems[2].F33_MaterialLotNo.Trim()
                    : string.Empty;
                tcf032f.Quantity3 = materialShelfStockItems.Count >= 3
                    ? materialShelfStockItems[2].F33_Amount
                    : 0;

                tcf032f.MaterialLotNo4 = materialShelfStockItems.Count >= 4
                    ? materialShelfStockItems[3].F33_MaterialLotNo.Trim()
                    : string.Empty;
                tcf032f.Quantity4 = materialShelfStockItems.Count >= 4
                    ? materialShelfStockItems[3].F33_Amount
                    : 0;

                tcf032f.MaterialLotNo5 = materialShelfStockItems.Count >= 5
                    ? materialShelfStockItems[4].F33_MaterialLotNo.Trim()
                    : string.Empty;
                tcf032f.Quantity5 = materialShelfStockItems.Count >= 5
                    ? materialShelfStockItems[4].F33_Amount
                    : 0;
            }
            else
            {
                tcf032f.ShelfStatus = "";
            }
            var selectValue =
                    (Constants.TX31OlDSheflStatus)
                    Enum.Parse(typeof(Constants.TX31OlDSheflStatus), shelfStatus);
            tcf032f.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf032f.Row = shelfNoRow;
            tcf032f.Bay = shelfNoBay;
            tcf032f.Level = shelfNoLevel;
            tcf032f.SearchCondition = searchCondition;
            if (tcf032f.StorageDate == null)
            {
                tcf032f.StorageDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            tcf032f.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);

            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf032f.ShowUpdate = false;
            }
            else { tcf032f.ShowUpdate = true; }

            return RenderViewToString("_SubscreenTCFC032F", tcf032f);
        }


        [HttpPost]
        public ActionResult UpdateSubscreentcf032f(InquiryRawMaterialShelfStatusModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();

                    //var olddata = inquiryCommonDomain.GetRawMaterialShelftStatusItem(model.Row,model.Bay, model.Level);
                    //var oldPalletNo = string.Empty;
                    //if (olddata != null) oldPalletNo = olddata.PalletNo.Trim();
                    //if ((!string.IsNullOrEmpty(oldPalletNo) && oldPalletNo != model.PalletNo.Trim()) || !string.IsNullOrWhiteSpace(model.PrcordNo))
                    //{
                    //    var canUpdate = inquiryCommonDomain.CanUpdate(model.PalletNo.Trim(), oldPalletNo, model.MaterialCode, model.PrcordNo,
                    //        model.PrtdvrNo);
                    //    if (!canUpdate.Item1)
                    //    {
                    //        return Json(new
                    //        {
                    //            Success = false,
                    //            Message = canUpdate.Item2
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    
                    var identityDomain = DependencyResolver.Current.GetService<IIdentityService>();
                    var rawMaterialShelftStatusItem = Mapper.Map<RawMaterialShelftStatusItem>(model);
                    var terminalNo = identityDomain.FindTerminalNo(HttpContext.User.Identity);
                    inquiryCommonDomain.UpdateSubscreentcf032f(rawMaterialShelftStatusItem, terminalNo);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// get AcceptanceClassification for Sub screen TCFC032F
        /// </summary>
        /// <param name="prcordNo"></param>
        /// <param name="prtdvrNo"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAcceptanceClassification(string prcordNo, string prtdvrNo)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();

            return Json(new
            {
                response = inquiryCommonDomain.GetAcceptanceClassification(prcordNo, prtdvrNo)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpOptions]
        public JsonResult PalletNo032fExit(string row,string bay,string level, string palletno)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var rawMaterialShelftStatusItem = inquiryCommonDomain.GetRawMaterialShelftStatusItem(row,
                bay, level);
            if (rawMaterialShelftStatusItem == null)
            {
                var canUpdate = inquiryCommonDomain.PalletNo032fExit(palletno);
                return Json(new {Success = canUpdate.Item1, Message = canUpdate.Item2},
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!rawMaterialShelftStatusItem.PalletNo.Trim().Equals(palletno))
                {
                    var canUpdate = inquiryCommonDomain.PalletNo032fExit(palletno);
                    return Json(new { Success = canUpdate.Item1, Message = canUpdate.Item2 },
                               JsonRequestBehavior.AllowGet);
                }
            }
            
            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }
        [HttpOptions]
        public JsonResult MaterialCode032fExit(string prcordNo, string prtdvrNo, string material)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var canUpdate = inquiryCommonDomain.MaterialCode032fExit(prcordNo, prtdvrNo, material);
            return Json(new { Success = canUpdate.Item1, Message = canUpdate.Item2 },
                       JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region f034f

        protected string CreateSubscreenTCFC034F(Constants.ExecutingClassification aimode, Constants.InquirySearchConditionWarehouseLocation searchCondition, string shelfNoRow,string shelfNoBay,string shelfNoLevel, string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var preProductShelftStatusItem = inquiryCommonDomain.GetPreProductShelftStatusItem(shelfNoRow,
                               shelfNoBay, shelfNoLevel);
            var tcf034f = new InquiryByPreProductShelfStatusModelView();
            if (preProductShelftStatusItem != null)
            {
                tcf034f = Mapper.Map<InquiryByPreProductShelfStatusModelView>(preProductShelftStatusItem);
            }
            else
            {
                tcf034f.ShelfStatus = "";
                tcf034f.StorageDate = DateTime.Now.ToString("dd/MM/yyyy");
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf034f.ShowUpdate = false;

            }
            else { tcf034f.ShowUpdate = true; }
            var selectValue =
                    (Constants.TX37OldSheflStatus)
                    Enum.Parse(typeof(Constants.TX37OldSheflStatus), shelfStatus);
            tcf034f.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf034f.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf034f.Row = shelfNoRow;
            tcf034f.Bay = shelfNoBay;
            tcf034f.Level = shelfNoLevel;
            tcf034f.SearchCondition = searchCondition;
            return RenderViewToString("_SubscreenTCFC034F", tcf034f);

        }

        /// <summary>
        /// 	If user already changed value of [Container SeqNo]: 	Count item from table TX49_PrePdtShfStk where f49_containerseqno = [Container SeqNo]. After executing, if the count number > 0 then the system will show a warning message with content “This Container Seq No is already existed !”.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="containerSeqNo"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult CheckContainerSeqNo034(string row, string bay, string level, int containerSeqNo)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var currentData = inquiryCommonDomain.GetPreProductShelftStatusItem(row,
                bay, level);

            if (currentData == null)
            {
                var canUpdate = inquiryCommonDomain.CanUpdate034f(containerSeqNo);
                if (!canUpdate.Item1)
                {
                    return Json(new {Success = false, Message = canUpdate.Item2}, JsonRequestBehavior.AllowGet);
                }
            }
            else if (currentData.ContainerSeqNo != containerSeqNo)
            {
                var canUpdate = inquiryCommonDomain.CanUpdate034f(containerSeqNo);
                if (!canUpdate.Item1)
                {
                    return Json(new { Success = false, Message = canUpdate.Item2 }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateSubscreentcf034f(InquiryByPreProductShelfStatusModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
                    var currentData = inquiryCommonDomain.GetPreProductShelftStatusItem(model.Row, model.Bay, model.Level);

                    if (currentData == null)
                    {
                        var canUpdate = inquiryCommonDomain.CanUpdate034f(model.ContainerSeqNo);
                        if (!canUpdate.Item1)
                        {
                            return Json(new { Success = false, Message = canUpdate.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (currentData.ContainerSeqNo != model.ContainerSeqNo)
                    {
                        var canUpdate = inquiryCommonDomain.CanUpdate034f(model.ContainerSeqNo);
                        if (!canUpdate.Item1)
                        {
                            return Json(new { Success = false, Message = canUpdate.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    
                    var preProductShelfStatusItem = Mapper.Map<PreProductShelfStatusItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf034f(preProductShelfStatusItem);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false,Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Getkndcmd(string precode, string prelot)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            return Json(inquiryCommonDomain.Getkndcmd(precode, prelot), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region f038f

        protected string CreateSubScreenTCF038F(Constants.ExecutingClassification aimode,Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryBySupplierPalletItem = inquiryCommonDomain.GetInquiryBySupplierPalletItem(shelfNoRow, shelfNoBay, shelfNoLevel);
            var tcf038f = new InquiryBySupplierPalletModelView();
            if (inquiryBySupplierPalletItem != null)
            {
                tcf038f = Mapper.Map<InquiryBySupplierPalletModelView>(inquiryBySupplierPalletItem);
            }
            else
            {
                tcf038f.ShelfStatus = "";
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf038f.ShowUpdate = false;

            }
            else { tcf038f.ShowUpdate = true; }
            var selectValue =
                    (Constants.TX31OlDSheflStatus)
                    Enum.Parse(typeof(Constants.TX31OlDSheflStatus), shelfStatus);
            tcf038f.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf038f.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf038f.Row = shelfNoRow;
            tcf038f.Bay = shelfNoBay;
            tcf038f.Level = shelfNoLevel;
            tcf038f.SearchCondition = searchCondition;
            return RenderViewToString("_SubscreenTCFC038F", tcf038f);
        }
        [HttpPost]
        public ActionResult UpdateSubscreentcf038f(InquiryBySupplierPalletModelView model)
        {
            if (model.StockedPallet > model.MaxPallet || model.StockedPallet <= 0)
                return Json(new { Success = false, Message = InQuiryResource.TCFC038F01 },
                           JsonRequestBehavior.AllowGet);
            if (ModelState.IsValid)
            {
                try
                {
                    var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
                    var inquiryBySupplierPalletItem = Mapper.Map<InquiryBySupplierPalletItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf038f(inquiryBySupplierPalletItem);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region f03Af

        protected string CreateSubScreenTCF03AF(Constants.ExecutingClassification aimode, Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByPreProductShelfStatusEmptyItem = inquiryCommonDomain.GetInquiryByPreProductShelfStatusEmptyItem(shelfNoRow,
                           shelfNoBay, shelfNoLevel);
            var tcf03Af = new InquiryByPreProductShelfStatusEmptyModelView();
            if (inquiryByPreProductShelfStatusEmptyItem != null)
            {
                tcf03Af =
                    Mapper.Map<InquiryByPreProductShelfStatusEmptyModelView>(inquiryByPreProductShelfStatusEmptyItem);
            }
            else
            {
                tcf03Af.ShelfStatus = "";
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf03Af.ShowUpdate = false;

            }
            else { tcf03Af.ShowUpdate = true; }
            var selectValue =
                    (Constants.TX37OldSheflStatus)
                    Enum.Parse(typeof(Constants.TX37OldSheflStatus), shelfStatus);
            tcf03Af.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf03Af.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf03Af.Row = shelfNoRow;
            tcf03Af.Bay = shelfNoBay;
            tcf03Af.Level = shelfNoLevel;
            tcf03Af.SearchCondition = searchCondition;
            return RenderViewToString("_SubscreenTCFC03AF", tcf03Af);
        }

        [HttpPost]
        public ActionResult UpdateSubscreentcf03Af(InquiryByPreProductShelfStatusEmptyModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
                    var inquiryByPreProductShelfStatusEmptyItem = Mapper.Map<InquiryByPreProductShelfStatusEmptyItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf03Af(inquiryByPreProductShelfStatusEmptyItem);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region f036f

        protected string CreateSubScreenTCF036F(Constants.ExecutingClassification aimode, Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByProductShelfStatusItem = inquiryCommonDomain.GetInquiryByProductShelfStatusItem(shelfNoRow, shelfNoBay, shelfNoLevel);
            var tcf036f = new InquiryByProductShelfStatusModelView();
            if (inquiryByProductShelfStatusItem != null)
            {
                tcf036f = Mapper.Map<InquiryByProductShelfStatusModelView>(inquiryByProductShelfStatusItem);
            }
            else
            {
                tcf036f.ShelfStatus = "";
                tcf036f.StorageDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf036f.ShowUpdate = false;

            }
            else { tcf036f.ShowUpdate = true; }
            var selectValue =
                    (Constants.TX51SheflStatus)
                    Enum.Parse(typeof(Constants.TX51SheflStatus), shelfStatus);
            tcf036f.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf036f.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf036f.Row = shelfNoRow;
            tcf036f.Bay = shelfNoBay;
            tcf036f.Level = shelfNoLevel;
            tcf036f.SearchCondition = searchCondition;
            return RenderViewToString("_SubscreenTCFC036F", tcf036f);
            
        }
        [HttpOptions]
        public JsonResult GetProductLotNo036f(string productcode, string preProductLotNo,int productClassification)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            return Json(new
            {
                response = inquiryCommonDomain.GetProductLotNo036f(productcode.Trim(), preProductLotNo.Trim(), productClassification)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpOptions]
        public JsonResult GetPackingUnit036f(string productcode)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            return Json(new
            {
                response = inquiryCommonDomain.GetPackingUnit036f(productcode.Trim())
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpOptions]
        public JsonResult ProductLotNoNotbeentabletised(string productcode, string preProductLotNo, int productClassification)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var success = inquiryCommonDomain.ProductLotNoNotbeentabletised(productcode.Trim(), preProductLotNo.Trim(),
                productClassification);
            return Json(new { Success = success, Message = string.Empty },
                       JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 	If user changed value of [Pallet No] (current [Pallet No] is not [Pallet No] when the form is beling loaded): if the new value is already existed in the table TX57_PdtShf, the system will show a warning message with content: “This pallet is being used !”. Set focus on [Pallet No] field.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletno"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult PalletNo036fExit(string row, string bay, string level, string palletno)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByProductShelfStatusItem = inquiryCommonDomain.GetInquiryByProductShelfStatusItem(row,
                bay, level);
            if (inquiryByProductShelfStatusItem == null)
            {
                if (inquiryCommonDomain.PalletNoExit(palletno))
                {
                    return Json(new {Success = false, Message = InQuiryResource.TCSS000024},
                        JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (!inquiryByProductShelfStatusItem.PalletNo.Trim().Equals(palletno.Trim()))
                {
                    if (inquiryCommonDomain.PalletNoExit(palletno.Trim()))
                    {
                        return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                           JsonRequestBehavior.AllowGet);
                    }

                }
            }
            
            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 	If [Fraction] > Packing Unit (retrieved from CBR 19 above) then the system shows a warning message with content “Fraction cannot be more than the packing unit !”. Set focus on [Fraction] field
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletno"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult FractionlessthanPackingunit(double fraction, string productCode)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var response = inquiryCommonDomain.GetPackingUnit036f(productCode.Trim());

            if (fraction > response)
            {
                return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                               JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, Message = string.Empty },
                               JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult UpdateSubscreentcf036f(InquiryByProductShelfStatusModelView model)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByProductShelfStatusItem =
                inquiryCommonDomain.GetInquiryByProductShelfStatusItem(model.Row, model.Bay, model.Level);
            if (inquiryByProductShelfStatusItem == null)
            {
                if (inquiryCommonDomain.PalletNoExit(model.PalletNo))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                           JsonRequestBehavior.AllowGet);
                }

            }else if (!model.PalletNo.Trim().Equals(inquiryByProductShelfStatusItem.PalletNo.Trim()))
            {
                if (inquiryCommonDomain.PalletNoExit(model.PalletNo))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                           JsonRequestBehavior.AllowGet);
                }
            }

            if (string.IsNullOrEmpty(model.ProductCode1) && string.IsNullOrEmpty(model.ProductCode2) && string.IsNullOrEmpty(model.ProductCode3) &&
                string.IsNullOrEmpty(model.ProductCode4) && string.IsNullOrEmpty(model.ProductCode5))
            {
                return Json(new { Success = false, Message = InQuiryResource.TCSS000018 },
                           JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.ProductCode1) && !string.IsNullOrEmpty(model.PreProductLotNo1) 
                && ((!string.IsNullOrEmpty(model.ProductCode2) && !string.IsNullOrEmpty(model.ProductCode2) && model.ProductCode1.Trim().Equals(model.ProductCode2.Trim()) && model.PreProductLotNo1.Trim().Equals(model.PreProductLotNo2.Trim())) 
                    || (!string.IsNullOrEmpty(model.ProductCode3) && !string.IsNullOrEmpty(model.ProductCode3) &&  model.ProductCode1.Trim().Equals(model.ProductCode3.Trim()) && model.PreProductLotNo1.Trim().Equals(model.PreProductLotNo3.Trim()))
                    || (!string.IsNullOrEmpty(model.ProductCode4) && !string.IsNullOrEmpty(model.ProductCode4) &&  model.ProductCode1.Trim().Equals(model.ProductCode4.Trim()) && model.PreProductLotNo1.Trim().Equals(model.PreProductLotNo4.Trim()))
                    || (!string.IsNullOrEmpty(model.ProductCode5) && !string.IsNullOrEmpty(model.ProductCode5) &&  model.ProductCode1.Trim().Equals(model.ProductCode5.Trim()) && model.PreProductLotNo1.Trim().Equals(model.PreProductLotNo5.Trim()))
                ))
            {
                return Json(new { Success = false, Message = InQuiryResource.TCFC036F04 },
                           JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.ProductCode2) && !string.IsNullOrEmpty(model.PreProductLotNo2)
                && ((!string.IsNullOrEmpty(model.ProductCode3) && !string.IsNullOrEmpty(model.ProductCode3) &&  model.ProductCode2.Trim().Equals(model.ProductCode3.Trim()) && model.PreProductLotNo2.Trim().Equals(model.PreProductLotNo3.Trim()))
                    || (!string.IsNullOrEmpty(model.ProductCode4) && !string.IsNullOrEmpty(model.ProductCode4) && model.ProductCode2.Trim().Equals(model.ProductCode4.Trim()) && model.PreProductLotNo2.Trim().Equals(model.PreProductLotNo4.Trim()))
                    || (!string.IsNullOrEmpty(model.ProductCode5) && !string.IsNullOrEmpty(model.ProductCode5) && model.ProductCode2.Trim().Equals(model.ProductCode5.Trim()) && model.PreProductLotNo2.Trim().Equals(model.PreProductLotNo5.Trim()))
                ))
            {
                return Json(new { Success = false, Message = InQuiryResource.TCFC036F04 },
                           JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.ProductCode3) && !string.IsNullOrEmpty(model.PreProductLotNo3)
                && ((!string.IsNullOrEmpty(model.ProductCode4) && !string.IsNullOrEmpty(model.ProductCode4) &&  model.ProductCode3.Trim().Equals(model.ProductCode4.Trim()) && model.PreProductLotNo3.Trim().Equals(model.PreProductLotNo4.Trim()))
                    || (!string.IsNullOrEmpty(model.ProductCode5) && !string.IsNullOrEmpty(model.ProductCode5) && model.ProductCode3.Trim().Equals(model.ProductCode5.Trim()) && model.PreProductLotNo3.Trim().Equals(model.PreProductLotNo5.Trim()))
                ))
            {
                return Json(new { Success = false, Message = InQuiryResource.TCFC036F04 },
                           JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.ProductCode4) && !string.IsNullOrEmpty(model.PreProductLotNo4) && !string.IsNullOrEmpty(model.ProductCode5) && !string.IsNullOrEmpty(model.ProductCode5)
                && model.ProductCode4.Trim().Equals(model.ProductCode5.Trim()) && model.PreProductLotNo4.Trim().Equals(model.PreProductLotNo5.Trim()))
            {
                return Json(new { Success = false, Message = InQuiryResource.TCFC036F04 },
                           JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dataUpdate = Mapper.Map<InquiryByProductShelfStatusItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf036f(dataUpdate);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region f037f

        protected string CreateSubScreenTCF037F(Constants.ExecutingClassification aimode,Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByProductShelfStatusExternalPreProductItem = inquiryCommonDomain.GetProductShelftStatusExternalPreProductItem(shelfNoRow, shelfNoBay, shelfNoLevel);
            var tcf037f = new InquiryByProductShelfStatusExternalPreProductModelView();
            if (inquiryByProductShelfStatusExternalPreProductItem != null)
            {
                tcf037f =
                    Mapper.Map<InquiryByProductShelfStatusExternalPreProductModelView>(
                        inquiryByProductShelfStatusExternalPreProductItem);
            }
            else
            {
                tcf037f.ShelfStatus = string.Empty;
                tcf037f.StorageDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf037f.ShowUpdate = false;
            }
            else { tcf037f.ShowUpdate = true; }
            var selectValue =
                    (Constants.TX51SheflStatus)
                    Enum.Parse(typeof(Constants.TX51SheflStatus),
                        shelfStatus);
            tcf037f.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf037f.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf037f.Row = shelfNoRow;
            tcf037f.Bay = shelfNoBay;
            tcf037f.Level = shelfNoLevel;
            tcf037f.SearchCondition = searchCondition;

            return RenderViewToString("_SubscreenTCFC037F", tcf037f);
        }

        /// <summary>
        /// 	Check current pre-product is external or not as following: SELECT Count(*) FROM TX42_KndCmd WHERE TX42_KndCmd.F42_OutSideClass = 1 and F42_PreProductCode = [Pre - product Code] and F42_PrePdtLotNo = [Pre - product LotNo]
        ///	If return number = 0 then the system shows a warning message with content “This pre-product lot-no is not external !”.
        /// </summary>
        /// <param name="preProductCode"></param>
        /// <param name="preproductLotNo"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult PreproductIsExternal(string preProductCode, string preproductLotNo)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            if (!inquiryCommonDomain.PreProductIsexternal(preProductCode.Trim(), preproductLotNo.Trim()))
            {
                return Json(new { Success = false, Message = "This pre-product lot-no is not external !" },
                           JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 	If user changed value of [Pallet No], if new value is already existed in the table   TX57_PdtShf (column f57_palletno) the system will show a warning message with content “This pallet is being used !”.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult PalletNoExit037(string row, string bay, string level, string palletNo)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryProductShelfStatusExternalPreProductItem = inquiryCommonDomain.GetProductShelftStatusExternalPreProductItem(row, bay, level);

            if (inquiryCommonDomain.PalletNoExit(palletNo.Trim()))
            {
                if (inquiryProductShelfStatusExternalPreProductItem == null)
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                      JsonRequestBehavior.AllowGet);
                }

                if (!inquiryProductShelfStatusExternalPreProductItem.PalletNo.Trim().Equals(palletNo.Trim()))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                      JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 	If user changed value of [Pallet No], if new value is already existed in the table   TX57_PdtShf (column f57_palletno) the system will show a warning message with content “This pallet is being used !”.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletSeqNo"></param>
        /// <returns></returns>
        [HttpOptions]
        public JsonResult PalletSeqNoExit037(string row, string bay, string level, int palletSeqNo)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryProductShelfStatusExternalPreProductItem = inquiryCommonDomain.GetProductShelftStatusExternalPreProductItem(row, bay, level);

            if (inquiryCommonDomain.PalletSeqNoExit(palletSeqNo))
            {
                if (inquiryProductShelfStatusExternalPreProductItem == null)
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCFC037F02 },
                       JsonRequestBehavior.AllowGet);
                }

                if (!inquiryProductShelfStatusExternalPreProductItem.PalletSeqNo.Equals(palletSeqNo))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCFC037F02 },
                      JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = true, Message = string.Empty },
                           JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateSubscreentcf037f(InquiryByProductShelfStatusExternalPreProductModelView model)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();

            if (!inquiryCommonDomain.PreProductIsexternal(model.PreProductCode,model.PreProductLotNo))
            {
                return Json(new { Success = false, Message = "This pre-product lot-no is not external !" },
                           JsonRequestBehavior.AllowGet);
            }

            var inquiryProductShelfStatusExternalPreProductItem =inquiryCommonDomain.GetProductShelftStatusExternalPreProductItem(model.Row, model.Bay, model.Level);

            if (inquiryCommonDomain.PalletNoExit(model.PalletNo))
            {
                if (inquiryProductShelfStatusExternalPreProductItem == null)
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                      JsonRequestBehavior.AllowGet);
                }

                if (!inquiryProductShelfStatusExternalPreProductItem.PalletNo.Trim().Equals(model.PalletNo))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCSS000024 },
                      JsonRequestBehavior.AllowGet);
                }
               
            }

            if (inquiryCommonDomain.PalletSeqNoExit(model.PalletSeqNo))
            {
                if (inquiryProductShelfStatusExternalPreProductItem == null)
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCFC037F02 },
                       JsonRequestBehavior.AllowGet);
                }

                if (!inquiryProductShelfStatusExternalPreProductItem.PalletSeqNo.Equals(model.PalletSeqNo))
                {
                    return Json(new { Success = false, Message = InQuiryResource.TCFC037F02 },
                      JsonRequestBehavior.AllowGet);
                }
            }
           
            if (ModelState.IsValid)
            {
                try
                {
                    var dataUpdate = Mapper.Map<InquiryByProductShelfStatusExternalPreProductItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf037f(dataUpdate);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region f03Bf

        protected string CreateSubScreenTCF03BF(Constants.ExecutingClassification aimode, Constants.InquirySearchConditionWarehouseLocation searchCondition,
            string shelfNoRow, string shelfNoBay, string shelfNoLevel,string shelfStatus)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var inquiryByProductShelfStatusEmptyItem = inquiryCommonDomain.GetInquiryByProductShelfStatusEmptyItem(shelfNoRow,
                           shelfNoBay, shelfNoLevel);
            var tcf03bf = new InquiryByProductShelfStatusEmptyModelView();
            if (inquiryByProductShelfStatusEmptyItem != null)
            {
                tcf03bf = Mapper.Map<InquiryByProductShelfStatusEmptyModelView>(inquiryByProductShelfStatusEmptyItem);
            }
            else
            {
                tcf03bf.ShelfStatus = string.Empty;
            }
            if (aimode == Constants.ExecutingClassification.Search)
            {
                tcf03bf.ShowUpdate = false;
            }
            else
            {
                tcf03bf.ShowUpdate = true;
            }
            var selectValue =
                    (Constants.TX51SheflStatus)
                    Enum.Parse(typeof(Constants.TX51SheflStatus),
                        shelfStatus);
            tcf03bf.MaxLoad = Constants.F14Maxload.MaxLoad;
            var storageOfEmptyProductPalletDomain = DependencyResolver.Current.GetService<IStorageOfEmptyProductPalletDomain>();
            var device = storageOfEmptyProductPalletDomain.CheckMaxiumNumberOfIpAddress();
            if (device != null)
            {
                if (device.F14_IPAddress1.HasValue)
                {
                    tcf03bf.MaxLoad = device.F14_IPAddress1.Value;
                }
            }
            tcf03bf.ShelfStatus = EnumsHelper.GetEnumDescription(selectValue);
            tcf03bf.ShelfNo = string.Format("{0}-{1}-{2}", shelfNoRow, shelfNoBay, shelfNoLevel);
            tcf03bf.Row = shelfNoRow;
            tcf03bf.Bay = shelfNoBay;
            tcf03bf.Level = shelfNoLevel;
            tcf03bf.SearchCondition = searchCondition;
            return RenderViewToString("_SubscreenTCFC03BF", tcf03bf);
            
        }

        [HttpPost]
        public ActionResult UpdateSubscreentcf03Bf(InquiryByProductShelfStatusEmptyModelView model)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var storageOfEmptyProductPalletDomain = DependencyResolver.Current.GetService<IStorageOfEmptyProductPalletDomain>();
            var device = storageOfEmptyProductPalletDomain.CheckMaxiumNumberOfIpAddress();
            var maxLoad = Constants.F14Maxload.MaxLoad;
            if (device != null)
            {
                if (device.F14_IPAddress1.HasValue)
                {
                    maxLoad = device.F14_IPAddress1.Value;
                }
            }
            if (model.PalletLoadAmout > maxLoad) 
            {
                return Json(new { Success = false, Message = InQuiryResource.TCSS000042 },
                          JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var dataUpdate = Mapper.Map<InquiryByProductShelfStatusEmptyItem>(model);
                    inquiryCommonDomain.UpdateSubscreentcf03Bf(dataUpdate);
                    return Json(new
                    {
                        Success = true,
                        Message = MessageResource.MSG9
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Message = "Can't update this record !" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public Grid GenerateGrid()
        {
            return new Grid("Grid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("SearchOutOfSignPicture", "InquiryByWarehouseLocation",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("ShelfNo", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetWidth(50)
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ShelfStatus")
                        .SetTitle("Shelf Status")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                );
        }

        [HttpPost]
        public JsonResult UpdateLocationMaterial(Constants.InquirySearchConditionWarehouseLocation searchCondition, string status, string shelfRow, string shelfBay, string shelfLevel)
        {
            var inquiryCommonDomain = DependencyResolver.Current.GetService<IInquiryCommonDomain>();
            var iStockTakingOfMaterialDomain = DependencyResolver.Current.GetService<IStockTakingOfMaterialDomain>();
            #region Material
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                if (status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Epy).ToString()) //1. Empty SRS, 0 in enum
                {
                    inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Material,
                        shelfRow, shelfBay, shelfLevel,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }
                if (status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_WhsPlt).ToString() || //2. Pallet SRS, 1 in enum 
                    status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_RsvStg).ToString() || //5.Store SRS, 4 in enum 
                    status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_RsvRtr).ToString() //6.Retr SRS, 5 in enum
                   )
                {
                    inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Material,
                        shelfRow, shelfBay, shelfLevel,true);
                    inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Material, shelfRow,
                        shelfBay, shelfLevel, status,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

                if (status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Pbt).ToString())  //Forbid SRS)
                {
                    inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Material, shelfRow,
                        shelfBay, shelfLevel, status,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

                if (status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_SplPlt).ToString())//3. Suppl SRS, 2 in enum 
                {
                    //Inquiry by Supplier Pallet [TCFC038F] 
                    #region TCFC038F

                    var result = CreateSubScreenTCF038F(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                    return Json(new
                    {
                        Success = true,
                        IsShowNewModel = true,
                        Data = result
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }

                if (status == ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Mtr).ToString())//4. Mat SRS, 3 in enum 
                {
                    //Inquiry of Raw Material Shelf Status Picture2[TCFC032F] 
                    #region TCFC032F
                    var result = CreateSubScreenTCF032F(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                    return Json(new
                    {
                        Success = true,
                        IsShowNewModel = true,
                        Data = result
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }
            }
            #endregion
            #region PreProduct
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                if (status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_Epy).ToString()) //1. Empty SRS, 0 in enum
                {
                    inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct,
                       shelfRow, shelfBay, shelfLevel,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

                if (status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_EpyCtn).ToString())
                {
                    //Pre-product Shelf Status Picture [Empty Container][TCFC03AF] 
                    #region TCFC03AF

                    var result = CreateSubScreenTCF03AF(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                    return Json(new
                    {
                        Success = true,
                        Data = result,
                        IsShowNewModel = true,
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }

                if (status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_Stk).ToString())
                {
                    //Pre-product Shelf Status Picture [TCFC034F]
                    #region TCFC034F
                    var result = CreateSubscreenTCFC034F(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);

                    return Json(new
                    {
                        Success = true,
                        Data = result,
                        IsShowNewModel = true,
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }

                if (status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_RsvStg).ToString() ||  //4.Store
                    status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_RsvRtr).ToString())
                {
                    inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct,
                        shelfRow, shelfBay, shelfLevel,true);
                    inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct,
                        shelfRow,
                        shelfBay, shelfLevel, status,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

                if (status == ((int)Constants.TX37SheflStatus.TX37_ShfSts_Pbt).ToString()) // 6. Forbid
                {
                    inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct,
                        shelfRow,
                        shelfBay, shelfLevel, status,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            #endregion
            #region Product
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                var shelfTypeTx51 = inquiryCommonDomain.GetShelftypeTx51(shelfRow,
                    shelfBay, shelfLevel);
                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_Epy).ToString())
                {
                    if (shelfTypeTx51 == ((int)Constants.TX51ShelfType.TX51_ShelfType_BadUse).ToString())
                    {
                        inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow,
                            shelfBay, shelfLevel, status,true);
                    }
                    else
                    {
                        inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow, shelfBay, shelfLevel,true);
                    }
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }

                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_WhsPlt).ToString())
                {
                    if (shelfTypeTx51 == ((int)Constants.TX51ShelfType.TX51_ShelfType_BadUse).ToString())
                    {
                        inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow, shelfBay, shelfLevel, status,true);

                    }
                    else
                    {
                        //Inquiry by product Shelf Status Picture (Empty Pallet) [TCFC03BF]
                        #region TCFC03BF

                        var result = CreateSubScreenTCF03BF(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                        return Json(new
                        {
                            Success = true,
                            Data = result,
                            IsShowNewModel = true,
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                }

                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_Pdt).ToString())
                {
                    //Inquiry by product Shelf Status Picture (Empty Pallet) [TCFC036F]
                    #region TCFC036F

                    var result = CreateSubScreenTCF036F(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                    return Json(new
                    {
                        Success = true,
                        Data = result,
                        IsShowNewModel = true,
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }

                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_BadPrePdt).ToString())
                {
                    /*
                     * If ls_newstatustext = “Out/Spec” then system opens screen Inquiry by Product Shelf Status Picture2[TCFC036F]  to continue.
                        Get Product Shelf Type as BR 20.
                        If Product Shelf Type = {tx51_shelftype_baduse} (1) then
                        System calls common fucntion f_tcsetstatus(2, ls_row, ls_bay, ls_level, TX51_ShfSts_BadPrePdt). Refer to common business CB 2: Update Status Function for more details.
                        After update successfully, system closes this screen and re-directs to the previous screen.                         
                     */
                    if (shelfTypeTx51 == Constants.TX51ShelfType.TX51_ShelfType_BadUse.ToString("D"))
                    {
                        inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow, shelfBay, shelfLevel, status,true);
                    }
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);

                }

                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_ExtPrePdt).ToString())
                {
                    //Picture (External Pre-product) [TCFC037F]  to continue.
                    #region TCFC037F
                    var result = CreateSubScreenTCF037F(Constants.ExecutingClassification.Update, searchCondition, shelfRow, shelfBay, shelfLevel, status);
                    return Json(new
                    {
                        Success = true,
                        Data = result,
                        IsShowNewModel = true,
                    }, JsonRequestBehavior.AllowGet);
                    #endregion

                }

                if (status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_RsvStr).ToString() || //Store
                    status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_RsvRtr).ToString() || //Retr
                    status == ((int)Constants.TX51SheflStatus.TX51_ShfSts_Pbt).ToString()) //Forbid
                {
                    inquiryCommonDomain.Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow, shelfBay, shelfLevel,true);

                    inquiryCommonDomain.Setshelf(Constants.InquirySearchConditionWarehouseLocation.Product,
                            shelfRow,
                            shelfBay, shelfLevel, status,true);
                    return Json(new
                    {
                        Success = true,
                        Message = Resources.MessageResource.MSG9,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            
            return Json(new
            {
                Success = true,
                Message = Resources.MessageResource.MSG9,
            }, JsonRequestBehavior.AllowGet);
        }

        public Tuple<bool, string> CheckPermission(Constants.InquirySearchConditionWarehouseLocation searchCondition)
        {
            var screenId = string.Empty;
            var identityService = DependencyResolver.Current.GetService<IIdentityService>();
            // Find available screens.
            var availableScreens = identityService.FindAccessibleScreens(HttpContext.User.Identity);
            // Initiate authentication result.
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                screenId = "TCFC022F";
            }
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                screenId = "TCFC023F";
            }
            if (searchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                screenId = "TCFC024F";
            }
            if (identityService.IsScreenAccessible(availableScreens, new[] { screenId }))
            {
                return new Tuple<bool, string>(true, string.Empty);
            }
            else
            {
                //This terminal is not permitted to call this function !
                return new Tuple<bool, string>(false, "This terminal is not permitted to call this function !");
            }
        }
    }
}