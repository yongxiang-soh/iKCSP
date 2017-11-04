using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
   public interface IFontStorageOfSupplierPalletDomain
   {
        ResponseResult Storage(string suppliCode, int storageQuantity, string terminalNo);
       List<FirstCommunicationResponse> CommutionC1(string terminal);
   }
}
