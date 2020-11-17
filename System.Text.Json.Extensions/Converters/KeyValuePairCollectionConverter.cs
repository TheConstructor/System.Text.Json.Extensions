using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace System.Text.Json.Extensions.Converters
{
    public class KeyValuePairCollectionConverter<TValue, TCollection>
        : JsonConverter<TCollection>
        where TCollection : ICollection<KeyValuePair<string, TValue>>
    {
        private readonly Func<TCollection> _provider;

        public KeyValuePairCollectionConverter()
        {
            var collectionType = typeof(TCollection);
            if (collectionType.IsAbstract)
            {
                throw new ArgumentException(
                    $"{collectionType} is abstract. Please specify a provider-Func.",
                    nameof(TCollection));
            }

            var constructorInfo = collectionType.GetConstructor(Array.Empty<Type>());
            if (constructorInfo == null)
            {
                throw new ArgumentException(
                    $"{collectionType} has no parameterless constructor. Please specify a provider-Func.",
                    nameof(TCollection));
            }

            _provider = Expression
                .Lambda<Func<TCollection>>(Expression.New(constructorInfo), Array.Empty<ParameterExpression>())
                .Compile();
        }

        public KeyValuePairCollectionConverter(Func<TCollection> provider)
        {
            _provider = provider;
        }

        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            reader.Read();

            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw reader.GetException($"Expected StartObject but found {reader.TokenType}");
            }

            var collection = _provider();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return collection;
                }

                if (reader.TokenType == JsonTokenType.Comment
                    && options?.ReadCommentHandling != JsonCommentHandling.Disallow)
                {
                    continue;
                }

                var propertyName = reader.GetString();
                var propertyValue = reader.ReadObject<TValue>(options);
                collection.Add(new KeyValuePair<string, TValue>(propertyName, propertyValue));
            }

            throw reader.GetException($"Expected EndObject but JSON ended");
        }

        public override void Write(Utf8JsonWriter writer, TCollection collection, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (collection == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            foreach (var (key, value) in collection)
            {
                writer.WriteObject(key, value, options);
            }

            writer.WriteEndObject();
        }
    }
}