using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SafeRequest
{
    public class Encryption
    {
        private readonly byte[] _key;

        public Encryption(string key)
            : this(key, Encoding.ASCII)
        { }

        public Encryption(string key, Encoding encoding)
        {
            Encoding = encoding;

            var bytes = encoding.GetBytes(key);
            using (var sha = SHA256.Create())
                _key = sha.ComputeHash(bytes);

            _key = _key.Take(32).ToArray();
            IV = Enumerable.Range(1, 16)
                .Select(x => (byte)0)
                .ToArray();
        }

        public Encoding Encoding { get; private set; }
        public byte[] IV { get; private set; }

        public string DecryptString(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            var plainBytes = PerformAesCryption(c => c.CreateDecryptor(), cipherBytes);
            return Encoding.GetString(plainBytes, 0, plainBytes.Length);
        }

        public string EncryptString(string plainText)
        {
            var plainBytes = Encoding.GetBytes(plainText);

            var cipherBytes = PerformAesCryption(c => c.CreateEncryptor(), plainBytes);
            return Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
        }

        public void SetIV(byte[] iv)
        {
            IV = iv;
        }

        private byte[] PerformAesCryption(Func<SymmetricAlgorithm, ICryptoTransform> cryptoDerivationFunction, byte[] data)
        {
            if (cryptoDerivationFunction == null)
                throw new ArgumentNullException(nameof(cryptoDerivationFunction));

            var crypto = Aes.Create();
            crypto.Mode = CipherMode.CBC;
            crypto.Key = _key;
            crypto.IV = IV;

            var cryptor = cryptoDerivationFunction(crypto);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, cryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return memoryStream.ToArray();
            }
        }
    }
}