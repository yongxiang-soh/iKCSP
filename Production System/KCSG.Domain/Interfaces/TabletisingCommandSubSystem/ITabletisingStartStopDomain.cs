using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.TabletisingCommondSubSystem;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.TabletisingCommandSubSystem
{
    public interface ITabletisingStartStopDomain
    {
        ResponseResult<GridResponse<TabletisingStartStopControlItem>> SearchCriteria(GridSettings gridSettings,string terminalNo);
        ResponseResult<GridResponse<TabletisingStarStopSelectItem>> Selected(string cmdno, string lotno, GridSettings gridSettings);
        bool ContainerSet(string cmdNo, string lotNo);
        ResponseResult TimeJob(string cmdNo, string productCode, string lotNo, string lowerLotNo);
        bool CheckedStatus(string cmdNo, string lotNo);
        bool Start(string commandNo, string lotNo, string preProductCode,string productCode);
        bool End(EndTabletisingItem model);
        bool ValidationForEndButton(string cmdNo);
        //ResponseResult Create(string kneadingNo, string lotNo);

    }
}
