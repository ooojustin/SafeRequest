using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace SafeRequest
{
    public class SafeRequest
    {
        public const string UserAgent = nameof(SafeRequest);
        private readonly Encryption encryption;

        public SafeRequest(string key, byte[] iv = null)
        {
            encryption = new Encryption(key);

            if (iv != null)
                SetIV(iv);
        }

        public bool NullifyProxy { get; set; } = true;

        public string DecryptString(string cipherText)
        {
            return encryption.DecryptString(cipherText);
        }

        public string EncryptString(string plainText)
        {
            return encryption.EncryptString(plainText);
        }

        public WebClient GetClient()
        {
            var webClient = new WebClient();

            if (!string.IsNullOrEmpty(UserAgent))
                webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (NullifyProxy)
                webClient.Proxy = null;

            return webClient;
        }

        public Response Request(string url, NameValueCollection values = null)
        {
            var dataDictionary = GetDictionary(values ?? new NameValueCollection());
            if (dataDictionary.ContainsKey("authentication_key"))
                throw new Exception("Key \"authentication_key\" must not be defined.");

            var response = new Response();
            var authKey = GetRandomAuthKey(5);
            dataDictionary.Add("authentication_key", encryption.EncryptString(authKey));

            try
            {
                var rawRequest = JsonConvert.SerializeObject(dataDictionary);
                var encryptedRequest = encryption.EncryptString(rawRequest);

                var webClient = GetClient();
                var encryptedResponse = webClient.UploadString(url, encryptedRequest);
                var rawResponse = encryption.DecryptString(encryptedResponse);

                response.Initialize(rawResponse, authKey);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static Dictionary<string, object> GetDictionary(NameValueCollection values)
        {
            var result = new Dictionary<string, object>();

            foreach (string key in values.Keys)
                result.Add(key, values[key]);

            return result;
        }

        private static string GetRandomAuthKey(int keyLength)
        {
            var randomizer = new Random();

            var keyChars = Enumerable.Range(1, keyLength)
                .Select(x => (char)randomizer.Next(33, 127))
                .ToArray();

            return new string(keyChars);
        }

        private void SetIV(byte[] iv)
        {
            if (iv != null && iv.Length == 16)
                encryption.SetIV(iv);
            else
                throw new ArgumentException($"{nameof(iv)} is invalid.");
        }
    }
}