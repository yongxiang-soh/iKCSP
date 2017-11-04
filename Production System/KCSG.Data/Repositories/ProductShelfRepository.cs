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
    public class ProductShelfRepository : RepositoryBase<TX57_PdtShf>
    {
        public ProductShelfRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertProductShelf(string palletNo, DateTime restorageDate, string outFlg, int updateCount)
        {
            var productShelf = new TX57_PdtShf();
            productShelf.F57_PalletNo = palletNo;
            productShelf.F57_ReStorageDate = restorageDate;
            productShelf.F57_OutFlg = outFlg;
            productShelf.F57_StorageDate = DateTime.Now;
            productShelf.F57_AddDate = DateTime.Now;
            productShelf.F57_UpdateDate = DateTime.Now;
            productShelf.F57_UpdateCount = updateCount;
            Update(productShelf);
        }

        public void InsertProductShelf(string palletNo, string outFlg, int updateCount)
        {
            if (!string.IsNullOrEmpty(palletNo))
            {
                var pdtShf = Get(i => i.F57_PalletNo.Trim().Equals(palletNo.Trim()));
                if (pdtShf == null)
                {
                    var productShelf = new TX57_PdtShf();
                    productShelf.F57_PalletNo = palletNo;
                    productShelf.F57_OutFlg = outFlg;
                    productShelf.F57_StorageDate = DateTime.Now;
                    productShelf.F57_AddDate = DateTime.Now;
                    productShelf.F57_UpdateDate = DateTime.Now;
                    productShelf.F57_UpdateCount = updateCount;
                    Add(productShelf);
                }
            }
        }
    }
}