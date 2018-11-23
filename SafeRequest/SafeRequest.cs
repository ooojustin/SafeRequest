using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security;

namespace SafeRequest {

    public class SafeRequest {

        public SafeRequest(string encryptionKey, byte[] iv = null) {
            encryption = new Encryption(encryptionKey);
            SetIV(iv);
        }

        public SafeRequest(SecureString encryptionKey, byte[] iv = null) {
            encryption = new Encryption(encryptionKey);
            SetIV(iv);
        }

        private void SetIV(byte[] iv) {
            if (iv != null)
                if (iv.Count() == 16)
                    encryption.SetIV(iv);
        }

        private string USER_AGENT = "SafeRequest";
        public string GetUserAgent() { return USER_AGENT; }
        public void SetUserAgent(string userAgent) { USER_AGENT = userAgent; }

        private Encryption encryption;
        public string EncryptString(string plainText) { return encryption.EncryptString(plainText); }
        public string DecryptString(string cipherText) { return encryption.DecryptString(cipherText); }

        public Response Request(string url, NameValueCollection values = null) {
            Response response = new Response(encryption);
            try {
                WebClient web = GetClient();
                if (values == null)
                    values = new NameValueCollection();
                Dictionary<string, object> dictionary = GetDictionary(values);
                if (dictionary.ContainsKey("authentication_key"))
                    throw new Exception("Key \"authentication_key\" must not be defined.");
                dictionary.Add("authentication_key", response.EncryptedAuth());
                string rawUpload = JsonConvert.SerializeObject(dictionary);
                string upload = encryption.EncryptString(rawUpload);
                string rawEncrypted = web.UploadString(url, upload);
                string raw = encryption.DecryptString(rawEncrypted);
                response.Initialize(raw);
            } catch (Exception ex) {
                response.status = false;
                response.message = ex.Message;
            }
            return response;
        }

        public WebClient GetClient(string userAgent = "") {
            if (string.IsNullOrEmpty(userAgent))
                userAgent = USER_AGENT;
            WebClient web = new WebClient();
            web.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
            web.Proxy = null;
            return web;
        }

        private static Dictionary<string, object> GetDictionary(NameValueCollection values) {
            var result = new Dictionary<string, object>();
            foreach (string key in values.Keys)
                result.Add(key, values[key]);
            return result;
        }

    }

}
