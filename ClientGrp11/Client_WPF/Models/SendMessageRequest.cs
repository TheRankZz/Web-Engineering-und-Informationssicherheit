﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    [Serializable]
    class SendMessageRequest
    {
        public Message envelope;

        public string recipient;

        public string timestamp;

        public string sig_service;
    }

}
