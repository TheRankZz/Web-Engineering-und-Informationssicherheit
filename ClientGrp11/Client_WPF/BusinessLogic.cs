using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Client_WPF
{
    /// <summary>
    /// Geschäftslogik
    /// </summary>
    class BusinessLogic
    {
        private Models.User user { get; set; }
        HttpClient client;

        private static BusinessLogic instance;

        private BusinessLogic()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://blue.cygnus.uberspace.de/classifiedserver/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static BusinessLogic Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BusinessLogic();
                }
                return instance;
            }
        }

        /// <summary>
        /// Neuen Benutzer registrieren und anmelden
        /// </summary>
        /// <param name="username">Benutzername</param>
        /// <param name="password">Kennwort</param>
        /// <returns>true wenn registrieren erfolgreich, andernfalls false</returns>
        public async Task<bool> register(string username, string password)
        {
            try
            {
                string usernameBase64 = Util.StringToBase64String(username);

                Models.RegisterRequest request = new Models.RegisterRequest();

                //Den salt_masterkey erstellen
                byte[] salt_masterkey = EncryptLogic.createSalt(64);
                request.salt_masterkey = Convert.ToBase64String(salt_masterkey);

                //Das Schlüsselpaar erstellen
                string publickey = null;
                string privatekey = null;
                EncryptLogic.createRSAKeyPair(out privatekey, out publickey);

                //Den PublicKey in Base64 konventieren
                request.pubkey_user = publickey;

                //Den PrivateKey verschlüsseln und in Base64 konventieren
                string encryptedprivkey = EncryptLogic.toEncryptPrivateKey(privatekey, password, salt_masterkey);
                request.privkey_user_enc = encryptedprivkey;

                HttpResponseMessage response = await client.PostAsJsonAsync(usernameBase64, request);
                if(response.IsSuccessStatusCode) {
                    return await login(username, password);
                } else {
                    throw new HttpRequestException(response.RequestMessage.Method + ":" 
                        + response.RequestMessage.RequestUri.AbsolutePath
                        + Environment.NewLine +
                        "HTTP-Statuscode: " + (int)response.StatusCode + " - " + response.StatusCode);
                }
            } catch (Exception ex) {
                Util.Log(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Benutzer anmelden
        /// </summary>
        /// <param name="username">Benutzer</param>
        /// <param name="password">Kennwort</param>
        /// <returns>true wenn Anmeldung erfolgreich, andernfalls false</returns>
        public async Task<bool> login(string username, string password)
        {
            string usernameBase64 = Util.StringToBase64String(username);
            try
            {
                HttpResponseMessage response = await client.GetAsync(usernameBase64);
                if (response.IsSuccessStatusCode) {
                    user = new Models.User();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<Models.LoginResponse>(jsonString);

                    user.Username = username;
                    user.publickey = model.pubkey_user;
                    user.salt_masterkey = Convert.FromBase64String(model.salt_masterkey);
                    user.privatekey = EncryptLogic.toDecryptPrivateKey(model.privkey_user_enc, password, user.salt_masterkey);

                    return true;
                } else {
                    throw new HttpRequestException(response.RequestMessage.Method + ":" 
                        + response.RequestMessage.RequestUri.AbsolutePath
                        + Environment.NewLine +
                        "HTTP-Statuscode: " + (int)response.StatusCode + " - " + response.StatusCode);
                }
            } catch(Exception ex) {
                Util.Log(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Aktuellen Benutzer abmelden
        /// </summary>
        public void logout()
        {
            if(this.user != null) {
                user = null;
            }
        }

        /// <summary>
        /// Aktuellen Benutzername holen
        /// </summary>
        /// <returns></returns>
        public string getUsername()
        {
            if(this.user != null) {
                return user.Username;
            }
            return null;
        }

        /// <summary>
        /// Öffentlicher Schlüssel vom Benutzer holen
        /// </summary>
        /// <param name="username">Benutzername</param>
        /// <returns>Öffentlicher Schlüssel</returns>
        private async Task<string> getPublicKeyFromUser(string username)
        {
            try
            {
                string usernameBase64 = Util.StringToBase64String(username);
                HttpResponseMessage response = await client.GetAsync(usernameBase64 + "/pubkey");
                if (response.IsSuccessStatusCode) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<Models.PubkeyResponse>(jsonString);
                    string publickeyfromreceiver = model.pubkey_user;

                    return publickeyfromreceiver;
                } else {
                    throw new HttpRequestException(response.RequestMessage.Method + ":" 
                        + response.RequestMessage.RequestUri.AbsolutePath
                        + Environment.NewLine +
                        "HTTP-Statuscode: " + (int)response.StatusCode + " - " + response.StatusCode);
                }

            } catch (Exception ex) {
                Util.Log(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Nachricht verschicken
        /// </summary>
        /// <param name="receiver">Empfänger</param>
        /// <param name="msg">Nachricht</param>
        /// <returns>true wenn erfolgreich, andernfalls false</returns>
        public async Task<bool> sendMessage(string receiver, string msg)
        {
            try
            {
                string usernameBase64 = Util.StringToBase64String(user.Username);

                //Symmetrischen Schlüssel und IV erzeugen
                byte[] key_recipient;
                byte[] iv;
                EncryptLogic.createAESKeyAndIV(out key_recipient, out iv);

                //Nachricht mit dem Symmetrischen Schlüssel und IV verschlüsseln. 
                byte[] msg_enc = EncryptLogic.toEncryptMessage(msg, key_recipient, iv);

                //PublicKey vom Empfänger holen
                string pubkey_rcv = await getPublicKeyFromUser(receiver);

                //Symmetrischen Schlüssel mit dem PublicKey vom Empfänger verschlüsseln.
                byte[] key_recipient_enc = EncryptLogic.toEncryptKeyRecipient(key_recipient, pubkey_rcv);


                Models.Message inner_envelope = new Models.Message();
                inner_envelope.sender = user.Username;
                inner_envelope.cipher = Convert.ToBase64String(msg_enc);
                inner_envelope.iv = Convert.ToBase64String(iv);
                inner_envelope.key_recipient_enc = Convert.ToBase64String(key_recipient_enc);

                //Inneren Umschlag signieren
                inner_envelope.sig_recipient =  EncryptLogic.createSignInnerEnvelope(inner_envelope, user.privatekey);

                Models.SendMessageRequest request = new Models.SendMessageRequest();
                request.envelope = inner_envelope;
                request.recipient = receiver;
                request.timestamp = Convert.ToString(Util.UnixTimeNow());

                //Äußeren Umschlag signieren
                request.sig_service = EncryptLogic.createSignOuterEnvelope(inner_envelope, request.timestamp, request.recipient, user.privatekey);

                HttpResponseMessage response = await client.PostAsJsonAsync(usernameBase64 + "/message", request);
                if (response.IsSuccessStatusCode) {
                    return true;
                } else {
                    throw new HttpRequestException(response.RequestMessage.Method + ":" 
                        + response.RequestMessage.RequestUri.AbsolutePath
                        + Environment.NewLine +
                        "HTTP-Statuscode: " + (int)response.StatusCode + " - " + response.StatusCode);
                }
            } catch (Exception ex) {
                Util.Log(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Eine Nachricht holen
        /// </summary>
        /// <returns>Nachricht(ViewMessage)</returns>
        public async Task<Models.ViewMessage> getMessage()
        {
            try
            {
                string usernameBase64 = Util.StringToBase64String(user.Username);

                var request = new Models.GetMessageRequest();
                request.timestamp = Convert.ToString(Util.UnixTimeNow());

                //Anfrage signieren
                request.sig_utime = EncryptLogic.createSignGetMessageRequest(request.timestamp, user.Username, user.privatekey);

                string url = String.Format(usernameBase64 + "/messages?timestamp={0}&sig_utime={1}",
                    request.timestamp, HttpUtility.UrlEncode(request.sig_utime));
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode) {

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<Models.Message>(jsonString);

                    //Signatur prüfen (Ob die Nachricht verändert wurde)
                    string publickey = await getPublicKeyFromUser(content.sender);
                    if (!EncryptLogic.verfiyInnerEnvelope(content, publickey)) {
                        throw new Exception("Die Nachricht ist nicht verifiziert.");
                    }

                    //IV 
                    byte[] iv = Convert.FromBase64String(content.iv);

                    //Key_recipient_enc(Symmetrischer Schlüssel verschlüsselt) entschlüsseln
                    byte[] key_recipient_enc = Convert.FromBase64String(content.key_recipient_enc);
                    byte[] key_recipient = EncryptLogic.toDecryptKeyRecipient(key_recipient_enc, user.privatekey);

                    //Cipher(verschlüsselte Nachricht) entschlüsseln
                    byte[] cipher_bytes = Convert.FromBase64String(content.cipher);
                    string msg_dec = EncryptLogic.toDecryptMessage(cipher_bytes, key_recipient, iv);

                    var vm = new Models.ViewMessage();
                    vm.sender = content.sender;
                    vm.content = msg_dec;

                    return vm;
                } else {
                    throw new HttpRequestException(response.RequestMessage.Method + ":"
                        + response.RequestMessage.RequestUri.AbsolutePath
                        + Environment.NewLine +
                        "HTTP-Statuscode: " + (int)response.StatusCode + " - " + response.StatusCode);
                }
            } catch (Exception ex) {
                Util.Log(ex.Message);
            }
            return null;
        }
    }
}
