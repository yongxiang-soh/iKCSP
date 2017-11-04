using System.Text;

namespace KCSG.jsGrid.MVC
{
    /// <summary>
    /// validate is a string as validate rule name or validation function or a validation configuration object or an array of validation configuration objects. Read more details about validation in the Validation section.
    /// </summary>
    public class Validator
    {
        private readonly string _validator;
        private readonly bool _isBuiltInValidator;
        private string _message;
        private bool _isMessageFunction;

        private string _param;
        private bool _paramIsText;

        /// <summary>
        /// validate is a string as validate rule name or validation function or a validation configuration object or an array of validation configuration objects. Read more details about validation in the Validation section.
        /// </summary>
        /// <param name="validator">built-in or custom validator</param>
        /// <param name="isBuiltIn">true if validator is built-in refe jsGrid.MVC.Enums.BuiltInValidator</param>
        public Validator(string validator, bool isBuiltIn)
        {
            _validator = validator;
            _isBuiltInValidator = isBuiltIn;
        }

        /// <summary>
        /// validation message or a function(value, item) returning validation message
        /// </summary>
        /// <param name="message">message string or message function</param>
        /// <param name="isFunction">true if message is function</param>
        /// <returns></returns>
        public Validator SetMessage(string message, bool isFunction)
        {
            _message = message;
            _isMessageFunction = isFunction;
            return this;
        }

        /// <summary>
        /// param a plain object with parameters to be passed to validation function
        /// </summary>
        /// <param name="param">param used for validate</param>
        /// <param name="isText">true for Pattern validate, other is false</param>
        /// <returns></returns>
        public Validator SetParam(string param, bool isText)
        {
            _param = param;
            _paramIsText = isText;
            return this;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_message) && string.IsNullOrEmpty(_param))
            {
                if (_isBuiltInValidator)
                {
                    return string.Format("\"{0}\",\r\n", _validator);
                }
                else
                {
                    return string.Format("{0},\r\n", _validator);
                }
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("{");
                if (_isBuiltInValidator)
                {
                    stringBuilder.AppendLine(string.Format("validator: \"{0}\",", _validator));
                }
                else
                {
                    stringBuilder.AppendLine(string.Format("validator: {0},", _validator));
                }

                if (!string.IsNullOrEmpty(_message))
                {
                    if (!_isMessageFunction)
                    {
                        stringBuilder.AppendLine(string.Format("message: \"{0}\",", _message));
                    }
                    else
                    {
                        stringBuilder.AppendLine(string.Format("message: {0},", _message));
                    }
                }

                if (!string.IsNullOrEmpty(_param))
                {
                    if (_paramIsText)
                    {
                        stringBuilder.AppendLine(string.Format("param: \"{0}\",", _param));
                    }
                    else
                    {
                        stringBuilder.AppendLine(string.Format("param: {0},", _param));
                    }
                }
                stringBuilder.AppendLine("},");

                return stringBuilder.ToString();
            }
        }
    }
}
