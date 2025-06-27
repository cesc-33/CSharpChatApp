using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ChatApp
{
    public class CryptoHelper
    {
        private readonly RSACryptoServiceProvider rsa;

        public string PublicKeyXML => rsa.ToXmlString(false);
        public string PrivateKeyXML => rsa.ToXmlString(true);

        public CryptoHelper()
        {
            rsa = new RSACryptoServiceProvider(2048);
        }

        // 1. AES-Key + IV erzeugen
        public (byte[] key, byte[] iv) GenerateAesKey()
        {
            using Aes aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
            return (aes.Key, aes.IV);
        }

        // 2. Text mit AES verschlüsseln
        public byte[] EncryptWithAes(string plainText, byte[] key,  byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        // 3. AES-verschlüsselten Text entschlüsseln
        public string DecryptWithAes(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // 4. AES-Key / andere Daten mit RSA (öffentlich) verschlüsseln
        public byte[] EncryptWithRsa(byte[] data, string publicKeyXml)
        {
            using var rsaReceiver = new RSACryptoServiceProvider();
            rsaReceiver.FromXmlString(publicKeyXml);
            return rsaReceiver.Encrypt(data, false);
        }

        // 5. RSA-Verschlüsselte Daten mit privatem Schlüssel entschlüsseln
        public byte[] DecryptWithRsa(byte[] cipherData)
        {
            return rsa.Decrypt(cipherData, false);
        }
        // Helfer
        public string ExtractPlainTextFromRtf(string rtf)
        {
            using var rtb = new RichTextBox();
            rtb.Rtf = rtf;
            return rtb.Text;
        }
    }
}
