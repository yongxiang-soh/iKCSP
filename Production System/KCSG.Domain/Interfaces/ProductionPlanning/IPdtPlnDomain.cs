using System;
using KCSG.Core.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IPdtPlnDomain
    {
        PdtPlnItem GetById(DateTime date,string preProdCode);
        void Create(PdtPlnItem pdtPln);
        void Update(PdtPlnItem pdtPln);
        bool Delete(DateTime date, string code);
        bool CheckUnique(string preProdCode, DateTime prodDate);
        ResponseResult CreateOrUpdate(PdtPlnItem model);
        ResponseResult<GridResponse<PdtPlnItem>> SearchCriteria(string date, Enum line, GridSettings gridSettings);
        bool CheckUnique3Reco(DateTime dateTime, string kndLine);
    }
}
