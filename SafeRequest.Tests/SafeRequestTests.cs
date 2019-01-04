using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace SafeRequest.Tests
{
    [TestClass]
    public class SafeRequestTests
    {
        private const string URL = "https://example.com/example.php";
        private const string ENCRYPTION_KEY = "your encryption key here";

        [TestMethod]
        public void SafeRequestEncryptDecrypt()
        {
            var safeRequest = new SafeRequest(ENCRYPTION_KEY);
            Response response;

            // POST example
            var values = new NameValueCollection();
            values["some_key"] = "some_value";
            response = safeRequest.Request(URL, values);
            var originalMessage = response.Message;

            // GET example
            // Note: Requests with null values still post an authentication key to verify the validity of the request.
            response = safeRequest.Request(URL);
            var newMessage = response.Message;

            Assert.IsNotNull(newMessage);
            Assert.AreEqual(originalMessage, newMessage);
        }
    }
}