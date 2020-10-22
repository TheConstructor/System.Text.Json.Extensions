using System.Collections.Generic;
using System.Text.Json.Extensions.Converters;
using System.Text.Json.Serialization;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest.Converters
{
    public class KeyValuePairCollectionConverterFactoryFixture
    {
        [Theory]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(IEnumerable<KeyValuePair<string, string>>), false)]
        [InlineData(typeof(KeyValuePair<string, string>), false)]
        [InlineData(typeof(List<KeyValuePair<string, string>>), true)]
        [InlineData(typeof(List<KeyValuePair<int, string>>), false)]
        [InlineData(typeof(List<KeyValuePair<string, int>>), true)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(Dictionary<int, string>), false)]
        public void CanConvertReportsCorrectly(Type typeToConvert, bool canConvert)
        {
            var factory = new KeyValuePairCollectionConverterFactory();
            factory.CanConvert(typeToConvert)
                .ShouldBe(canConvert);
        }

        [Fact]
        public void CanReadToList()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var factory = new KeyValuePairCollectionConverterFactory();
            var jsonConverter = factory
                .CreateConverter(typeof(List<KeyValuePair<string, string>>), jsonSerializerOptions)
                .ShouldBeAssignableTo<JsonConverter<List<KeyValuePair<string, string>>>>();

            var keyValuePair = jsonConverter.Read(ref reader, typeof(List<KeyValuePair<string, string>>),
                    jsonSerializerOptions)
                .ShouldHaveSingleItem();
            keyValuePair.ShouldSatisfyAllConditions(
                () => keyValuePair.Key.ShouldBe("Key"),
                () => keyValuePair.Value.ShouldBe("Value"));
        }

        [Fact]
        public void CanReadToDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var factory = new KeyValuePairCollectionConverterFactory();
            var jsonConverter = factory
                .CreateConverter(typeof(Dictionary<string, string>), jsonSerializerOptions)
                .ShouldBeAssignableTo<JsonConverter<Dictionary<string, string>>>();

            var keyValuePair = jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                    jsonSerializerOptions)
                .ShouldHaveSingleItem();
            keyValuePair.ShouldSatisfyAllConditions(
                () => keyValuePair.Key.ShouldBe("Key"),
                () => keyValuePair.Value.ShouldBe("Value"));
        }

        [Fact]
        public void CanWriteList()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();

            var factory = new KeyValuePairCollectionConverterFactory();
            var jsonConverter = factory
                .CreateConverter(typeof(List<KeyValuePair<string, string>>), jsonSerializerOptions)
                .ShouldBeAssignableTo<JsonConverter<List<KeyValuePair<string, string>>>>();

            var json = TestHelper.WithWriter(writer =>
                {
                    jsonConverter.Write(writer, new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Key", "Value")
                        },
                        jsonSerializerOptions);
                },
                jsonSerializerOptions);

            json.ShouldBe("{\"Key\":\"Value\"}");
        }

        [Fact]
        public void CanWriteDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();

            var factory = new KeyValuePairCollectionConverterFactory();
            var jsonConverter = factory
                .CreateConverter(typeof(Dictionary<string, string>), jsonSerializerOptions)
                .ShouldBeAssignableTo<JsonConverter<Dictionary<string, string>>>();

            var json = TestHelper.WithWriter(writer =>
                {
                    jsonConverter.Write(writer, new Dictionary<string, string>
                        {
                            {"Key", "Value"}
                        },
                        jsonSerializerOptions);
                },
                jsonSerializerOptions);

            json.ShouldBe("{\"Key\":\"Value\"}");
        }
    }
}