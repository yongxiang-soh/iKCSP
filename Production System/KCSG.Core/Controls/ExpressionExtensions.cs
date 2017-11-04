using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.Build.Framework;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;


namespace KCSG.Core.CustomControls
{
    public static class ExpressionExtensions
    {
        public static bool IsRequired<TModel, TValue>(this Expression<Func<TModel, TValue>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            return memberExpression != null
                   && memberExpression.Member.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0;
        }

        public static int MaxLength<TModel, TValue>(this Expression<Func<TModel, TValue>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                return 0;
            }

            var stringLengthAttributes = memberExpression.Member.GetCustomAttributes(typeof(StringLengthAttribute), true);
            if (stringLengthAttributes.Length > 0)
            {
                var stringLengthAttribute = (StringLengthAttribute)stringLengthAttributes[0];
                return stringLengthAttribute.MaximumLength;
            }

            var maxLengthAttributes = memberExpression.Member.GetCustomAttributes(typeof(MaxLengthAttribute), true);
            if (maxLengthAttributes.Length > 0)
            {
                var stringLengthAttribute = (MaxLengthAttribute)maxLengthAttributes[0];
                return stringLengthAttribute.Length;
            }

            return 0;
        }
    }
}