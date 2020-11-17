using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Text.Json.Extensions.Converters
{
    public class KeyValuePairCollectionConverterFactory : JsonConverterFactory
    {
        public static Type GetValueType(Type typeToConvert)
        {
            if (typeToConvert?.IsAbstract != false
                || typeToConvert.GetConstructor(Array.Empty<Type>()) == null)
            {
                return null;
            }

            Type valueType = null;

            foreach (var @interface in typeToConvert.GetInterfaces())
            {
                if (!@interface.IsGenericType
                    || @interface.GetGenericTypeDefinition() != typeof(ICollection<>))
                {
                    continue;
                }

                var itemType = @interface.GetGenericArguments()[0];
                if (!itemType.IsGenericType
                    || itemType.GetGenericTypeDefinition() != typeof(KeyValuePair<,>))
                {
                    continue;
                }

                var genericArguments = itemType.GetGenericArguments();
                if (genericArguments[0] != typeof(string))
                {
                    continue;
                }

                if (valueType != null && valueType != genericArguments[1])
                {
                    return null;
                }

                valueType = genericArguments[1];
            }

            return valueType;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return GetValueType(typeToConvert) != null;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(
                typeof(KeyValuePairCollectionConverter<,>)
                    .MakeGenericType(GetValueType(typeToConvert), typeToConvert));
        }
    }
}