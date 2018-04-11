namespace Template.Api.Common.Swashbuckle.AspNetCore.SwaggerGen
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Microsoft.AspNetCore.Authorization;
    using global::Swashbuckle.AspNetCore.Swagger;
    using global::Swashbuckle.AspNetCore.SwaggerGen;

    public class SubHeaderOperationFilter: IOperationFilter
    {
        public void Apply( Operation operation, OperationFilterContext context )
        {
            if( context.ApiDescription.ControllerAttributes().OfType<AllowAnonymousAttribute>().Any() )
            {
                return;
            }

            if( operation.Parameters == null )
            {
                operation.Parameters = new List<IParameter>();
            }

            operation.Parameters.Add( new NonBodyParameter
            {
                Name = "X-Sub",
                In = "header",
                Type = "string",
                Required = false
            } );

            if( operation.Responses.ContainsKey( "401" ) )
            {
                operation.Responses.Add( "401", new Response { Description = "Unauthorized" } );
            }

            if( operation.Responses.ContainsKey( "403" ) )
            {
                operation.Responses.Add( "403", new Response { Description = "Forbidden" } );
            }
        }
    }
}