using System.Linq;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class PrePdtMkpRepository : RepositoryBase<TM02_PrePdtMkp>
    {
        public PrePdtMkpRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateForSavePreProduct(TM03_PreProduct preProduct,bool isCreate)
        {
            var lstPreProductMaterial = GetAll()
                .Where(i => i.F02_PreProductCode.Trim().Equals(preProduct.F03_PreProductCode.Trim()));
            foreach (var tm02PrePdtMkp in lstPreProductMaterial)
            {
                tm02PrePdtMkp.F02_IsDraft = false;
                if (isCreate)
                {
                    tm02PrePdtMkp.F02_AddDate = preProduct.F03_AddDate;
                }
                else
                {
                    tm02PrePdtMkp.F02_UpdateDate = preProduct.F03_UpdateDate;
                }
                Update(tm02PrePdtMkp);
            }
        }

      
    }
}
