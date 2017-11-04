using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KCSG.Web.Validators
{
    public class GreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        #region Properties

        /// <summary>
        /// Number which represents as a milestone for value comparision.
        /// </summary>
        private readonly double _milestone;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize attribute with milestone as byte type.
        /// </summary>
        /// <param name="milestone"></param>
        public GreaterThanAttribute(byte milestone)
        {
            _milestone = Convert.ToDouble(milestone);
        }

        /// <summary>
        /// Initialize attribute with milestone as byte int.
        /// </summary>
        /// <param name="milestone"></param>
        public GreaterThanAttribute(int milestone)
        {
            _milestone = Convert.ToDouble(milestone);
        }

        /// <summary>
        /// Initialize attribute with milestone as byte float.
        /// </summary>
        /// <param name="milestone"></param>
        public GreaterThanAttribute(float milestone)
        {
            _milestone = Convert.ToDouble(milestone);
        }

        /// <summary>
        /// Initialize attribute with milestone as byte type.
        /// </summary>
        /// <param name="milestone"></param>
        public GreaterThanAttribute(double milestone)
        {
            _milestone = milestone;
        }


        #endregion

        #region Methods

        /// <summary>
        /// This function is for validating input value and generate validation error messages as available.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // No value is passed to validation context. 
            // Treat the validation is valid because it is not required.
            if (value == null)
                return ValidationResult.Success;
            
            // Convert the value to double.
            var analyzedInput = Convert.ToDouble(value);
            
            // Input doesn't meet the comparision requirement.
            if (analyzedInput <= _milestone)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }

        /// <summary>
        /// This function is for validating input value and generate validation error messages as available.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            // Initialize validation rule attribute which should be generated on client-side.
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "greater"
            };

            modelClientValidationRule.ValidationParameters["milestone"] = _milestone;
            yield return modelClientValidationRule;
        }

        #endregion
    }
}