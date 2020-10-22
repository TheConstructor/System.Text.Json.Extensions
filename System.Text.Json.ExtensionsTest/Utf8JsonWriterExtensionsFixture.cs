using System.Text.Json.Extensions;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest
{
    public class Utf8JsonWriterExtensionsFixture
    {
        [Fact]
        public void WriteObjectWritesDummyObject()
        {
            var dummyObject = new TestHelper.DummyObject
            {
                MyValue = "TestValue"
            };
            var jsonSerializerOptions = new JsonSerializerOptions();

            var json = TestHelper.WithWriter(writer => { writer.WriteObject(dummyObject, jsonSerializerOptions); },
                jsonSerializerOptions);

            json.ShouldBe("{\"MyValue\":\"TestValue\"}");
        }
    }
}