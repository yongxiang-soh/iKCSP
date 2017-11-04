using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Core.Controls.CustomerValidate
{
    public class ExtRange : RangeAttribute
    {
        private object _minimum;
        private object _maximum;
        

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            var result = Convert.ToDouble(_minimum) > Convert.ToDouble(value.ToString().Trim(','));
            return result ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }

        public override bool IsValid(object value)
        {
            return false;
        }

        public ExtRange(int minimum, int maximum) : base(minimum, maximum)
        {
            _minimum = minimum;
         _maximum = maximum;
        }

        public ExtRange(double minimum, double maximum) : base(minimum, maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public ExtRange(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }
    }
}
