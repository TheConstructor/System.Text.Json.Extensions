using System.Text.Json.Extensions;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest
{
    public class Utf8JsonReaderExtensionsFixture
    {
        [Fact]
        public void ReadObjectReadDummyObject()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"MyValue\": \"MyValue\"}", jsonSerializerOptions, out var reader);

            var dummyObject = reader.ReadObject<TestHelper.DummyObject>(jsonSerializerOptions);

            dummyObject.MyValue
                .ShouldBe("MyValue");
        }
    }
}