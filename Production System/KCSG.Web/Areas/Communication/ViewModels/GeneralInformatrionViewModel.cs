using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.Communication.ViewModels
{
    public class GeneralInformatrionViewModel
    {
        [Display(Name = @"View")]
        public string ViewSelect { get; set; }

        [Display(Name = @"Status")]
        public string Status { get; set; }

        [Display(Name = @"Status Request")]
        public string StatusRequest { get; set; }

        public DetailInformationViewModel DetailInformation { get; set; }
        public int Communcation { get; set; }
    }
}