namespace Template.Api.Versioning
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Versioning;

    public class AcceptHeaderApiVersionReader: IApiVersionReader
    {
        private static readonly Regex versionRegex = new Regex( @"^application\/vnd\.Template.Api.v(\d+)\+json$" );

        public string Read( HttpRequest request )
        {
            var acceptHeader = request.Headers.ContainsKey( "Accept" ) ? request.Headers["Accept"].First() : "";
            var match = versionRegex.Match( acceptHeader );

            if( !match.Success )
            {
                return null;
            }

            return !int.TryParse( match.Groups[1].Value, out var version ) ? null : version.ToString();
        }

        public void AddParameters( IApiVersionParameterDescriptionContext context ) { }
    }
}