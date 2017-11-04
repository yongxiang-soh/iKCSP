using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class RetrievalOfPreProductViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("Tabletising Line")]
        public string TabletisingLine { get; set; }

        [DisplayName("Tabletising Name")]
        public string TabletisingLineName { get; set; }

        public Grid Grid { get; set; }
    }
}