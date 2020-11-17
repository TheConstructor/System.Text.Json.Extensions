using System.Collections.Generic;
using System.Text.Json.Extensions;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest
{
    public class Utf8JsonReaderExtensionsFixture
    {
        [Fact]
        public void ReadObjectReadsDummyObject()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"MyValue\": \"MyValue\"}", jsonSerializerOptions, out var reader);

            var dummyObject = reader.ReadObject<TestHelper.DummyObject>(jsonSerializerOptions);

            dummyObject.MyValue
                .ShouldBe("MyValue");
        }

        [Fact]
        public void ReadObjectsReadsDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var keyValuePair = reader.ReadObject<Dictionary<string, string>>(jsonSerializerOptions)
                .ShouldHaveSingleItem();
            keyValuePair.ShouldSatisfyAllConditions(
                () => keyValuePair.Key.ShouldBe("Key"),
                () => keyValuePair.Value.ShouldBe("Value"));
        }

        [Fact]
        public void ReadObjectsReadsString()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("\"MyString\"", jsonSerializerOptions, out var reader);

            reader.ReadObject<string>(jsonSerializerOptions)
                .ShouldBe("MyString");
        }

        [Fact]
        public void ReadObjectsReadsInt()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("42", jsonSerializerOptions, out var reader);

            reader.ReadObject<int>(jsonSerializerOptions)
                .ShouldBe(42);
        }
    }
}