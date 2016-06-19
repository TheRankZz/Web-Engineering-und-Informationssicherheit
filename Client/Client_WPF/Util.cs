using System;
using System.Collections.Generic;
using System.Linq;
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
            return enc.GetBytes(str);
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
    }
}
