using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeRequest.NET;

namespace SafeRequest.NET_Test {

    class Program {

        private const string URL = "https://example.com/example.php";
        private const string ENCRYPTION_KEY = "your encryption key here";

        static void Main(string[] args) {

            NET.SafeRequest safeRequest = new NET.SafeRequest(ENCRYPTION_KEY);
            Response response;

            // POST example
            NameValueCollection values = new NameValueCollection();
            values["some_key"] = "some_value";
            response = safeRequest.Request(URL, RequestType.POST, values);
            Console.WriteLine(response.message);
            Console.ReadKey();

            // GET example
            response = safeRequest.Request(URL, RequestType.GET);
            Console.WriteLine(response.message);
            Console.ReadKey();

        }

    }

}
