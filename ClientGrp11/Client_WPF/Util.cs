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
            var enc = new System.Text.UTF8Encoding();
            var result = enc.GetBytes(str);
            return result;
        }


        public static string GetString(byte[] bytes)
        {
            var enc = new System.Text.UTF8Encoding();
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

        public static void Log(int statusCode)
        {
            string msg = "HTTP-Statuscode: " + statusCode;
            Log(msg);
        }

        public static void Log(string logMessage)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nFehler: ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.WriteLine("{0}", logMessage);
                w.WriteLine("-------------------------------");
            }
        }

    }
}
