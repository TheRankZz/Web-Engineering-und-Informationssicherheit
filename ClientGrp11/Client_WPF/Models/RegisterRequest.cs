using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    class RegisterRequest
    {
        public string salt_masterkey;

        public string pubkey_user;

        public string privkey_user_enc;
    }
}
