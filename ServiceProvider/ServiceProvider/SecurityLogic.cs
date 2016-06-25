using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        /// <summary>
        /// Öffentlicher Schlüssel aus PEM-String holen
        /// </summary>
        /// <param name="publickey">Öffentlicher Schlüssel im PEM-Format</param>
        /// <returns>AsymmetricKeyParameter</returns>
        private static AsymmetricKeyParameter getPublicKey(string publickey)
        {
            RsaKeyParameters rsaparam;
            using (var r = new StringReader(publickey))
            {
                rsaparam = (RsaKeyParameters)new PemReader(r).ReadObject();
            }
            return rsaparam;
        }



        private static bool verfiyContent(string content, byte[] signature, string publickey)
        {
            var key = getPublicKey(publickey);
            bool success = false;

            ISigner sig = SignerUtilities.GetSigner("SHA256withRSA");
            byte[] data = Util.Converter.GetBytes(content);
            sig.Init(false, key);
            sig.BlockUpdate(data, 0, data.Length);

            //Signatur-Bytes prüfen
            success = sig.VerifySignature(signature);

            return success;
        }
    }
}