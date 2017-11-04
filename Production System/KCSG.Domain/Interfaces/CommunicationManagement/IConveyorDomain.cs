using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces
{
    public interface IConveyorDomain
    {
        ResponseResult<GridResponse<ConveyorItem>> Search(int communication, GridSettings gridSettings);
        TM05_Conveyor GetConveyor(string code);
        void UpdateConveyor(TM05_Conveyor model);
    }
}