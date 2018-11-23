using System;
using System.Collections.Specialized;

namespace SafeRequest.Test {

    class Program {

        private const string URL = "https://example.com/example.php";
        private const string ENCRYPTION_KEY = "your encryption key here";

        static void Main(string[] args) {

            SafeRequest safeRequest = new SafeRequest(ENCRYPTION_KEY);
            Response response;

            // POST example
            NameValueCollection values = new NameValueCollection();
            values["some_key"] = "some_value";
            response = safeRequest.Request(URL, values);
            Console.WriteLine(response.message);
            Console.ReadKey();

            // GET example
            // Note: Requests with null values still post an authentication key to verify the validity of the request.
            response = safeRequest.Request(URL);
            Console.WriteLine(response.message);
            Console.ReadKey();

        }

    }

}
