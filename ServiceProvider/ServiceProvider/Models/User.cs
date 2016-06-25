using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    public class User
    {
        public User()
        {
            messages = new List<Message>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string username { get; set; }

        public string salt_masterkey { get; set; }

        public string privkey_enc { get; set; }

        public string pubkey { get; set; }

        public List<Message> messages { get; set; }
    }
}