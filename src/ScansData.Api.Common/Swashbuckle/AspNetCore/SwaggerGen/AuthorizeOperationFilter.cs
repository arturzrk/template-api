namespace ScansData.Api.Common.Swashbuckle.AspNetCore.SwaggerGen
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Microsoft.AspNetCore.Authorization;
    using global::Swashbuckle.AspNetCore.Swagger;
    using global::Swashbuckle.AspNetCore.SwaggerGen;

    public class AuthorizeOperationFilter: IOperationFilter
    {
        private readonly string _scheme;

        public AuthorizeOperationFilter( string scheme )
        {
            _scheme = scheme;
        }

        public void Apply( Operation operation, OperationFilterContext context )
        {
            if( context.ApiDescription.ControllerAttributes().OfType<AllowAnonymousAttribute>().Any()
                || context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>().Any() )
            {
                return;
            }

            if( operation.Responses.ContainsKey( "401" ) )
            {
                operation.Responses.Add( "401", new Response { Description = "Unauthorized" } );
            }

            if( operation.Responses.ContainsKey( "403" ) )
            {
                operation.Responses.Add( "403", new Response { Description = "Forbidden" } );
            }

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new[] { _scheme } }
                }
            };
        }
    }
}