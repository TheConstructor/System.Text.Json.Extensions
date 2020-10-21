using System.IO;
using System.Text.Json.Extensions;

namespace System.Text.Json.ExtensionsTest
{
    public static class TestHelper
    {
        public static void GetReader(string json, JsonSerializerOptions options, out Utf8JsonReader reader)
        {
            var jsonReaderOptions = options.GetReaderOptions();
            var utf8Bytes = Encoding.UTF8.GetBytes(json);
            reader = new Utf8JsonReader(utf8Bytes, jsonReaderOptions);
        }

        public static string WithWriter(Action<Utf8JsonWriter> action, JsonSerializerOptions options, bool skipValidation = true)
        {
            using var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer, options.GetWriterOptions(skipValidation)))
            {
                action(writer);
                writer.Flush();
            }

            return Encoding.UTF8.GetString(buffer.GetBuffer().AsSpan(0, (int) buffer.Length));
        }

        public class DummyObject
        {
            public string MyValue { get; set; }
        }
    }
}