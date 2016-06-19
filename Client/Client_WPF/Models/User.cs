using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    class User
    {
        public string Username { get; set; }

        public byte[] salt_masterkey { get; set; }

        public string publickey { get; set; }

        public string privatekey { get; set; }
    }
}
