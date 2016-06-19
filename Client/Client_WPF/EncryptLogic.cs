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
        /// Erzeugt einen Salt für den Masterkey
        /// </summary>
        /// <returns></returns>
        public static byte[] createSaltMasterKey()
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] salt_masterkey = new byte[64];
            rngCsp.GetBytes(salt_masterkey);
            return salt_masterkey;
        }

        /// <summary>
        /// MasterKey erstellen
        /// </summary>
        /// <param name="pwd">Password vom Benutzer</param>
        /// <param name="salt_masterkey">Salt für den Masterkey</param>
        /// <param name="keysize">Key-Größe</param>
        /// <returns></returns>
        private static byte[] createMasterkey(string pwd, byte[] salt_masterkey, int keysize)
        {
            DeriveBytes derivedKey = new Rfc2898DeriveBytes(pwd, salt_masterkey, 1000);
            return derivedKey.GetBytes(keysize >> 3);
        }

        /// <summary>
        /// RSA Public- und Privatekey erstellen
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
        /// Privatekey mit password und masterkey verschlüsseln
        /// </summary>
        /// <param name="privkey">Privatekey</param>
        /// <param name="password">Password vom Benutzer</param>
        /// <param name="salt_masterkey">Salt für den Masterkey</param>
        /// <returns>Verschlüsselter PrivateKey</returns>
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
        /// Entschlüsseln vom Privatekey
        /// </summary>
        /// <param name="privkey_enc">PrivateKey verschlüsselt</param>
        /// <param name="password">Password vom Benutzer</param>
        /// <param name="salt_masterkey">Salt für den Masterkey</param>
        /// <returns>Entschlüsselter PrivateKey</returns>
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
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="publickey"></param>
        /// <returns></returns>
        public static string encrptMessage(string msg, string publickey)
        {
            //get a stream from the string
            var sr = new System.IO.StringReader(publickey);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var pubKey = (RSAParameters)xs.Deserialize(sr);
   
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);
 
            //for encryption, always handle bytes...
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(msg);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            string cypherText = Convert.ToBase64String(bytesCypherText);
            return cypherText;
        }


        public static string decryptMessage(string cypher, string privatekey)
        {
            byte[] bytesCypherText = Convert.FromBase64String(cypher);

            var sr = new System.IO.StringReader(privatekey);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var privKey = (RSAParameters)xs.Deserialize(sr);

            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);

            //decrypt and strip pkcs#1.5 padding
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            string plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
            return plainTextData;
        }
    }
}
