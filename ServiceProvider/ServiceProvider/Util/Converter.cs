using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Util
{
    public static class Converter
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static byte[] GetBytes(string str)
        {
            System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
            var result = enc.GetBytes(str);
            return result;
        }

        public static string GetString(byte[] bytes)
        {
            System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
            return enc.GetString(bytes);
        }

        public static string StringToBase64String(string str)
        {
            return Convert.ToBase64String(GetBytes(str));
        }

        public static string Base64StringToString(string base64)
        {
            return GetString(Convert.FromBase64String(base64));
        }
    }
}