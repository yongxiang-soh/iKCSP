using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.Communication.ViewModels
{
    public class WeighingEquipmentViewModel
    {
        public Constants.ViewSelectedC4 ViewSelect { get; set; }
        public string Status { get; set; }
        public string Model { get; set; }
        public Constants.SelectData SelectData { get; set; }
        [DisplayName("Code No")]
        public string CodeNo { get; set; }


        public string Date { get; set; }
        public Constants.Line Line { get; set; }
        public Grid MaterialGrid { get; set; }
        public Grid PreproductGrid { get; set; }
        public Grid RetrievalGrid { get; set; }
        public Grid KneadingCommandGrid { get; set; }
        public Grid KneadingResultsGrid { get; set; }
        public string  LogEdit { get; set; }

    }
}