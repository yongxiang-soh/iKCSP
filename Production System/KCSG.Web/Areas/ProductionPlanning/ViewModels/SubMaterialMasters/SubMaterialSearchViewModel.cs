using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.SubMaterialMasters
{
    public class SubMaterialSearchViewModel
    {
        [StringLength(12)]
        [DisplayName("Sup. Material Code")]
        public string MaterialCode { get; set; }
        public Grid Grid { get; set; }
    }
}