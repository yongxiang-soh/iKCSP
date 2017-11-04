using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.Material
{
    public class MaterialSearchViewModel
    {
        [DisplayName("Material Code")]
        [StringLength(12)]
        public string MaterialCode { get; set; }
        public Grid Grid { get; set; }
    }
}