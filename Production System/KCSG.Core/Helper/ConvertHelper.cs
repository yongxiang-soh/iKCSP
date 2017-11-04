using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KCSG.Core.Helper
{
  public  class ConvertHelper
    {
       
        public static byte[] ToByteArray(String hexString)
        {
            hexString = hexString.Trim();

            int NumberChars = hexString.Length;

            if (hexString.IndexOf("0x") == 0)
            {
                hexString = hexString.Substring(2, NumberChars - 2);
                NumberChars -= 2;
            }

            if (NumberChars % 2 > 0)
            {
                hexString = "0" + hexString;
                NumberChars += 1;
            }

            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static byte[] ToByteArray(object data)
        {
            return (byte[])data;
        }

        public static string ToHexString(byte[] data, bool showPrefix)
        {
            string hexString = string.Empty;

            if (data == null)
            {
                hexString = "0";
            }
            else
            {
                hexString = BitConverter.ToString(data).Replace("-", "");
            }

            if (showPrefix)
            {
                hexString = "0x" + hexString;
            }

            return hexString;
        }

        public static string ToHexString(byte[] data)
        {
            return ToHexString(data, true);
        }

        public static decimal ToDecimal(object data)
        {
            return ToDecimal(data, 0);
        }

        public static decimal ToDecimal(object data, decimal defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else
            {
                Regex pattern = new Regex(@"^[-+]?([\d]+[.]?(\d)*|[\d]*[.]?(\d)+)$");

                if (pattern.IsMatch(data.ToString().Replace(",", "")))
                {
                    return Convert.ToDecimal(data.ToString().Replace(",", ""));
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static double ToDouble(object data)
        {
            return ToDouble(data, 0.0);
        }

        public static double ToDouble(object data, double defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else
            {
                Regex pattern = new Regex(@"^[-+]?([\d]+[.]?(\d)*|[\d]*[.]?(\d)+)$");

                if (pattern.IsMatch(data.ToString().Replace(",", "")))
                {
                    return Convert.ToDouble(data.ToString().Replace(",", ""));
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static int ToInteger(object data)
        {
            return ToInteger(data, 0).Value;
        }

        public static int? ToInteger(object data, int? defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else
            {
                Regex pattern = new Regex(@"^[-+]?([\d]+[.]?(\d)*|[\d]*[.]?(\d)+)$");

                if (pattern.IsMatch(data.ToString().Replace(",", "")))
                {
                    return Convert.ToInt32(ConvertHelper.ToDecimal(data));
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static long ToLong(object data)
        {
            return ToLong(data, 0);
        }

        public static long ToLong(object data, long defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else
            {
                Regex pattern = new Regex(@"^[-+]?([\d]+[.]?(\d)*|[\d]*[.]?(\d)+)$");

                if (pattern.IsMatch(data.ToString().Replace(",", "")))
                {
                    return Convert.ToInt64(ConvertHelper.ToDecimal(data));
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static DateTime ToDateTime(object data)
        {
            return ToDateTime(data, DateTime.MinValue);
        }

        public static DateTime ToDateTime(object data, DateTime defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else if (string.IsNullOrEmpty(data.ToString()))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(data);
                }
                catch
                {
                    return new DateTime(1753, 1, 1, 0, 0, 1);
                }

            }
        }

        /// <summary>
        /// Convert To DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ConvertToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }
            return (DateTime?)null;
        }

        public static DateTime ConvertToDateTimeFull(string strDateTime)
        {
            DateTime dtFinaldate;
            string sDateTime;

            try
            {
                CultureInfo cultureinfo = new CultureInfo("en-US");
                dtFinaldate = DateTime.ParseExact(strDateTime, "dd/MM/yyyy", cultureinfo); //Convert.ToDateTime(strDateTime);
            }
            catch (Exception)
            {
                string[] sDate = strDateTime.Split('/');
                string[] sTime = sDate[2].Split(' ');
                sDateTime = sDate[1] + '/' + sDate[0] + '/' + sDate[2];

                CultureInfo cultureinfo = new CultureInfo("en-US");
                dtFinaldate = DateTime.ParseExact(sDateTime, "dd/MM/yyyy", cultureinfo); //Convert.ToDateTime(strDateTime);
            }

            return dtFinaldate;
        }

        public static DateTime ConvertToDateTimeFullHourMinute(string strDateTime)
        {
            DateTime dtFinaldate;
            string sDateTime;

            string[] formats = new string[] { "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy h:m tt" };

            try
            {
                CultureInfo cultureinfo = new CultureInfo("en-US");
                dtFinaldate = DateTime.ParseExact(strDateTime, "dd/MM/yyyy hh:mm tt", cultureinfo); //Convert.ToDateTime(strDateTime);
            }
            catch (Exception)
            {
                string[] sDate = strDateTime.Split('/');
                string[] sTime = sDate[2].Split(' ');
                sDateTime = sDate[1] + '/' + sDate[0] + '/' + sDate[2];

                CultureInfo cultureinfo = new CultureInfo("en-US");
                dtFinaldate = DateTime.ParseExact(sDateTime, "dd/MM/yyyy hh:mm tt", cultureinfo); //Convert.ToDateTime(strDateTime);
            }

            return dtFinaldate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string RevertDateTimes(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    string[] var = date.Split('/');
                    if (var.Length != 3)
                    {
                        return null;
                    }
                    var result = var[1] + "/" + var[0] + "/" + var[2];
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ToBoolean(object data)
        {
            return ToBoolean(data, false);
        }

        public static bool ToBoolean(object data, bool defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else
            {
                Regex pattern = new Regex(@"^(?i)(True|False|1|0)$");

                if (pattern.IsMatch(data.ToString()))
                {
                    return Convert.ToBoolean(data.ToString().Replace("1", "True").Replace("0", "False"));
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static string ToUniqueIdentifier(object data)
        {
            string stringFormat = "^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$";

            if (data == DBNull.Value)
            {
                return Guid.Empty.ToString();
            }
            else if (data == null)
            {
                return Guid.Empty.ToString();
            }
            else
            {
                Match match = Regex.Match(data.ToString(), stringFormat);

                if (!match.Success)
                {
                    return Guid.Empty.ToString();
                }
                else
                {
                    return data.ToString();
                }
            }
        }

        public static string ToString(object data)
        {
            return ToString(data, string.Empty);
        }

        public static string ToString(object data, string defaultValue)
        {
            if (data == DBNull.Value)
            {
                return defaultValue;
            }
            else if (data == null)
            {
                return defaultValue;
            }
            else if (string.IsNullOrEmpty(data.ToString()))
            {
                return defaultValue;
            }
            else
            {
                return data.ToString();
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}

    