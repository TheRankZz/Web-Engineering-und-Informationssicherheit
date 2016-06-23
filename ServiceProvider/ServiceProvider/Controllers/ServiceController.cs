using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServiceProvider.Models;
using System.Web.Http.Description;

namespace ServiceProvider.Controllers
{
    public class ServiceController : ApiController, Models.IService 
    {
       
        static Dictionary<string, User> users =
            new Dictionary<string, User>();


        public ServiceController()
        {
        }


        [Route("{user}")]
        [HttpGet]
        [ResponseType(typeof(User))]
        public HttpResponseMessage login(string user)
        {
            if(users.ContainsKey(user))
            {
                User u = users[user];

                LoginResponse response = new LoginResponse();
                response.privkey_enc = u.privkey_enc;
                response.pubkey = u.pubkey;
                response.salt_masterkey = u.salt_masterkey;

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("{user}")]
        [HttpPost]
        public HttpResponseMessage register(string user, [FromBody]RegisterRequest request)
        {
            if(!users.ContainsKey(user))
            {
                User newuser = new Models.User();
                newuser.username = user;
                newuser.privkey_enc = request.privkey_enc;
                newuser.salt_masterkey = request.salt_masterkey;
                newuser.pubkey = request.pubkey;

                users.Add(user, newuser);

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("{user}/publickey")]
        [HttpGet]
        [ResponseType(typeof(PubkeyResponse))]
        public HttpResponseMessage getPubkey(string user)
        {
            if (users.ContainsKey(user))
            {
                User u = users[user];

                PubkeyResponse response = new PubkeyResponse();
                response.pubkey = u.pubkey;

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("{user}/message")]
        [HttpPatch]
        [ResponseType(typeof(Message))]
        public HttpResponseMessage getMessage(string user, [FromBody]GetMessageRequest request)
        {
            if (users.ContainsKey(user))
            {
                User u = users[user];

                if (u.message != null)
                {
                    //Timestamp prüfen
                    Double unixtimeDouble = Convert.ToDouble(request.timestamp);
                    DateTime timestamp = Util.Converter.UnixTimeStampToDateTime(unixtimeDouble)
                        .AddMinutes(5);

                    if (DateTime.Now > timestamp)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    //TODO: request.dig_sig
                    if(!SecurityLogic.verfiyGetMessageRequest(request, u.username, u.pubkey))
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    if (u.message != null)
                    {
                        Message msg = u.message;
                        u.deleteMessage();

                        return Request.CreateResponse(HttpStatusCode.OK, msg);
                    }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("{user}/message")]
        [HttpPost]
        public HttpResponseMessage sendMessage(string user, [FromBody]SendMessageRequest request)
        {
            if (users.ContainsKey(user))
            {
                User u = users[user];

                if (users.ContainsKey(request.receiver))
                {
                    //Timestamp prüfen
                    Double unixtimeDouble = Convert.ToDouble(request.timestamp);
                    DateTime timestamp = Util.Converter.UnixTimeStampToDateTime(unixtimeDouble)
                        .AddMinutes(5);
                    if(DateTime.Now > timestamp)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    
                    //TODO: request.sig_service
                    if(!SecurityLogic.verfiyOuterEnvelope(request, u.pubkey))
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    User sender = users[user];
                    User receiver = users[request.receiver];

                    Message msg = new Message();
                    msg.cipher = request.inner_envelope.cipher;
                    msg.iv = request.inner_envelope.iv;
                    msg.key_recipient_enc = request.inner_envelope.key_recipient_enc;
                    msg.sig_recipient = request.inner_envelope.sig_recipient;
                    msg.sender = user;

                    receiver.message = msg;

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("{user}")]
        [HttpDelete]
        public HttpResponseMessage deleteUser(string user)
        {
            if(users.ContainsKey(user))
            {
                users.Remove(user);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
