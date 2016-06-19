using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client_WPF
{
    class BusinessLogic
    {
        private Models.User user { get; set; }
        HttpClient client;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private static BusinessLogic instance;

        private BusinessLogic()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://10.60.70.15/");
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

        public async Task<bool> register(string username, string password)
        {
            try
            {
                Models.RegisterRequest request = new Models.RegisterRequest();

                //Den salt_masterkey erstellen
                byte[] salt_masterkey = EncryptLogic.createSaltMasterKey();
                request.salt_masterkey = Convert.ToBase64String(salt_masterkey);

                //Das Schlüsselpaar erstellen
                string publickey = null;
                string privatekey = null;
                EncryptLogic.createNewKeyPair(out privatekey, out publickey);

                //Den PublicKey in Base64 konventieren
                request.pubkey = Util.StringToBase64String(publickey);

                //Den PrivateKey verschlüsseln und in Base64 konventieren
                string encryptedprivkey = EncryptLogic.encryptPrivatekey(privatekey, password, salt_masterkey);
                request.privkey_enc = encryptedprivkey;

                HttpResponseMessage response = await client.PostAsJsonAsync("/" + username, request);

                if (response.IsSuccessStatusCode)
                {
                    return await login(username, password);
                }
                
            } catch (Exception ex) {

            }
            return false;
        }



        public async Task<bool> login(string username, string password)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/" + username);
                if (response.IsSuccessStatusCode)
                {
                    user = new Models.User();
                    Models.LoginResponse loginresponse = await response.Content.ReadAsAsync<Models.LoginResponse>();

                    user.Username = username;
                    user.publickey = Util.Base64StringToString(loginresponse.pubkey);
                    user.salt_masterkey = Convert.FromBase64String(loginresponse.salt_masterkey);
                    user.privatekey = EncryptLogic.decryptPrivatekey(loginresponse.privkey_enc, password, user.salt_masterkey);

                    return true;
                }
            } catch(Exception ex)
            {
            }
            return false;
        }


        public void logout()
        {
            if(this.user != null)
            {
                user = null;
            }
        }

        public string getUsername()
        {
            if(this.user != null)
            {
                return user.Username;
            }
            return null;
        }


        public async Task<string> getPublicKeyFromUser(string username)
        {
            HttpResponseMessage response = await client.GetAsync("/" + username + "/publickey");
            if (response.IsSuccessStatusCode)
            {
                Models.PubkeyResponse pubkeyresponse = await response.Content.ReadAsAsync<Models.PubkeyResponse>();
                string publickeyfromreceiver = Util.Base64StringToString(pubkeyresponse.pubkey);
                return publickeyfromreceiver;
            }
            return null;
        }



        public async Task<bool> sendMessage(string receiver, string msg)
        {
            if (receiver == "true")
            {
                return true;
            }
            return false;
        }


        public async Task<Models.Message> getMessage()
        {
            var msg = new Models.Message();
            msg.content = "Test-Nachrichten-Text";
            msg.sender = "lennart";
            return msg;
        }

    }
}
