using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class MaterialShelfDomain : BaseDomain, IMaterialShelfDomain
    {
    

        #region Constructor

        public MaterialShelfDomain(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
          
        }

        #endregion

        #region Methods

        public void Create(MaterialShelfItem materialShelf)
        {
            var entity = Mapper.Map<TX32_MtrShf>(materialShelf);
            _unitOfWork.MaterialShelfRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void DeleteByPalletNo(string palletNo)
        {
            _unitOfWork.MaterialShelfRepository.Delete(s => s.F32_PalletNo.Trim().Equals(palletNo));
            _unitOfWork.Commit();
        }

        #endregion
    }
}
