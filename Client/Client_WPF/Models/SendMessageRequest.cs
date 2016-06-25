using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    [Serializable]
    class SendMessageRequest
    {
        public Message inner_envelope;

        public string receiver;

        public string timestamp;

        public string sig_service;
    }
}
