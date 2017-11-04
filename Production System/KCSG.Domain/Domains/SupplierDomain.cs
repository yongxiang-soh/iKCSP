using System.Collections.Generic;
using System.Linq;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains
{
    public class SupplierDomain :BaseDomain, ISupplierDomain
    {
        #region Constructor

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonDomain"></param>
        public SupplierDomain(IUnitOfWork unitOfWork, ICommonDomain commonDomain)
            : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _commonDomain = commonDomain;
        }

        #endregion

        public bool CheckUnique(string code)
        {
            return _unitOfWork.SupplierRepossitories.GetAll().Any(m => m.F04_SupplierCode.Equals(code.Trim()));
        }

        public IEnumerable<TM04_Supplier> GetSuppliers(string code)
        {
            return _unitOfWork.SupplierRepossitories.GetAll().Where(s => s.F04_SupplierCode.Contains(code));
        }

        public ResponseResult<GridResponse<SupplierItem>> GetSupplierCodes(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.SupplierRepossitories.GetAll();
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F04_SupplierCode.ToUpper().Contains(code.ToUpper()));
            var resultLst = result.ToList().Select(s => new SupplierItem
            {
                F04_SupplierCode = s.F04_SupplierCode,
                F04_SupplierName = s.F04_SupplierName
            });
            // Sort and paging
            var itemCount = resultLst.Count();
            OrderByAndPaging(ref result, gridSettings);
            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<SupplierItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<SupplierItem>>(resultModel, true);
            //return result;
        }

        #region Properties

        /// <summary>
        ///     Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Domain handles common businesses.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        #endregion
    }
}