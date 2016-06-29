using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client_WPF
{
    class EncryptLogic
    {
        /// <summary>
        /// Zufalls-Salt erzeugen
        /// </summary>
        /// <param name="keysize">Salt-Größe</param>
        /// <returns>Salt</returns>
        public static byte[] createSalt(int keysize)
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[keysize];
            rngCsp.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Masterkey für Benutzer erzeugen
        /// </summary>
        /// <param name="pwd">Kennwort vom Benutzer</param>
        /// <param name="salt_masterkey">Salt für den Masterkey</param>
        /// <param name="keysize">Key-Größe</param>
        /// <returns></returns>
        private static byte[] createMasterkey(string pwd, byte[] salt_masterkey, int keysize)
        {
            DeriveBytes derivedKey = new Rfc2898DeriveBytes(pwd, salt_masterkey, 1000);
            return derivedKey.GetBytes(keysize >> 3);
        }

        /// <summary>
        /// RSA: Öffentlicher und privater Schlüssel erzeugen (PEM-Format)
        /// </summary>
        /// <param name="privatekey">Privater Schlüssel</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        public static void createRSAKeyPair(out string privatekey, out string publickey)
        {
            //RSA-Schlüssel-Paar erzeugen
            var r = new RsaKeyPairGenerator();
            r.Init(new KeyGenerationParameters(new SecureRandom(), 1024));
            var keys = r.GenerateKeyPair();

            //Private Key im PEM-Format 
            TextWriter textWriter = new StringWriter();
            var pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(keys.Private);
            pemWriter.Writer.Flush();
            textWriter.Close();
            privatekey = textWriter.ToString();

            //Public Key im PEM-Formart
            TextWriter textWriter1 = new StringWriter();
            var pemWriter1 = new PemWriter(textWriter1);
            pemWriter1.WriteObject(keys.Public);
            pemWriter1.Writer.Flush();
            textWriter1.Close();
            publickey = textWriter1.ToString();
        }

        /// <summary>
        /// Privater Schlüssel mit AES verschlüsseln
        /// </summary>
        /// <param name="privkey">Privater Schlüssel</param>
        /// <param name="password">Kennwort vom Benutzer</param>
        /// <param name="salt_masterkey">Salt</param>
        /// <returns>Privater Schlüsel(verschlüsselt) in Base64</returns>
        public static string toEncryptPrivateKey(string privkey, string password, byte[] salt_masterkey)
        {
            byte[] data = Util.GetBytes(privkey);
            byte[] encryptedData = null;
            using (AesCryptoServiceProvider aesp = new AesCryptoServiceProvider())
            {
                aesp.GenerateIV();
                aesp.Key = createMasterkey(password, salt_masterkey, aesp.KeySize);
                aesp.Mode = CipherMode.CBC;
                aesp.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream(data.Length))
                {
                    ms.Write(aesp.IV, 0, 16);
                    using (ICryptoTransform encryptor = aesp.CreateEncryptor(aesp.Key, aesp.IV))
                    {
                        using (CryptoStream crs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            crs.Write(data, 0, data.Length);
                            crs.FlushFinalBlock();
                        }
                    }
                    encryptedData = ms.ToArray();
                }
            }
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Privater Schlüssel mit AES entschlüsseln
        /// </summary>
        /// <param name="privkey_enc">Privater Schlüssel(verschlüsselt)</param>
        /// <param name="password">Kennwort vom Benutzer</param>
        /// <param name="salt_masterkey">Salt</param>
        /// <returns>Privater Schlüssel(entschlüsselt)</returns>
        public static string toDecryptPrivateKey(string privkey_enc, string password, byte[] salt_masterkey)
        {
            byte[] data = Convert.FromBase64String(privkey_enc);
            byte[] decryptedData = new byte[data.Length];
            using (AesCryptoServiceProvider aesp = new AesCryptoServiceProvider())
            {
                aesp.Key = createMasterkey(password, salt_masterkey, aesp.KeySize);
                aesp.Mode = CipherMode.CBC;
                aesp.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, 16);
                    using (ICryptoTransform decryptor = aesp.CreateDecryptor(aesp.Key, iv))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                        }
                    }
                }
            }
            return Util.GetString(decryptedData);
        }

        /// <summary>
        /// Symmetrischen Schlüssel mit RSA verschlüsseln
        /// </summary>
        /// <param name="key">Symmetrischer Schlüssel(klartext)</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        /// <returns>Symmetrischer Schlüssel (verschlüsselt)</returns>
        public static byte[] toEncryptKeyRecipient(byte[] key_recipient, string publickey)
        {
            var key = getPublicKey(publickey);

            var rsa = new RsaEngine();
            rsa.Init(true, key);
            var key_enc = rsa.ProcessBlock(key_recipient, 0, key_recipient.Length);
            return key_enc;
        }

        /// <summary>
        /// Symmetrischen Schlüssel mit RSA entschlüsseln
        /// </summary>
        /// <param name="key_recipient_enc">Symmetrischer Schlüssel(verschlüsselt)</param>
        /// <param name="privatekey">Privater Schlüssel</param>
        /// <returns>Symmetrischer Schlüssel(entschlüsselt)</returns>
        public static byte[] toDecryptKeyRecipient(byte[] key_recipient_enc, string privatekey)
        {
            var key = getPrivateKey(privatekey);

            var rsa = new RsaEngine();
            rsa.Init(false, key);
            var key_dec = rsa.ProcessBlock(key_recipient_enc, 0, key_recipient_enc.Length);
            return key_dec;
        }

        /// <summary>
        /// Öffentlicher Schlüssel aus PEM-String holen
        /// </summary>
        /// <param name="publickey">Öffentlicher Schlüssel im PEM-Format</param>
        /// <returns>AsymmetricKeyParameter</returns>
        private static AsymmetricKeyParameter getPublicKey(string publickey)
        {
            RsaKeyParameters asmkeypar;

            using (var reader = new StringReader(publickey))
                asmkeypar = (RsaKeyParameters)new PemReader(reader).ReadObject();

            return asmkeypar;
        }

        /// <summary>
        /// Privater Schlüssel aus PEM-String holen
        /// </summary>
        /// <param name="privatekey">Private Schlüssel im PEM-Format</param>
        /// <returns>AsymmetricKeyParameter</returns>
        private static AsymmetricKeyParameter getPrivateKey(string privatekey)
        {
            AsymmetricCipherKeyPair asmkeypar;

            using (var reader = new StringReader(privatekey))
                asmkeypar = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            return (RsaKeyParameters)asmkeypar.Private;
        }

        /// <summary>
        /// Nachricht mit AES verschlüsseln
        /// </summary>
        /// <param name="encryptedBytes">Nachricht(klartext)</param>
        /// <param name="key">Schlüssel</param>
        /// <param name="IV">Initialisierungsvektor</param>
        /// <returns>Nachricht(verschlüsselt)</returns>
        public static string toDecryptMessage(byte[] encryptedBytes, byte[] key, byte[] IV)
        {
            string text_dec = null;

            using (AesManaged aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream crs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(crs))
                        {
                            text_dec = sr.ReadToEnd();
                        }
                    }
                }
            }

            return text_dec;
        }

        /// <summary>
        /// Nachricht mit AES entschlüsseln
        /// </summary>
        /// <param name="msg">Nachricht(verschlüsselt)</param>
        /// <param name="key">Schlüssel</param>
        /// <param name="IV">Initialisierungsvektor</param>
        /// <returns>Nachricht(entschlüsselt)</returns>
        public static byte[] toEncryptMessage(string msg, byte[] key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
    
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream crs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(crs))
                        {
                            sw.Write(msg);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
        }

        /// <summary>
        /// Schlüssel und Initialisierungsvektor für AES erzeugen
        /// </summary>
        /// <param name="key">Schlüssel</param>
        /// <param name="IV">Initialisierungsvektor</param>
        public static void createAESKeyAndIV(out byte[] key, out byte[] IV)
        {
            using (AesManaged aes = new AesManaged())
            { 
                key = aes.Key;
                IV = aes.IV;
            }
        }

        /// <summary>
        /// Signieren des Inhaltes
        /// </summary>
        /// <param name="content">Inhalt</param>
        /// <param name="privateKey">Privater Schlüssel</param>
        /// <returns>Signatur(base64)</returns>
        private static string createSign(string content, string privateKey)
        {
            var privKey = getPrivateKey(privateKey);

            ISigner sig = SignerUtilities.GetSigner("SHA256withRSA");

            sig.Init(true, privKey);
            byte[] data = Util.GetBytes(content);
            sig.BlockUpdate(data, 0, data.Length);
            byte[] signedBytes = sig.GenerateSignature();

            return Convert.ToBase64String(signedBytes);
        }

        /// <summary>
        /// Signieren des inneren Umschlages
        /// </summary>
        /// <param name="inner_envelope">Innere Umschlag</param>
        /// <param name="privateKey">Privater Schlüssel</param>
        public static string createSignInnerEnvelope(Models.Message envelope, string privateKey)
        {
            string content = envelope.sender + envelope.cipher + envelope.iv + envelope.key_recipient_enc;
            return createSign(content, privateKey);
        }

        /// <summary>
        /// Signieren des äußeren Umschlages
        /// </summary>
        /// <param name="outer_envelope">äußer Umschlag</param>
        /// <param name="privateKey">Privater Schlüssel</param>
        public static string createSignOuterEnvelope(Models.Message envelope, string timestamp, string recipient, string privateKey)
        {
            string content = envelope.sender + envelope.cipher + envelope.iv + envelope.key_recipient_enc
                + envelope.sig_recipient + timestamp + recipient;
            return createSign(content, privateKey);
        }

        /// <summary>
        /// Signieren der Get-Anfrage zum Nachrichten holen
        /// </summary>
        /// <param name="request">Anfrage</param>
        /// <param name="username">Benutzername</param>
        /// <param name="privateKey">Privater Schlüssel</param>
        public static string createSignGetMessageRequest(string timestamp, string username, string privateKey)
        {
            string content = username + timestamp;
            return createSign(content, privateKey);
        }

        /// <summary>
        /// Verifizieren der Nachricht
        /// </summary>
        /// <param name="inner_envelope">Innere Umschlag</param>
        /// <param name="publicKey">Öffentlicher Schlüssel</param>
        /// <returns>true wenn verifizierung erfolgreich, andernfalls false</returns>
        public static bool verfiyInnerEnvelope(Models.Message envelope, string publicKey)
        {
            var pubkey = getPublicKey(publicKey);
            bool success = false;

            string content = envelope.sender + envelope.cipher + envelope.iv + envelope.key_recipient_enc;

            ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
            byte[] data = Util.GetBytes(content);
            
            signer.Init(false, pubkey);
            signer.BlockUpdate(data, 0, data.Length);
            byte[] signature = Convert.FromBase64String(envelope.sig_recipient);
            success = signer.VerifySignature(signature);
            return success;
        }
    }
}
