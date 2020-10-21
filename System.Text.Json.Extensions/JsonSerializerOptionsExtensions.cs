namespace System.Text.Json.Extensions
{
    public static class JsonSerializerOptionsExtensions
    {
        public static JsonReaderOptions GetReaderOptions(this JsonSerializerOptions jsonSerializerOptions)
        {
            if (jsonSerializerOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonSerializerOptions));
            }

            return new JsonReaderOptions
            {
                CommentHandling = jsonSerializerOptions.ReadCommentHandling,
                AllowTrailingCommas = jsonSerializerOptions.AllowTrailingCommas,
                MaxDepth = jsonSerializerOptions.MaxDepth
            };
        }

        public static JsonWriterOptions GetWriterOptions(this JsonSerializerOptions jsonSerializerOptions,
            bool skipValidation = true)
        {
            if (jsonSerializerOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonSerializerOptions));
            }

            return new JsonWriterOptions
            {
                Encoder = jsonSerializerOptions.Encoder,
                Indented = jsonSerializerOptions.WriteIndented,
                SkipValidation = skipValidation
            };
        }
    }
}