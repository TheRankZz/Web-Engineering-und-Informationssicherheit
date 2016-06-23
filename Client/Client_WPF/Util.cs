using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Client_WPF
{
    class Util
    {
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

        public static string GetString(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }


        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

    }
}
