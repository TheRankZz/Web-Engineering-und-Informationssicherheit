﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    class Message
    {
        public string sender { get; set; }

        public string cipher { get; set; }

        public string iv { get; set; }

        public string key_recipient_enc { get; set; }

        public string sig_recipient { get; set; }
    }
}
