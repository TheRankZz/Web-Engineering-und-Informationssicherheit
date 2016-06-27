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
    public class VerifyLogic
    {
        /// <summary>
        /// Verifizieren der GetMessage-Anfrage
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="username">Benutzername</param>
        /// <param name="dig_sig">Signatur</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        /// <returns>true wenn verifizierung erfolgreich, andernfalls false</returns>
        public static bool verifyGetMessageRequest(string timestamp, string username, string dig_sig, string publickey)
        {
            string content = username + timestamp;
            byte[] signature = Convert.FromBase64String(dig_sig);
            return verifyContent(content, signature, publickey);
        }

        /// <summary>
        /// Verifizieren der Nachricht
        /// </summary>
        /// <param name="outer_envelope">Äußer Umschlag</param>
        /// <param name="publicKey">Öffentlicher Schlüssel</param>
        /// <returns>true wenn verifizierung erfolgreich, andernfalls false</returns>
        public static bool verifyOuterEnvelope(Models.MessageResponse envelope, string timestamp, string receiver, string sig_service, string publickey)
        {
            string content = envelope.sender + envelope.cipher + envelope.iv + envelope.key_recipient_enc 
                + envelope.sig_recipient + timestamp + receiver;

            byte[] signature = Convert.FromBase64String(sig_service);

            return verifyContent(content, signature, publickey);
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


        /// <summary>
        /// Inhalt mit Signatur prüfen
        /// </summary>
        /// <param name="content">Inhalt</param>
        /// <param name="signature">Signatur</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        /// <returns>true wenn die Prüfung erfolgreich war, andernfalls false</returns>
        private static bool verifyContent(string content, byte[] signature, string publickey)
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