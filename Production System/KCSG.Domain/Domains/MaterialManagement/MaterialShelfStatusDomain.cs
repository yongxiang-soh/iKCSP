using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Models.MaterialManagement;
using System.Linq;
using KCSG.Core.Constants;
using System;
using System.Collections.Generic;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class MaterialShelfStatusDomain : BaseDomain, IMaterialShelfStatusDomain
    {

        public MaterialShelfStatusDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }

        public MaterialShelfStatusItem GetByShelfBayLevel(string shelfRow, string shelfBay, string shelfLevel)
        {
            var result = _unitOfWork.MaterialShelfStatusRepository.GetAll().FirstOrDefault(m => (m.F31_ShelfRow.Equals(shelfRow) && m.F31_ShelfBay.Equals(shelfBay) && m.F31_ShelfLevel.Equals(shelfLevel)));
            var item = Mapper.Map<MaterialShelfStatusItem>(result);
            return item;
        }



        #region Methods

        //public bool CheckStorageMaterialShelfStatus()
        //{
        //    var emptyShelf = ((int)KCSG.Core.Constants.Constants.TX31_MtrShfSts_ShelfStatus.EmptyShelf).ToString();
        //    var shelfStatus = _unitOfWork.MaterialShelfStatusRepository.GetAll()
        //        .FirstOrDefault(m => (m.F31_ShelfStatus.Equals(emptyShelf) && (m.F31_CmnShfAgnOrd.HasValue || m.F31_LqdShfAgnOrd.HasValue)));
        //    if (shelfStatus != null) return true;
        //    return false;
        //}

        //public List<TX31_MtrShfSts> GetShelfStorageMaterial(string liquidFlag)
        //{
        //    var emptyShelf = ((int)KCSG.Core.Constants.Constants.TX31_MtrShfSts_ShelfStatus.EmptyShelf).ToString();
        //    List<TX31_MtrShfSts> result = null;
        //    if (liquidFlag.Equals(((int)KCSG.Core.Constants.Constants.TX31_MtrShfSts_LiquidFlag.NonLiquid).ToString()))
        //    {
        //        result = _unitOfWork.MaterialShelfStatusRepository.GetAll().Where(m => (
        //            m.F31_ShelfStatus.Equals(emptyShelf) && m.F31_CmnShfAgnOrd.HasValue
        //        )).ToList();
        //    }
        //    else
        //    {
        //        result = _unitOfWork.MaterialShelfStatusRepository.GetAll().Where(m => (
        //            m.F31_ShelfStatus.Equals(emptyShelf) && m.F31_LqdShfAgnOrd.HasValue
        //        )).ToList();
        //    }
        //    return result;
        //}

        //public MaterialShelfStatusItem Search(string shelfRow, string shelfBay, string shelfLevel)
        //{
        //    var result = _unitOfWork.MaterialShelfStatusRepository.GetAll().FirstOrDefault(m => (m.F31_ShelfRow.Equals(shelfRow) && m.F31_ShelfBay.Equals(shelfBay) && m.F31_ShelfLevel.Equals(shelfLevel)));
        //    var item = Mapper.Map<MaterialShelfStatusItem>(result);
        //    return item;
        //}

        //public void Update(TX31_MtrShfSts shelf)
        //{
        //    _unitOfWork.MaterialShelfStatusRepository.Update(shelf);
        //    _unitOfWork.Commit();
        //}

        //public void Update(MaterialShelfStatusItem shelfStatus)
        //{
        //    var entity = Mapper.Map<TX31_MtrShfSts>(shelfStatus);
        //    _unitOfWork.MaterialShelfStatusRepository.Update(entity);
        //    _unitOfWork.Commit();
        //}

        #endregion
    }
}
