using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KCSG.Web.Validators
{
    public class MinAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _minValue;

        public MinAttribute(int minValue)
        {
            this._minValue = minValue;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();

            rule.ErrorMessage = ErrorMessageString;

            rule.ValidationType = "min";
            rule.ValidationParameters.Add("min", _minValue);
            yield return rule;
        }

        public override bool IsValid(object value)
        {
            return (int)value < _minValue;
        }
    }
}