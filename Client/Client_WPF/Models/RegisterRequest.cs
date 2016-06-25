using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class RegisterRequest
    {
        public string salt_masterkey;

        public string pubkey;

        public string privkey_enc;
    }
}
