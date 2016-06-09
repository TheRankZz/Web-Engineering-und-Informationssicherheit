using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider.Models
{
    interface IService
    {
        HttpResponseMessage register(String user, RegisterRequest request);
        HttpResponseMessage login(string user);
        HttpResponseMessage getPubkey(string user);
        HttpResponseMessage sendMessage(string user, SendMessageRequest request);
        HttpResponseMessage getMessage(string user, GetMessageRequest request);
        HttpResponseMessage deleteUser(string user);
    }
}
