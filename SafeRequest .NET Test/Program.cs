using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeRequest.NET;

namespace SafeRequest.NET_Test {

    class Program {

        static void Main(string[] args) {
            string password = "testing123";
            Encryption enc = new Encryption(password);
            string encrypted = enc.EncryptString("test");
            string decrypted = enc.DecryptString(encrypted);
            Console.WriteLine("encrypted: " + encrypted);
            Console.WriteLine("decrypted: " + decrypted);
            Console.ReadKey();
        }

    }

}
