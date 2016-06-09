using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    public class LoginResponse
    {
        public string salt_masterkey { get; set; }

        public string privkey_enc { get; set; }

        public string pubkey { get; set; }
    }
}