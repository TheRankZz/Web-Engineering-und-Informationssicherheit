using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    public class LoginResponse
    {
        public string salt_masterkey;

        public string privkey_enc;
        public string pubkey;
    }
}