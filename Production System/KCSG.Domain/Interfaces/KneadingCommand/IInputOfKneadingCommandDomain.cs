using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.KneadingCommand;
using KCSG.Domain.Models.ProductionPlanning;

namespace KCSG.Domain.Interfaces.KneadingCommand
{
    public interface IInputOfKneadingCommandDomain
    {
        ResponseResult CreateOrUpdate(string selectedValue, int within, int lotQuantity);
        bool DeleteKneadingCommand(string deleteDate, string deleteCode);
        ResponseResult<GridResponse<PdtPlnItem>> SearchCriteria(DateTime dateCurrent,DateTime date, Enum line, GridSettings gridSettings);
        ResponseResult<GridResponse<PdtPlnItem>> SearchCriteriaSelected(string selectedValue, GridSettings gridSettings);
    
        /// <summary>
        /// This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        Task<PrintKneadingCommandItem> SearchRecordsForPrinting(string preProductCode, string commandNo);
    }
}
