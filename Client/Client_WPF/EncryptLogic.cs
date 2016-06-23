using System;
using System.Collections.Generic;
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
        /// Zufalls Salt erzeugen
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
        /// RSA: öffentlicher und privaten Schlüssel erzeugen
        /// </summary>
        /// <param name="privatekey">Privater Schlüssel</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        public static void createNewKeyPair(out string privatekey, out string publickey)
        {
            var csp = new RSACryptoServiceProvider(2048);

            //Publickey in String umwandeln
            var pubKey = csp.ExportParameters(false);
            var sw1 = new System.IO.StringWriter();
            var xs1 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs1.Serialize(sw1, pubKey);
            publickey = sw1.ToString();

            //Privatekey in String umwandeln
            var privKey = csp.ExportParameters(true);
            var sw2 = new System.IO.StringWriter();
            var xs2 = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs2.Serialize(sw2, privKey);
            privatekey = sw2.ToString();
        }

        /// <summary>
        /// Privater Schlüssel mit AES verschlüsseln
        /// </summary>
        /// <param name="privkey">Privater Schlüssel</param>
        /// <param name="password">Kennwort vom Benutzer</param>
        /// <param name="salt_masterkey">Salt</param>
        /// <returns>Privater Schlüsel(verschlüsselt) in Base64</returns>
        public static string encryptPrivatekey(string privkey, string password, byte[] salt_masterkey)
        {
            byte[] data = Util.GetBytes(privkey);
            byte[] encryptedData = null;
            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                provider.GenerateIV();
                provider.Key = createMasterkey(password, salt_masterkey, provider.KeySize);
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                using (MemoryStream memStream = new MemoryStream(data.Length))
                {
                    memStream.Write(provider.IV, 0, 16);
                    using (ICryptoTransform encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                    }
                    encryptedData = memStream.ToArray();
                }
            }
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Privater Schlüssel entschlüsseln
        /// </summary>
        /// <param name="privkey_enc">Privater Schlüssel(verschlüsselt)</param>
        /// <param name="password">Kennwort vom Benutzer</param>
        /// <param name="salt_masterkey">Salt</param>
        /// <returns>Privater Schlüssel(entschlüsselt)</returns>
        public static string decryptPrivatekey(string privkey_enc, string password, byte[] salt_masterkey)
        {
            byte[] data = Convert.FromBase64String(privkey_enc);
            byte[] decryptedData = new byte[data.Length];
            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                provider.Key = createMasterkey(password, salt_masterkey, provider.KeySize);
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                using (MemoryStream memStream = new MemoryStream(data))
                {
                    byte[] iv = new byte[16];
                    memStream.Read(iv, 0, 16);
                    using (ICryptoTransform decryptor = provider.CreateDecryptor(provider.Key, iv))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                        }
                    }
                }
            }
            return Util.GetString(decryptedData);
        }

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

        /// <summary>
        /// Symmetrischen Schlüssel mit RSA verschlüsseln
        /// </summary>
        /// <param name="key">Symmetrischer Schlüssel(klartext) als Bytes</param>
        /// <param name="publickey">Öffentlicher Schlüssel</param>
        /// <returns>Symmetrischer Schlüssel (verschlüsselt)</returns>
        public static byte[] encryptKeyRecipient(byte[] key, string publickey)
        {
            var pubKey = deserializeRSAKey(publickey);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);
            var key_enc = csp.Encrypt(key, false);
            return key_enc;
        }

        /// <summary>
        /// Symmetrischen Schlüssel mit RSA entschlüsseln
        /// </summary>
        /// <param name="key_recipient_enc">Symmetrischer Schlüssel(verschlüsselt)</param>
        /// <param name="privatekey">Privater Schlüssel</param>
        /// <returns>Symmetrischer Schlüssel(entschlüsselt)</returns>
        public static byte[] decryptKeyRecipient(byte[] key_recipient_enc, string privatekey)
        {
            var privKey = deserializeRSAKey(privatekey);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);

            var key_dec = csp.Decrypt(key_recipient_enc, false);
            return key_dec;
        }

        /// <summary>
        /// Nachricht mit AES verschlüsseln
        /// </summary>
        /// <param name="encryptedBytes">Nachricht(klartext)</param>
        /// <param name="key">Schlüssel</param>
        /// <param name="IV">Initialisierungsvektor</param>
        /// <returns>Nachricht(verschlüsselt)</returns>
        public static string decryptMessage(byte[] encryptedBytes, byte[] key, byte[] IV)
        {
            string text_dec = null;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            text_dec = srDecrypt.ReadToEnd();
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
        public static byte[] encryptMessage(string msg, byte[] key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aesAlg = new AesManaged())
            {
    
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(msg);
                        }
                        encrypted = msEncrypt.ToArray();
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
        public static void createKeyAndIV(out byte[] key, out byte[] IV)
        {
            using (AesManaged myAes = new AesManaged())
            { 
                key = myAes.Key;
                IV = myAes.IV;
            }
        }
    }
}
