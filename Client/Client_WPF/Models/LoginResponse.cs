using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class LoginResponse
    {
        public string salt_masterkey;

        public string privkey_enc;

        public string pubkey;
    }
}
