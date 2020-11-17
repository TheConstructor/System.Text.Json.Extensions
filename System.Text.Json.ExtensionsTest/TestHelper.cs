using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Extensions;

namespace System.Text.Json.ExtensionsTest
{
    public static class TestHelper
    {
        public static void GetReader(string json, JsonSerializerOptions options, out Utf8JsonReader reader,
            JsonCommentHandling? jsonCommentHandling = null)
        {
            var jsonReaderOptions = options.GetReaderOptions(jsonCommentHandling);
            var utf8Bytes = Encoding.UTF8.GetBytes(json);
            reader = new Utf8JsonReader(utf8Bytes, jsonReaderOptions);
        }

        public static string WithWriter(Action<Utf8JsonWriter> action, JsonSerializerOptions options,
            bool skipValidation = true)
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

        public abstract class AbstractCollection : ICollection<KeyValuePair<string, string>>
        {
            public AbstractCollection()
            {
            }

            public abstract IEnumerator<KeyValuePair<string, string>> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public abstract void Add(KeyValuePair<string, string> item);
            public abstract void Clear();
            public abstract bool Contains(KeyValuePair<string, string> item);
            public abstract void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex);
            public abstract bool Remove(KeyValuePair<string, string> item);
            public abstract int Count { get; }
            public abstract bool IsReadOnly { get; }
        }

        public class ListWithoutDefaultConstructor : List<KeyValuePair<string, string>>
        {
            public ListWithoutDefaultConstructor(int capacity) : base(capacity)
            {
            }
        }
    }
}