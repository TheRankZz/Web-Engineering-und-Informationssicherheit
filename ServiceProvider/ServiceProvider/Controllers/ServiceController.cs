using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServiceProvider.Models;
using System.Web.Http.Description;
using ServiceProvider.DAO;

namespace ServiceProvider.Controllers
{
    public class ServiceController : ApiController, Models.IService 
    {

        public ServiceController()
        {
        }

        [Route("{user}")]
        [HttpGet]
        [ResponseType(typeof(User))]
        public HttpResponseMessage login(string user)
        {
            var u = UserDAO.Instance.findByUsername(user);
          
            if (u != null)
            {
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
            if(!UserDAO.Instance.isExists(user))
            {
                User newuser = new Models.User();
                newuser.username = user;
                newuser.privkey_enc = request.privkey_enc;
                newuser.salt_masterkey = request.salt_masterkey;
                newuser.pubkey = request.pubkey;

                if(UserDAO.Instance.insert(newuser))
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("{user}/publickey")]
        [HttpGet]
        [ResponseType(typeof(PubkeyResponse))]
        public HttpResponseMessage getPubkey(string user)
        {
            if (UserDAO.Instance.isExists(user))
            {
                User u = UserDAO.Instance.findByUsername(user);

                PubkeyResponse response = new PubkeyResponse();
                response.pubkey = u.pubkey;

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("{user}/message")]
        [HttpPatch]
        [ResponseType(typeof(MessageResponse))]
        public HttpResponseMessage getMessage(string user, [FromBody]GetMessageRequest request)
        {
            if (UserDAO.Instance.isExists(user))
            {
                User u = UserDAO.Instance.findByUsername(user);

                if (u.messages.Count != 0)
                {
                    //Timestamp prüfen
                    Double unixtimeDouble = Convert.ToDouble(request.timestamp);
                    DateTime timestamp = Util.Converter.UnixTimeStampToDateTime(unixtimeDouble)
                        .AddMinutes(5);

                    if (DateTime.Now > timestamp)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    ////TODO: request.dig_sig
                    string publickey = Util.Converter.Base64StringToString(u.pubkey);
                    if (!SecurityLogic.verfiyGetMessageRequest(request, u.username, publickey))
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    var msg = UserDAO.Instance.getFirstMessage(u);
                    if (msg != null)
                    {
                        MessageResponse msg_r = new MessageResponse();
                        msg_r.cipher = msg.cipher;
                        msg_r.iv = msg.iv;
                        msg_r.key_recipient_enc = msg.key_recipient_enc;
                        msg_r.sender = msg.sender;
                        msg_r.sig_recipient = msg.sig_recipient;

                        UserDAO.Instance.removeMessage(u, msg);

                        return Request.CreateResponse(HttpStatusCode.OK, msg_r);
                    }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("{user}/message")]
        [HttpPost]
        public HttpResponseMessage sendMessage(string user, [FromBody]SendMessageRequest request)
        {
            if (UserDAO.Instance.isExists(user))
            {
                User u = UserDAO.Instance.findByUsername(user);

                if (UserDAO.Instance.isExists(request.receiver))
                {
                    //Timestamp prüfen
                    Double unixtimeDouble = Convert.ToDouble(request.timestamp);
                    DateTime timestamp = Util.Converter.UnixTimeStampToDateTime(unixtimeDouble)
                        .AddMinutes(5);


                    if (DateTime.Now > timestamp)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    //TODO: request.sig_service

                    string publickey = Util.Converter.Base64StringToString(u.pubkey);
                    if (!SecurityLogic.verfiyOuterEnvelope(request, publickey))
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    User sender = UserDAO.Instance.findByUsername(user);
                    User receiver = UserDAO.Instance.findByUsername(request.receiver);

                    Message msg = new Message();
                    msg.cipher = request.inner_envelope.cipher;
                    msg.iv = request.inner_envelope.iv;
                    msg.key_recipient_enc = request.inner_envelope.key_recipient_enc;
                    msg.sig_recipient = request.inner_envelope.sig_recipient;
                    msg.sender = user;


                    UserDAO.Instance.addMessage(receiver, msg);

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("{user}")]
        [HttpDelete]
        public HttpResponseMessage deleteUser(string user)
        {
            if(UserDAO.Instance.isExists(user))
            {
                UserDAO.Instance.delete(user);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
