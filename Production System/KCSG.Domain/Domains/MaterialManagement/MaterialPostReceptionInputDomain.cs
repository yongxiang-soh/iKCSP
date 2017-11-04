using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class MaterialPostReceptionInputDomain : BaseDomain, IMaterialPostReceptionInputDomain
    {
        #region Constructor

        public MaterialPostReceptionInputDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
        }

        #endregion

        #region Methods

        public MaterialItem GetById(string id)
        {
            var entity = _unitOfWork.MaterialRepository.GetById(id);
            return Mapper.Map<MaterialItem>(entity);
        }

        public ResponseResult<GridResponse<MaterialPostReceptionInputItem>> SearchCriteria(GridSettings gridSettings)
        {
            var materialLst = _unitOfWork.MaterialRepository.GetAll();
            var materialShelfStatusLst = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            var materialShelfStockLst = _unitOfWork.MaterialShelfStockRepository.GetAll();
            var materialShelfLst = _unitOfWork.MaterialShelfRepository.GetAll();

            var result = from materialShelfStock in materialShelfStockLst
                         from material in materialLst
                         from materialShelf in materialShelfLst
                         from materialShelfStatus in materialShelfStatusLst
                             where materialShelfStock.F33_MaterialCode.Equals(material.F01_MaterialCode)
                         && materialShelfStock.F33_PalletNo.Equals( materialShelf.F32_PalletNo)
                         && materialShelfStock.F33_PalletNo.Equals( materialShelfStatus.F31_PalletNo)
                         && materialShelf.F32_PrcOrdNo == null
                         && materialShelf.F32_PrtDvrNo == null
                         select new
                         {
                             F33_MaterialCode = material.F01_MaterialCode,
                             material.F01_MaterialDsp,
                             ShelfNo =
                                 materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" +
                                 materialShelfStatus.F31_ShelfLevel,
                             F33_PalletNo = materialShelf.F32_PalletNo,
                             materialShelfStatus.F31_StorageDate,
                             materialShelf.F32_StorageDate
                         };

            //var result = from materialShelfStock in materialShelfStockLst
            //    join material in materialLst on materialShelfStock.F33_MaterialCode.Trim() equals
            //        material.F01_MaterialCode.Trim()
            //    join materialShelf in materialShelfLst on materialShelfStock.F33_PalletNo.Trim() equals
            //        materialShelf.F32_PalletNo.Trim()
            //    join materialShelfStatus in materialShelfStatusLst on materialShelfStock.F33_PalletNo.Trim() equals
            //        materialShelfStatus.F31_PalletNo.Trim()
            //    where (materialShelf.F32_PrcOrdNo.Trim() == null) && (materialShelf.F32_PrtDvrNo.Trim() == null)
            //    select new
            //    {
            //        F33_MaterialCode = material.F01_MaterialCode,
            //        material.F01_MaterialDsp,
            //        ShelfNo =
            //            materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" +
            //            materialShelfStatus.F31_ShelfLevel,
            //        F33_PalletNo = materialShelf.F32_PalletNo,
            //        materialShelfStatus.F31_StorageDate
            //    };
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);

            //var lstResult = Mapper.Map<MaterialPostReceptionInputItem>(result);
            var resultLst = result.ToList().Select(p => new MaterialPostReceptionInputItem
            {
                F33_MaterialCode = p.F33_MaterialCode,
                F01_MaterialDsp = p.F01_MaterialDsp,
                ShelfNo = p.ShelfNo,
                F33_PalletNo = p.F33_PalletNo,
                F31_StorageDate = p.F32_StorageDate
            });

            var resultModel = new GridResponse<MaterialPostReceptionInputItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<MaterialPostReceptionInputItem>>(resultModel, true);
        }

        public ResponseResult SavePostReception(MaterialPostReceptionInputItem model)
        {
            //find tx30 record 
            var reception =
                _unitOfWork.ReceptionRepository.Get(
                    r =>
                        r.F30_PrcOrdNo.Trim().Equals(model.P_O_No.Trim()) &&
                        r.F30_PrtDvrNo.Trim().Equals(model.PartialDelivery.Trim()));

            // not found
            if (reception == null)
                return new ResponseResult(false, Constants.Messages.Material_MSG001);
            // material_code must be same
            //if (reception.F30_MaterialCode.Trim() != model.F33_MaterialCode.Trim())
            //    return new ResponseResult(false, Constants.Messages.Material_MSG001);

            //update tx30
            var amount = model.Quantity1 + model.Quantity2 + model.Quantity3 + model.Quantity4 + model.Quantity5;
            reception.F30_StoragedAmount += amount;
            reception.F30_UpdateDate = DateTime.Now;
            reception.F30_UpdateCount += 1;
            _unitOfWork.ReceptionRepository.Update(reception);

            // update tx32
            var materialShelf = _unitOfWork.MaterialShelfRepository.Get(
                m => m.F32_PalletNo.Trim().Equals(model.F33_PalletNo.Trim()) && (m.F32_PrcOrdNo.Trim() == null));

            if (materialShelf != null)
            {
                //Mapper.Map(model, entity);
                materialShelf.F32_PrcOrdNo = model.P_O_No;
                materialShelf.F32_PrtDvrNo = model.PartialDelivery;
                materialShelf.F32_UpdateDate = DateTime.Now;
                materialShelf.F32_UpdateCount += 1;
                _unitOfWork.MaterialShelfRepository.Update(materialShelf);
            }

            //Change database
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        //Check Material Reception does not have any record whose [Material Code] = current [Material Code], [P.O. No.] = current [P.O. No.] and [Partial Delivery] = current [Partial Delivery],
        public bool CheckedMaterialReceptionExists(string materialCode, string pONo, string partialDelivery)
        {
            var materialReception =
                _unitOfWork.ReceptionRepository.GetMany(
                    i =>
                        i.F30_MaterialCode.Trim().Equals(materialCode.Trim()) &&
                        i.F30_PrcOrdNo.Trim().Equals(pONo.Trim()) &&
                        i.F30_PrtDvrNo.Trim().Equals(partialDelivery.Trim()));
            if (materialCode.Any())
                return true;
            return false;
        }


        /// <summary>
        /// Refer br10 - srs material management v1.0.1
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckTotalQuantity(MaterialPostReceptionInputItem item)
        {
            var reception =
                _unitOfWork.ReceptionRepository.Get(
                    i =>
                        i.F30_PrcOrdNo.Trim().Equals(item.P_O_No.Trim()) &&
                        i.F30_PrtDvrNo.Trim().Equals(item.PartialDelivery.Trim()));

            var total = item.Quantity1 + item.Quantity2 + item.Quantity3 + item.Quantity4 + item.Quantity5 +
                        reception.F30_StoragedAmount;
            if (total > reception.F30_ExpectAmount)
                return false;
            return true;
        }

        /// <summary>
        /// Find material shelf stocks by using material code 
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public async Task<IList<TX33_MtrShfStk>> FindMaterialShelfStocks(string materialCode, string palletNo)
        {
            var shelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();
            shelfStocks =
                shelfStocks.Where(
                    x => x.F33_MaterialCode.Trim().Equals(materialCode) && x.F33_PalletNo.Trim().Equals(palletNo));

            return await shelfStocks.ToListAsync();
        }
        #endregion
    }
}