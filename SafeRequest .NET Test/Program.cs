using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeRequest.NET;

namespace SafeRequest.NET_Test {

    class Program {

        static void Main(string[] args) {
            Response resp = Networking.Request("https://example.com/example.php", "your encryption key here", RequestType.GET);
            Console.WriteLine(resp.message);
            Console.ReadKey();
        }

    }

}
