using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.Communication.ViewModels
{
    public class DetailInformationViewModel
    {

         //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Date { get; set; }
       
        [StringLength(10)]
        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Terminal { get; set; }
        public string EditLog { get; set; }
        public Grid CommonQueue { get; set; }
        public Grid CommonHistory { get; set; }
    }
}