using System.Collections.Generic;
using System.Text.Json.Extensions.Converters;
using Shouldly;
using Xunit;

namespace System.Text.Json.ExtensionsTest.Converters
{
    public class KeyValuePairCollectionConverterFixture
    {
        #region Read

        [Fact]
        public void CanReadToList()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, List<KeyValuePair<string, string>>>();

            var list = jsonConverter.Read(ref reader, typeof(List<KeyValuePair<string, string>>),
                jsonSerializerOptions);
            list.ShouldBe(new[]
            {
                new KeyValuePair<string, string>("Key", "Value")
            });
        }

        [Fact]
        public void CanReadToListWithDuplicateKey()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\", \"Key\": \"OtherValue\"}", jsonSerializerOptions,
                out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, List<KeyValuePair<string, string>>>();

            var list = jsonConverter.Read(ref reader, typeof(List<KeyValuePair<string, string>>),
                jsonSerializerOptions);
            list.ShouldBe(new[]
            {
                new KeyValuePair<string, string>("Key", "Value"),
                new KeyValuePair<string, string>("Key", "OtherValue")
            });
        }

        [Fact]
        public void CanReadToDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            var dictionary = jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                jsonSerializerOptions);
            dictionary.ShouldBe(new[]
            {
                new KeyValuePair<string, string>("Key", "Value")
            }, ignoreOrder: true);
        }

        [Fact]
        public void CanReadToDictionaryWithCustomComparator()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>(
                () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

            var dictionary = jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                jsonSerializerOptions);
            dictionary.ShouldSatisfyAllConditions(
                () => dictionary.ShouldBe(new[]
                {
                    new KeyValuePair<string, string>("Key", "Value")
                }, ignoreOrder: true),
                () => dictionary.Comparer.ShouldBeSameAs(StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void CanReadNull()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("null", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                    jsonSerializerOptions)
                .ShouldBeNull();
        }

        [Fact]
        public void CanReadEmptyObject()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{}", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                    jsonSerializerOptions)
                .ShouldBeEmpty();
        }

        [Fact]
        public void ReadRejectsArray()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            Should.Throw<JsonException>(
                () =>
                {
                    Utf8JsonReader reader;
                    try
                    {
                        TestHelper.GetReader("[]", jsonSerializerOptions, out reader);
                    }
                    catch (JsonException e)
                    {
                        e.ShouldBeNull("Helper failed");
                        throw;
                    }

                    jsonConverter.Read(ref reader, typeof(Dictionary<string, string>), jsonSerializerOptions);
                });
        }

        [Fact]
        public void ReadRejectsUnfinishedObject()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            Should.Throw<JsonException>(
                () =>
                {
                    Utf8JsonReader reader;
                    try
                    {
                        TestHelper.GetReader("{\"Key\": \"Value\"", jsonSerializerOptions, out reader);
                    }
                    catch (JsonException e)
                    {
                        e.ShouldBeNull("Helper failed");
                        throw;
                    }

                    jsonConverter.Read(ref reader, typeof(Dictionary<string, string>), jsonSerializerOptions);
                });
        }

        [Fact]
        public void ReadCanSkipComments()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            TestHelper.GetReader("{/*Comment*/\"Key\":/*Comment*/\"Value\"/*Comment*/}", jsonSerializerOptions,
                out var reader, JsonCommentHandling.Allow);

            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            var dictionary = jsonConverter.Read(ref reader, typeof(Dictionary<string, string>),
                jsonSerializerOptions);
            dictionary.ShouldBe(new[]
            {
                new KeyValuePair<string, string>("Key", "Value")
            }, ignoreOrder: true);
        }

        #endregion

        #region Write

        [Fact]
        public void CanWriteList()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, List<KeyValuePair<string, string>>>();

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
        public void CanWriteListWithDuplicateKey()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, List<KeyValuePair<string, string>>>();

            var json = TestHelper.WithWriter(writer =>
                {
                    jsonConverter.Write(writer, new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Key", "Value"),
                            new KeyValuePair<string, string>("Key", "OtherValue")
                        },
                        jsonSerializerOptions);
                },
                jsonSerializerOptions);

            json.ShouldBe("{\"Key\":\"Value\",\"Key\":\"OtherValue\"}");
        }

        [Fact]
        public void CanWriteDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

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

        [Fact]
        public void CanWriteNullList()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, List<KeyValuePair<string, string>>>();

            var json = TestHelper.WithWriter(writer => { jsonConverter.Write(writer, null, jsonSerializerOptions); },
                jsonSerializerOptions);

            json.ShouldBe("null");
        }

        [Fact]
        public void CanWriteNullDictionary()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();

            var json = TestHelper.WithWriter(writer => { jsonConverter.Write(writer, null, jsonSerializerOptions); },
                jsonSerializerOptions);

            json.ShouldBe("null");
        }

        [Fact]
        public void WriteRequiresWriter()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            var jsonConverter = new KeyValuePairCollectionConverter<string, Dictionary<string, string>>();
            Should.Throw<ArgumentNullException>(
                () => { jsonConverter.Write(null, null, jsonSerializerOptions); });
        }

        #endregion

        #region Interface

        [Fact]
        public void DefaultConstructorThrowsWhenUsingInterface()
        {
            Should.Throw<ArgumentException>(() =>
                new KeyValuePairCollectionConverter<string, ICollection<KeyValuePair<string, string>>>());
        }

        #endregion

        #region AbstractCollection

        [Fact]
        public void DefaultConstructorThrowsWhenUsingAbstractClass()
        {
            Should.Throw<ArgumentException>(() =>
                new KeyValuePairCollectionConverter<string, TestHelper.AbstractCollection>());
        }

        #endregion

        #region ListWithoutDefaultConstructor

        [Fact]
        public void DefaultConstructorThrowsWhenUsingListWithoutDefaultConstructor()
        {
            Should.Throw<ArgumentException>(() =>
                new KeyValuePairCollectionConverter<string, TestHelper.ListWithoutDefaultConstructor>());
        }

        [Fact]
        public void CanReadToListWithoutDefaultConstructor()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            TestHelper.GetReader("{\"Key\": \"Value\"}", jsonSerializerOptions, out var reader);

            var jsonConverter = new KeyValuePairCollectionConverter<string, TestHelper.ListWithoutDefaultConstructor>(
                () => new TestHelper.ListWithoutDefaultConstructor(1));

            var list = jsonConverter.Read(ref reader, typeof(TestHelper.ListWithoutDefaultConstructor),
                jsonSerializerOptions);
            list.ShouldBe(new[]
            {
                new KeyValuePair<string, string>("Key", "Value")
            });
        }

        #endregion
    }
}