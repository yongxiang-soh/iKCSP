using System.Collections.Generic;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class StockTakingOfProductConfirmViewModel
    {
        public IList<StockTakingOfProductConfirmItem> Items { get; set; }
    }
}