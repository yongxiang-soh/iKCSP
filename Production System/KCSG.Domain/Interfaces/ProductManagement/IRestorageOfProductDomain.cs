using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Domains.ProductManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.Core.Models;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IRestorageOfProductDomain
    {
        List<RestorageOfProductViewItem> GetListRestorageOfProduct(string palletNo);
        void DeAssignProduct(string palletNo);
        bool StoreProduct(RestoreProductItem item, string terminalNo);
        IList<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo, RestoreProductItem item);
    }
}
