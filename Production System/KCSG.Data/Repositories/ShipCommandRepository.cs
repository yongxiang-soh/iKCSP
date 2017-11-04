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
   public class ShipCommandRepository :RepositoryBase<TX45_ShipCommand>
    {
        public ShipCommandRepository(IKCSGDbContext context) : base(context)
        {
        }

       public void InsertShipCommand(string shipCommandNo,double shipAmount,double shippedAmount)
       {
           var shipCommand = new TX45_ShipCommand();
           shipCommand.F45_ShipCommandNo = shipCommandNo;
           shipCommand.F45_ShipDate = DateTime.Now;
           shipCommand.F45_ShipAmount = shipAmount;
           shipCommand.F45_ShippedAmount = shippedAmount;
           shipCommand.F45_AddDate = DateTime.Now;
           shipCommand.F45_UpdateDate = DateTime.Now;
           shipCommand.F45_UpdateCount = 0;

           Add(shipCommand);
       }
    }
}
