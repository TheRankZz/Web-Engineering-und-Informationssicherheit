using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ServiceProvider
{
    public class SecurityLogic
    {
        /// <summary>
        /// RSA-Key deserialisieren
        /// </summary>
        /// <param name="key">Privatekey oder PublicKey</param>
        /// <returns>RSAParameters</returns>
        private static RSAParameters deserializeRSAKey(string key)
        {
            var sr = new System.IO.StringReader(key);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }

        public static bool verfiyGetMessageRequest(Models.GetMessageRequest request, string username, string publickey)
        {
            string content = username + request.timestamp;
            byte[] signature = Convert.FromBase64String(request.dig_sig);

            return verfiyContent(content, signature, publickey);
        }

        /// <summary>
        /// Verifizieren der Nachricht
        /// </summary>
        /// <param name="outer_envelope">Äußer Umschlag</param>
        /// <param name="publicKey">Öffentlicher Schlüssel</param>
        /// <returns>true wenn verifizierung erfolgreich, andernfalls false</returns>
        public static bool verfiyOuterEnvelope(Models.SendMessageRequest outer_envelope, string publickey)
        {
            string content_inner = outer_envelope.inner_envelope.cipher + outer_envelope.inner_envelope.iv
                + outer_envelope.inner_envelope.key_recipient_enc + outer_envelope.inner_envelope.sender;

            string content = content_inner + outer_envelope.timestamp + outer_envelope.receiver;

            byte[] signature = Convert.FromBase64String(outer_envelope.sig_service);

            return verfiyContent(content, signature, publickey);
        }


        private static bool verfiyContent(string content, byte[] signature, string publickey)
        {
            bool success = false;
            var pubkey = deserializeRSAKey(Util.Converter.Base64StringToString(publickey));

            using (var rsa = new RSACryptoServiceProvider())
            {

                byte[] data = Util.Converter.GetBytes(content);
                try
                {
                    rsa.ImportParameters(pubkey);

                    success = rsa.VerifyData(data, "SHA256", signature);
                }
                catch (CryptographicException e)
                {
                    Debug.Fail(e.Message);
                }
            }
            return success;
        }
    }
}