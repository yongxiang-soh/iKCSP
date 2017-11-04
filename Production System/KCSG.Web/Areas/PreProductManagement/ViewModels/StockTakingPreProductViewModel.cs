using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class StockTakingPreProductViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("Sheft No.")]
        public string ShelfNoFrom { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ShelfNoTo { get; set; }

        /// <summary>
        /// Stock-taking pre-products list.
        /// </summary>
        public Grid StockTakingPreProductGrid { get; set; }

        [DisplayName("Retrieval of Shelf No.")]
        public string RetrievalShelfNo { get; set; }

        [DisplayName("Pre-Product Code")]
        public string PreProductCode { get; set; }

        [DisplayName("Container Type")]
        public int ContainerType { get; set; }

        //Confirm with Retrieval of PreProduct Container
        [Display(Name = @"Shelf No.")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Quantity")]
        [Range(0.01,double.MaxValue,ErrorMessageResourceType =typeof (PreProductManagementResources),ErrorMessageResourceName = "MSG6")]
        public double Quantity { get; set; }

        [Display(Name = @"Pre-Product  Code")]
        public string ConfirmPreProductCode { get; set; }

        [Display(Name = @"Container Type")]
        public string ConfirmContainerType { get; set; }

        [Display(Name = @"Pre-Product Name")]
        public string PreProductName { get; set; }

        [Display(Name = @"Container No.")]
        public string ContainerNo { get; set; }

        [Display(Name = @"Lot No.")]
        public string LotNo { get; set; }

        [Display(Name = @"Container Code")]
        public string ContainerCode { get; set; }
    }
}