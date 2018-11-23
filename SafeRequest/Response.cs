using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SafeRequest {

    public class Response {

        public Response(Encryption encryption) {
            _encryption = encryption;
            authenticationKey = Convert.ToString(new Random().Next(10000, 100000));
        }

        // public variables
        public bool status;
        public string message;
        public string raw;
        private Dictionary<string, object> data;

        // public functions
        public T GetData<T>(string key) { return (T)Convert.ChangeType(data[key], typeof(T)); }
        public void AddData(string key, object value) { data.Add(key, value); }
        public bool DataExists(string key) { return data.ContainsKey(key); }
        public string EncryptedAuth() { return _encryption.EncryptString(authenticationKey); }

        // private variables
        private Encryption _encryption;
        private string authenticationKey;

        public void Initialize(string _raw) {
            raw = _raw;
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
            if (!DataExists("authentication_key") || GetData<string>("authentication_key") != authenticationKey)
                throw new Exception("Response authentication failed.");
            if (DataExists("status") && DataExists("message")) {
                status = GetData<bool>("status");
                message = GetData<string>("message").Replace("\\n", Environment.NewLine);
            } else throw new Exception("Response is missing required data.");
        }

    }

}
