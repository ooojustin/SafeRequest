using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SafeRequest.NET {

    class Encryption {

        public static string EncryptString(string plainText, string key, byte[] iv = null) {
            // determine init vector
            if (iv == null) iv = _iv;
            // get byte array from key [ bcrypt hash = cost factor ;) ]
            byte[] encKey = GetBCryptKey(key);
            // encrypt text with key/iv
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = encKey.Take(32).ToArray();
            encryptor.IV = iv;
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

        public static string DecryptString(string cipherText, string key, byte[] iv = null) {
            // determine init vector
            if (iv == null) iv = _iv;
            // get byte array from key [ bcrypt hash = cost factor ;) ]
            byte[] decKey = GetBCryptKey(key);
            // decrypt cipher with key/iv
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = decKey.Take(32).ToArray();
            encryptor.IV = iv;
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

        private static byte[] GetBCryptKey(string key) {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(key, 12);
            return Encoding.ASCII.GetBytes(hashedPassword);
        }

        private static byte[] _iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        public void SetIV(byte[] iv) { _iv = iv; }

    }

}
