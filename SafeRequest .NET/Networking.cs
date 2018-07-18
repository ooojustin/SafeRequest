using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SafeRequest.NET {

    public class SafeRequest {

        public SafeRequest(string encryptionKey) {
            encryption = new Encryption(encryptionKey);
        }

        private Encryption encryption;
        private string USER_AGENT = "SafeRequest .NET";
        public string GetUserAgent() { return USER_AGENT; }
        public void SetUserAgent(string userAgent) { USER_AGENT = userAgent; }

        public Response Request(string url, RequestType type, NameValueCollection values = null) {
            Response response = new Response(type, encryption);
            try {
                WebClient web = GetClient();
                string raw = string.Empty;
                switch (type) {
                    case RequestType.GET:
                        raw = web.DownloadString(url);
                        break;
                    case RequestType.POST:
                        if (values == null)
                            throw new Exception("Missing POST values.");
                        Dictionary<string, object> dictionary = GetDictionary(values);
                        if (dictionary.ContainsKey("authentication_key"))
                            throw new Exception("Key \"authentication_key\" must not be defined.");
                        dictionary.Add("authentication_key", response.POSTAuthentication());
                        string rawPOST = JsonConvert.SerializeObject(dictionary);
                        string POST = encryption.EncryptString(rawPOST);
                        raw = web.UploadString(url, POST);
                        break;
                }
                raw = encryption.DecryptString(raw);
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

    public class Response {

        public Response(RequestType type, Encryption encryption) {
            _type = type;
            if (type == RequestType.POST) {
                _encryption = encryption;
                authenticationKey = Convert.ToString(new Random().Next(10000, 100000));
            }
        }

        // public variables
        public bool status;
        public string message;
        public string raw;
        private Dictionary<string, object> data;

        // public functions
        public T GetData<T>(string key) { return (T)data[key]; }
        public void AddData(string key, object value) { data.Add(key, value); }
        public bool DataExists(string key) { return data.ContainsKey(key); }
        public string POSTAuthentication() { return _encryption.EncryptString(authenticationKey); }

        // private variables
        private RequestType _type;
        private Encryption _encryption;
        private string authenticationKey;

        public void Initialize(string _raw) {
            raw = _raw;
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
            if (_type == RequestType.POST)
                if (!DataExists("authentication_key") || GetData<string>("authentication_key") != authenticationKey)
                    throw new Exception("Response authentication failed.");
            if (DataExists("status") && DataExists("message")) {
                status = GetData<bool>("status");
                message = GetData<string>("message");
            } else throw new Exception("Response is missing required data.");
        }

    }


    public enum RequestType { NONE, GET, POST }

}
