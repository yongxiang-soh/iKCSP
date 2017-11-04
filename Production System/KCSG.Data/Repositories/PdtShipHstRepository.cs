using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class PdtShipHstRepository : RepositoryBase<TH70_PdtShipHst>
    {
        public PdtShipHstRepository(IKCSGDbContext context) : base(context)
        {
        }

        public void InsertProductShipHistory(string shipCommandNo, string palletNo, string preProductLotNo,
            string productCode, string shelfNo, double shippedAmount)
        {
            var productShipHistory = new TH70_PdtShipHst();
            productShipHistory.F70_ShipCommandNo = shipCommandNo;
            productShipHistory.F70_PalletNo = palletNo;
            productShipHistory.F70_PrePdtLotNo = preProductLotNo;
            productShipHistory.F70_ProductCode = productCode;
            productShipHistory.F70_ShelfNo = shelfNo;
            productShipHistory.F70_ProductLotNo = preProductLotNo;
            productShipHistory.F70_ShippedAmount = shippedAmount;
            productShipHistory.F70_AddDate = DateTime.Now;
            productShipHistory.F70_UpdateDate = DateTime.Now;
            productShipHistory.F70_UpdateCount = 0;

            Add(productShipHistory);
        }
    }
}