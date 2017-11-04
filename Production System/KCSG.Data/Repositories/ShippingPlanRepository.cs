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
    public class ShippingPlanRepository : RepositoryBase<TX44_ShippingPlan>
    {
        public ShippingPlanRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateShippedAmount(string shipCommandNo, double shippedAmount)
        {
            if (shipCommandNo != "")
            {
                var tx44 = GetById(shipCommandNo);
                tx44.F44_ShippedAmount += shippedAmount;
                tx44.F44_UpdateDate = DateTime.Now;
                Update(tx44);
            }            
        }
    }
}
