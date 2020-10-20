using System.Text.Json.Extensions;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest
{
    public class Utf8JsonReaderExtensionsFixture
    {
        private class DummyObject
        {
            public string MyValue { get; set; }
        }
        
        [Fact]
        public void ReadObjectReadDummyObject()
        {
            var jsonReaderOptions = new JsonReaderOptions();
            var jsonSerializerOptions = new JsonSerializerOptions();
            var utf8Bytes = Encoding.UTF8.GetBytes("{\"MyValue\": \"MyValue\"}");
            var reader = new Utf8JsonReader(utf8Bytes, jsonReaderOptions);

            var dummyObject = reader.ReadObject<DummyObject>(jsonSerializerOptions);
            
            dummyObject.MyValue
                .ShouldBe("MyValue");
        }
    }
}