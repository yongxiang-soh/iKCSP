using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Infrastructure
{
    public class SubmitButtonSelector : ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            // Try to find out if the name exists in the data sent from form
            var value = controllerContext.Controller.ValueProvider.GetValue(Name);
            if (value != null)
            {
                return true;
            }
            return false;

        }
    }
}