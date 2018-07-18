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

            Response response;

            // POST example
            NameValueCollection values = new NameValueCollection();
            values["some_key"] = "some_value";
            response = Networking.Request(URL, ENCRYPTION_KEY, RequestType.POST, values);
            Console.WriteLine(response.message);
            Console.ReadKey();

            // GET example
            response = Networking.Request(URL, ENCRYPTION_KEY, RequestType.GET);
            Console.WriteLine(response.message);
            Console.ReadKey();

        }

    }

}
