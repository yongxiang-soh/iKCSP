using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class ProductShippingPlanningDomain : BaseDomain, IProductShippingPlanningDomain
    {
        #region Constructor

        public ProductShippingPlanningDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
        }

        #endregion

        #region Methods

        public ProductShippingPlanningItem GetById(string id,string productCode,string userCode)
        {
            var entity =
                _unitOfWork.ShippingPlanRepository.GetAll()
                    .FirstOrDefault(i => i.F44_ShipCommandNo.Trim().Equals(id.Trim()));
            var tm09Entity = _unitOfWork.ProductRepository.GetById(productCode.Trim());
            var tm10Entity = _unitOfWork.EndUserRepository.GetById(userCode.Trim());
            var result = Mapper.Map<ProductShippingPlanningItem>(entity);
            //result. = tm03Entity;
            result.F09_ProductDesp = tm09Entity != null ? tm09Entity.F09_ProductDesp : "";
            result.F10_EndUserName = tm10Entity != null ? tm10Entity.F10_EndUserName : "";
            result.DeliveryDate = result.F44_DeliveryDate == null ? null : result.F44_DeliveryDate.Value.ToString("dd/MM/yyyy");
            return result;            
        }

        public void Create(ProductShippingPlanningItem productShippingPlanning)
        {
            var entity = Mapper.Map<TX44_ShippingPlan>(productShippingPlanning);
            _unitOfWork.ShippingPlanRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(ProductShippingPlanningItem productShippingPlanning)
        {
            var entity = Mapper.Map<TX44_ShippingPlan>(productShippingPlanning);
            _unitOfWork.ShippingPlanRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public bool Delete(string prodCode)
        {
            _unitOfWork.ShippingPlanRepository.Delete(s => s.F44_ShipCommandNo.Equals(prodCode.Trim()));
            _unitOfWork.Commit();
            return true;

        }
        public bool Exist(string shippingNo)
        {
           var exist =  _unitOfWork.ShippingPlanRepository.GetAll().Any(m => m.F44_ShipCommandNo.ToUpper().Equals(shippingNo.ToUpper()));
            return exist;
        }

        public ResponseResult<GridResponse<ProductShippingPlanningItem>> SearchCriteria(string codeShipNo,
            GridSettings gridSettings)
        {
            var proShips = _unitOfWork.ShippingPlanRepository.GetAll();

            var endUsers = _unitOfWork.EndUserRepository.GetAll();

            var products = _unitOfWork.ProductRepository.GetAll();

            var test = Constants.F44_Status.F44_Sts_NotShip;
            if (!string.IsNullOrEmpty(codeShipNo))
            {
                var result = from product in products
                             join proShip in proShips on product.F09_ProductCode equals proShip.F44_ProductCode
                             join endUser in endUsers on proShip.F44_EndUserCode equals endUser.F10_EndUserCode
                    where proShip.F44_ShipCommandNo.Contains(codeShipNo.Trim()) &&
                          proShip.F44_Status.Trim().Equals(test)
                    select new ProductShippingPlanningItem
                    {
                        F44_ShipCommandNo = proShip.F44_ShipCommandNo,
                        F44_ProductCode = proShip.F44_ProductCode,
                        F44_ProductLotNo = proShip.F44_ProductLotNo,
                        F44_ShpRqtAmt = proShip.F44_ShpRqtAmt,
                        F44_DeliveryDate = proShip.F44_DeliveryDate,
                        F44_EndUserCode = proShip.F44_EndUserCode,
                        F10_EndUserName = endUser.F10_EndUserName,
                        F09_ProductDesp = product.F09_ProductDesp
                    };

                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                var resultModel = new GridResponse<ProductShippingPlanningItem>(result, itemCount);
                return new ResponseResult<GridResponse<ProductShippingPlanningItem>>(resultModel, true);
            }
            else
            {
                var result = from product in products
                             join proShip in proShips on product.F09_ProductCode equals proShip.F44_ProductCode
                             join endUser in endUsers on proShip.F44_EndUserCode equals endUser.F10_EndUserCode
                    
                             where proShip.F44_Status.Trim().Equals(test)
                             select new ProductShippingPlanningItem
                             {
                                 F44_ShipCommandNo = proShip.F44_ShipCommandNo,
                                 F44_ProductCode = proShip.F44_ProductCode,
                                 F44_ProductLotNo = proShip.F44_ProductLotNo,
                                 F44_ShpRqtAmt = proShip.F44_ShpRqtAmt,
                                 F44_DeliveryDate = proShip.F44_DeliveryDate,
                                 F44_EndUserCode = proShip.F44_EndUserCode,
                                 F10_EndUserName = endUser.F10_EndUserName,
                                 F09_ProductDesp = product.F09_ProductDesp
                             };

                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                var resultModel = new GridResponse<ProductShippingPlanningItem>(result, itemCount);
                return new ResponseResult<GridResponse<ProductShippingPlanningItem>>(resultModel, true);
            }
        }

        public bool CheckUnique(string shipNo)
        {
            return
                _unitOfWork.ShippingPlanRepository.GetAll()
                    .Any(m => m.F44_ShipCommandNo.Trim().Equals(shipNo.Trim()));
        }

        public string GenShippingNo()
        {
            string ShippingNo = "";
            string SubNo = "";
            string currentMonth = DateTime.Now.Month.ToString();
            string currentYear = DateTime.Now.Year.ToString();
            string date = DateTime.Today.ToString("dd");
            //var date = ConvertHelper.ConvertToDateTime(DateTime.Today.ToString("dd")); 
            
            

            var noManages = _unitOfWork.NoManageRepository.Get(i=>i.F48_SystemId.Trim().Equals("00000"));
            if (noManages != null)
            {
                if (noManages.F48_CnrKndCmdNo > 999)
                {
                    noManages.F48_CnrKndCmdNo = 1;                    
                    SubNo = "0" + noManages.F48_CnrKndCmdNo;                                  
                }
                else
                {                    
                    noManages.F48_CnrKndCmdNo += 1;
                    //if (((int)Math.Abs(noManages.F48_CnrKndCmdNo)).ToString().Length = 1)
                    if ((noManages.F48_CnrKndCmdNo).ToString().Length == 1)
                    {
                        SubNo = "0" + noManages.F48_CnrKndCmdNo;
                    }
                    else
                    {                        
                        //SubNo = "0" + (noManages.F48_CnrKndCmdNo).ToString().Substring(noManages.F48_CnrKndCmdNo.ToString().Length - 2);                        
                        SubNo = (noManages.F48_CnrKndCmdNo).ToString().Substring(noManages.F48_CnrKndCmdNo.ToString().Length - 2);
                    }                    
                }
            }
            else
            {
                SubNo = "00";
            }

            if (currentMonth == "10")
            {
                currentMonth = "X";
            } 
            else if (currentMonth == "11")
            {
                currentMonth = "Y";
            }
            else if (currentMonth == "12")
            {
                currentMonth = "Z";
            }
            ShippingNo = currentYear.Substring(0, 1) + currentYear.Substring(currentYear.Length - 2) + currentMonth +
                         //date.ToString().Substring(0, 2) + SubNo;
                         date + SubNo;
            return ShippingNo;
        }

        public bool CheckProductShelfStatus(string proCode, string productLotNo)
        {
            var result = _unitOfWork.ProductShelfStockRepository.GetAll()
                    .Any(i => i.F40_ProductCode.Trim().Equals(proCode) && i.F40_ProductLotNo.Trim().Equals(productLotNo) && i.F40_StockFlag.Equals(Constants.F40_StockFlag.TX40_StkFlg_Stk));            
            return result;

            //string ProductLotNo = productLotNo;
            //var productShelf = _unitOfWork.ProductShelfStockRepository.Get(i => i.F40_ProductCode.Trim().Equals(proCode) && i.F40_ProductLotNo.Trim().Equals(productLotNo) && i.F40_StockFlag.Equals(Constants.F40_StockFlag.TX40_StkFlg_Stk));
            //if (productShelf == null)
            //{
            //    ProductLotNo = "";
            //    //new ResponseResult(false, Constants.Messages.Material_MSG001);                
            //}
            //return ProductLotNo;
        }

        //public ProductShippingPlanningItem CheckReqShippingQty(ProductShippingPlanningItem model)
        //{
        //    double reqShippingQty = model.F44_ShpRqtAmt;
        //    string mode = "" ;            
            
        //    var products = _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(model.F44_ProductCode));
        //    if (products.F09_PackingUnit > 0)
        //    {                
        //        if ((model.F44_ShpRqtAmt) < products.F09_PackingUnit)
        //        {
        //            reqShippingQty = products.F09_PackingUnit;
        //        }
        //        else
        //        {
        //            reqShippingQty = model.F44_ShpRqtAmt;
        //        }
        //        mode = (reqShippingQty / products.F09_PackingUnit).ToString(CultureInfo.InvariantCulture);
        //    }
        //    var results = new ProductShippingPlanningItem()
        //    {
        //        Mode = mode,
        //        F44_ShpRqtAmt = reqShippingQty
        //    };
        //    return results;
        //}
        public ProductShippingPlanningItem CheckReqShippingQty(string productCode, double shippingQty)
        {
            double reqShippingQty = shippingQty;
            string mode = "";

            var products = _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(productCode));
            if (products.F09_PackingUnit > 0)
            {
                if (shippingQty < products.F09_PackingUnit)
                {
                    reqShippingQty = products.F09_PackingUnit;
                }
                else
                {
                    reqShippingQty = shippingQty;
                }
                mode = (reqShippingQty / products.F09_PackingUnit).ToString(CultureInfo.InvariantCulture);
            }
            var results = new ProductShippingPlanningItem()
            {
                Mode = mode,
                F44_ShpRqtAmt = reqShippingQty
            };
            return results;
        }

        public ResponseResult CreateOrUpdate(ProductShippingPlanningItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F44_ShipCommandNo))
                    if (CheckUnique(model.F44_ShipCommandNo))
                        return new ResponseResult(false, Constants.Messages.ProductShipping_MSG21);
                var entity = Mapper.Map<TX44_ShippingPlan>(model);
                entity.F44_ShipCommandNo = GenShippingNo();
                entity.F44_ShippedAmount = 0;
                entity.F44_Status = Constants.F44_Status.F44_Sts_NotShip;
                entity.F44_AddDate = DateTime.Now;
                entity.F44_UpdateDate = DateTime.Now;
                if (model.DeliveryDate != null)
                {
                    entity.F44_DeliveryDate = ConvertHelper.ConvertToDateTimeFull(model.DeliveryDate);
                }                
                entity.F44_UpdateCount = 0;

                _unitOfWork.ShippingPlanRepository.Add(entity);
            }
            else
            {
                //var entity = _unitOfWork.ProductCertificationRepository.GetById(model.F67_ProductCode);
                var entity =
                    _unitOfWork.ShippingPlanRepository.Get(
                        m => m.F44_ShipCommandNo.Trim().Equals(model.F44_ShipCommandNo.Trim()));
                if (entity == null)
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                //Mapper.Map(model, entity);
                entity.F44_ProductCode = model.F44_ProductCode;
                entity.F44_ShpRqtAmt = model.F44_ShpRqtAmt;
                entity.F44_ProductLotNo = model.F44_ProductLotNo;
                //entity.F44_DeliveryDate = model.F44_DeliveryDate;
                entity.F44_EndUserCode = model.F44_EndUserCode;
                entity.F44_UpdateDate = DateTime.Now;
                if (model.DeliveryDate != null)
                {
                    entity.F44_DeliveryDate = ConvertHelper.ConvertToDateTimeFull(model.DeliveryDate);
                }                
                entity.F44_UpdateCount = entity.F44_UpdateCount + 1; ;
                _unitOfWork.ShippingPlanRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public IEnumerable<TX44_ShippingPlan> GetProductShippingPlanning(string productCode)
        {
            return
                _unitOfWork.ShippingPlanRepository.GetAll()
                    .Where(i => i.F44_ProductCode.ToUpper().Contains(productCode.ToUpper()));
        }

        #endregion
    }
}