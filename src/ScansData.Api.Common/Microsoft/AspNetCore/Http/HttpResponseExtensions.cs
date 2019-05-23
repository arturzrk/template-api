namespace Template.Api.Common.Microsoft.AspNetCore.Http
{
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Http;
    using global::Newtonsoft.Json;

    public static class HttpResponseExtensions
    {
        public static async Task WriteJsonAsync( this HttpResponse response, HttpStatusCode statusCode, object value, JsonSerializerSettings jsonSerializerSettings )
        {
            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            var text = JsonConvert.SerializeObject( value, Formatting.Indented, jsonSerializerSettings );

            await response.WriteAsync( text, Encoding.UTF8 );
        }
    }
}