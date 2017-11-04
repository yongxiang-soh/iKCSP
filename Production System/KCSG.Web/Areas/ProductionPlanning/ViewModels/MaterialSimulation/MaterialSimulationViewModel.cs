using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation
{
    public class MaterialSimulationViewModel
    {
        
        [DisplayName(@"From")]
        [Remote("CheckDate", "MaterialSimulation", HttpMethod = "POST",ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName ="MSG32" )]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string From { get; set; }
        
        [DisplayName(@"To")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckDate", "MaterialSimulation", HttpMethod = "POST", AdditionalFields = "From")]
        public string To { get; set; }

        [DisplayName(@"Include inventory under retrieval")]
        public Constants.Choice InventoryUnderRetrieval { get; set; }
        [DisplayName(@"Include accepted material only")]
        public Constants.Choice AcceptedMaterialOnly { get; set; }
        [DisplayName(@"Include Material used in other commands")]
        public Constants.Choice MaterialUsedInOtherCommands { get; set; }
        [DisplayName(@"Select Simulation Type")]
        public Constants.SimulationType SimulationType { get; set; }
        public Grid Grid     { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName(@"Select Material")]
        public string SelectMaterial { get; set; }

        public bool IsPlan { get; set; }

    }
}