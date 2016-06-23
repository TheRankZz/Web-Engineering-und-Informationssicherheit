using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    [Serializable]
    class Message
    {
        public string sender;

        public string cipher;

        public string iv;

        public string key_recipient_enc;

        public string sig_recipient;
    }
}
