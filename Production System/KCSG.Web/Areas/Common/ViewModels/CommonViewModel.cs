using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.Common.ViewModels
{
    public class CommonViewModel
    {
        public string ModelType { get; set; }
        [DisplayName("Keyword")]
        [StringLength(15)]
        public string KeyWord { get; set; }

        public string Code { get; set; }
        public Grid Grid { get; set; }

        public string ListLabel { get; set; }
    }
}