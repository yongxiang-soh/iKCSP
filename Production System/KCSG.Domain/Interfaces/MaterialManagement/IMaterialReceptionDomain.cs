using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IMaterialReceptionDomain
    {
        MaterialReceptionItem GetById(string id);
        MaterialReceptionItem GetByMaterialReception(string prcOrdNo, string prtDvrNo);

        /// <summary>
        /// Find material reception by using primary keys.
        /// </summary>
        /// <param name="prcOrdNo"></param>
        /// <param name="prtDvrNo"></param>
        /// <returns></returns>
        Task<MaterialReceptionItem> SearchByPrimaryKeys(string prcOrdNo, string prtDvrNo);

        void Create(MaterialReceptionItem material);
        void Update(MaterialReceptionItem material);
        bool Delete(string prcOrdNo,string prtDvrNo,string materialCode );
        bool CheckUnique(string prcOrdNo, string prtDvrNo);
        bool CheckPrtDvrNo(string prtDvrNo);
        ResponseResult CreateOrUpdate(MaterialReceptionItem model);
        ResponseResult<GridResponse<MaterialReceptionItem>> SearchCriteria(string prcOrdNo, string parDelivery, GridSettings gridSettings);
       
    }
}
