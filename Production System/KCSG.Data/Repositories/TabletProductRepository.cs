using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class TabletProductRepository : RepositoryBase<TX56_TbtPdt>
    {
        public TabletProductRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateTabletProduct(TX56_TbtPdt tabletProduct, string certificationFlag, DateTime certificationDate, double storageAmt, string status, DateTime shipDate)
        {
            tabletProduct.F56_CertificationFlag = certificationFlag;
            tabletProduct.F56_CertificationDate = certificationDate;
            tabletProduct.F56_StorageAmt = storageAmt;
            tabletProduct.F56_Status = status;
            tabletProduct.F56_ShipDate = shipDate;
            tabletProduct.F56_UpdateDate = DateTime.Now;

            Update(tabletProduct);
        }
        public void UpdateTabletProduct(TX56_TbtPdt tabletProduct, string certificationFlag,double storageAmt)
        {
            tabletProduct.F56_CertificationFlag = certificationFlag;
            tabletProduct.F56_StorageAmt = storageAmt;
            tabletProduct.F56_UpdateDate = DateTime.Now;

            Update(tabletProduct);
        }
    }
}
