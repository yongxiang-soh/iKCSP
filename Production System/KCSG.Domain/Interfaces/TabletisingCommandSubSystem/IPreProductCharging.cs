using KCSG.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Interfaces.TabletisingCommandSubSystem
{
    public interface IPreProductCharging
    {
        //PreProductChargingItem GetPreProductChargingItem(string kndcmdno, string prepdtlotno, string tabletline);
        PreProductChargingItem GetPreProductChargingItemByContainerCode(string containerCode, string tabletline);
        void UpdatePreProductCharging(PreProductChargingItem model);

        IQueryable<TX49_PrePdtShfStk> GetTX49_PrePdtShfStkByCode(string code);
        TX41_TbtCmd GetTX41_TbtCmdByCmd(string cmdNo);
    }
}
