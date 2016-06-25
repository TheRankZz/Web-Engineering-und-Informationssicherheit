using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ServiceProvider.Models
{
    [Serializable]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string sender { get; set; }

        public string cipher { get; set; }

        public string iv { get; set; }

        public string key_recipient_enc { get; set; }

        public string sig_recipient { get; set; }
    }
}