using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    public class User
    {
        public string username;

        public string salt_masterkey;

        public string privkey_enc;

        public string pubkey;

        public Message message;

        public void deleteMessage()
        {
            this.message = null;
        }
    }
}