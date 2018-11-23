using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SafeRequest {

    public class Encryption {

        public Encryption(string key) {
            byte[] bytes = Encoding.ASCII.GetBytes(key);
            EstablishKey(bytes);
        }

        public Encryption(SecureString key) {
            byte[] bytes = key.ToByteArray(Encoding.ASCII);
            EstablishKey(bytes);
        }

        private void EstablishKey(byte[] bytes) {
            SHA256 mySHA256 = SHA256.Create();
            _key = mySHA256.ComputeHash(bytes);
        }

        private byte[] _key;
        private byte[] _iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        public void SetIV(byte[] iv) { _iv = iv; }

        public string EncryptString(string plainText) {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = _key.Take(32).ToArray();
            encryptor.IV = _iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }

        public string DecryptString(string cipherText) {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = _key.Take(32).ToArray();
            encryptor.IV = _iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            } finally {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
        }

    }

}
