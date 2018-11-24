using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security;

namespace SafeRequest {

    public class SafeRequest {

        public SafeRequest(string key, byte[] iv = null) {
            encryption = new Encryption(key);
            SetIV(iv);
        }

        private void SetIV(byte[] iv) {
            if (iv != null)
                if (iv.Count() == 16)
                    encryption.SetIV(iv);
        }

        public string UserAgent = "SafeRequest";
        public bool NullifyProxy = true;

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

        public WebClient GetClient() {

            WebClient web = new WebClient();

            if (!string.IsNullOrEmpty(UserAgent))
                web.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (NullifyProxy)
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
