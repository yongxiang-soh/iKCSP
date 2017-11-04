using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.TabletisingCommandSubSystem
{
    public interface IManagementOfProductLabelDomain
    {
         TM09_Product GetProductItem(string productCode);
        TX42_KndCmd GetKneadingCommand(string commandNo, string lotNo);
        //TM14_Device GetDevice(string commandNo, string lotNo, string tableLine);
        int? GetCSNo1(string commandNo, string preProductLotNo);
    }
}
