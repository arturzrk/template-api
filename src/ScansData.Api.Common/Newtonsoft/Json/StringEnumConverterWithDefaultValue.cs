namespace ScansData.Api.Common.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Converters;

    public class StringEnumConverterWithDefaultValue: StringEnumConverter
    {
        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            try
            {
                return base.ReadJson( reader, objectType, existingValue, serializer );
            }
            catch( JsonSerializationException )
            {
                return existingValue;
            }
        }
    }
}