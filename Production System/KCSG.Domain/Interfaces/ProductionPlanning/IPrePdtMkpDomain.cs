using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IPrePdtMkpDomain
    {
        PrePdtMkpItem GetById(string preProdCode, string matCode);
        ResponseResult<GridResponse<PrePdtMkpMatItem>> SearchByPreProductCode(string preProductCode, GridSettings gridSettings);
        void Create(TM02_PrePdtMkp prePdtMkp);
        void Update(TM02_PrePdtMkp prePdtMkp);
        List<TM02_PrePdtMkp> GetAllByPreProduct(string preProdCode);
        void Delete(string preProductCode, string materialCode, string thrawSeqNo);
        void Delete(string preProductCode);
        bool CheckUnique(string preProdCode, string matCode);
        ResponseResult CreateOrUpdate(PrePdtMkpItem model);
        bool PotSeqNo(string f02PreProductCode, string f02_potseqno);
        int CountByPreproductCode(string preProdCode);
    }
}
