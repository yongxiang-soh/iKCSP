using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct
{
    #region Hantt27
    public class PreProductSearchViewModel
    {
        [StringLength(12)]
        [Display(Name = "Pre-Product Code")]
        public string PreProductCode { get; set; }

        public Grid Grid { get; set; }
    }
    #endregion
}