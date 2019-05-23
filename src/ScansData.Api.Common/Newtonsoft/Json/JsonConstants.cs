namespace Template.Api.Common.Newtonsoft.Json
{
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Serialization;

    public static class JsonConstants
    {
        static JsonConstants()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Populate,
                Formatting = Formatting.Indented
            };
            JsonSerializerSettings.Converters.Add( new StringEnumConverterWithDefaultValue() );
        }

        public static JsonSerializerSettings JsonSerializerSettings { get; }
    }
}