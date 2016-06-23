using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    class User
    {
        public string Username;

        public byte[] salt_masterkey;

        public string publickey;

        public string privatekey;
    }
}
