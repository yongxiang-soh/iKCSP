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
    public class PckMtrRepository : RepositoryBase<TM11_PckMtr>
    {
        public PckMtrRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateWithProduct(TM09_Product entity, bool isCreate)
        {
            var pckMtr = GetMany(i => i.F11_ProductCode.Trim().Equals(entity.F09_PreProductCode.Trim()));
            foreach (var tm11PckMtr in pckMtr)
            {
                if (isCreate)
                {
                    tm11PckMtr.F11_AddDate = entity.F09_AddDate;
                }
                else
                {
                    tm11PckMtr.F11_UpdateDate = entity.F09_UpdateDate;
                }
                Update(tm11PckMtr);
            }
        }
    }
}
