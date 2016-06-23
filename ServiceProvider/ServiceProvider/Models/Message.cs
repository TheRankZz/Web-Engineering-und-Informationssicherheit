using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    [Serializable]
    public class Message
    {
        public string sender;

        public string cipher;

        public string iv;

        public string key_recipient_enc;

        public string sig_recipient;
    }
}