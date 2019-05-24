using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SafeRequest
{
    public class Response
    {
        private Dictionary<string, object> _data;

        public string Message { get; set; }
        public bool Status { get; set; }

        public void AddData(string key, object value)
        {
            _data.Add(key, value);
        }

        public bool DataExists(string key)
        {
            return _data.ContainsKey(key);
        }

        public T GetData<T>(string key)
        {
            return (T)Convert.ChangeType(_data[key], typeof(T));
        }

        public void Initialize(string rawData, string authKey)
        {
            _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawData);

            if (!DataExists("authentication_key") || GetData<string>("authentication_key") != authKey)
                throw new InvalidOperationException("Response authentication failed.");

            if (DataExists("status") && DataExists("message"))
            {
                Status = GetData<bool>("status");
                Message = GetData<string>("message").Replace("\\n", Environment.NewLine);
            }

            throw new InvalidOperationException("Response is missing required data.");
        }
    }
}