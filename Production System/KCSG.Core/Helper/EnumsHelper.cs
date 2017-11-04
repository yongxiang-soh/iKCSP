using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using KCSG.Core.Constants;

namespace KCSG.Core.Helper
{
    public static class EnumsHelper 
    {
        public static string ToStringValue(this List<SelectListItem> selectListItems)
        {
            var stringList = selectListItems.Select(x => string.Format("{0}:{1}", x.Value, x.Text)).ToArray();
            return string.Join(";", stringList);
        }
        
        public static List<SelectListItem> GetListItemsWithDescription<T>(Enum selectedValue) where T : struct
        {
            var list =
                Enum.GetValues(typeof (T))
                    .Cast<T>().OrderBy(i=>i.ToString())
                    .Select(x => new SelectListItem { Text = GetDescription(x), Value = ((int)Enum.Parse(x.GetType(), x.ToString())).ToString(), Selected = Enum.Parse(x.GetType(), x.ToString()).Equals(selectedValue )})
                    .ToList();
            return list;
        }

        
        public static string GetDescription(object en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
       
        public static string EnumsDisplay<T>(int value) where T : struct
        {
            Type type = typeof (T);
            return ((T) Enum.Parse(type, value.ToString())).ToString();
        }

        public static string GetEnumDescription(Enum value)
        {

            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public static string GetEnumDescriptionWithDefaultValue(Enum value,string defaultValue)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());

                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                if (attributes != null &&
                    attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return defaultValue;
            }catch(Exception ex)
            {
                return defaultValue;
            }
            
        }

        public static string GetDescription<T>(int value) where T : struct
        {
            try
            {
                Type type = typeof(T);
                var enumValue = Enum.GetName(type, value);
                MemberInfo[] memInfo = type.GetMember(enumValue);

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {

                return value.ToString();
            }
         
        }
        public static IEnumerable<SelectListItem> GetListItemsWithDescription<T>() where T : struct
        {
            return GetListItemsWithDescription<T>(null);
        }
    }
}
