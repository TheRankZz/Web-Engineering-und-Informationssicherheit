using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF.Models
{
    [Serializable]
    class LoginResponse
    {
        public string salt_masterkey { get; set; }
        public string privkey_user_enc { get; set; }
        public string pubkey_user { get; set; }
        
    }
}
