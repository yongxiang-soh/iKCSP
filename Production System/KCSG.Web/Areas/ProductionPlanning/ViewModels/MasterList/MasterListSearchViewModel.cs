using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MasterList
{
    public class MasterListSearchViewModel
    {
        /// <summary>
        /// Master list code.
        /// </summary>
        [DisplayName("Master Code")]
        [StringLength(12)]
        public string MasterCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint SearchBy { get; set; }

        /// <summary>
        /// Grid which is used for displaying materials.
        /// </summary>
        public Grid GridMaterial { get; set; }

        /// <summary>
        /// Grid which is used for displaying pre-products.
        /// </summary>
        public Grid GridPreProduct { get; set; }

        /// <summary>
        /// Grid which is used for displaying products.
        /// </summary>
        public Grid GridProduct { get; set; }
    }
}