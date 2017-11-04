using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfOutOfSpecPreProduct
{
    public class RetrievalOfOutOfSpecPreProductViewModel
    {
        public Grid Grid { get; set; }
        [DisplayName("Shelf Status")]
        public int  ShelfStatus { get; set; }
    }
}