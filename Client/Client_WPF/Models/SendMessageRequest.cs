using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    class SendMessageRequest
    {
        public Message inner_envelope { get; set; }

        public string receiver { get; set; }

        public string timestamp { get; set; }

        public string sig_service { get; set; }
    }
}
