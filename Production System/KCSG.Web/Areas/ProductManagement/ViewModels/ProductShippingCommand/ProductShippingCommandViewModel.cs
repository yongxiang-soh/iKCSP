using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingCommand
{
    public class ProductShippingCommandViewModel
    {
        /// <summary>
        /// Grid which displays shipping commands list.
        /// </summary>
        [BindNever]
        public Grid ProductShippingCommandGrid { get; set; }

        /// <summary>
        /// Grid which displays shipping commands list.
        /// </summary>
        [BindNever]
        public Grid ProductShippingCommandDetailsGrid { get; set; }

        public double Total { get; set; }
    }
}