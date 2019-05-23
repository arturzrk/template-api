namespace Template.Api.Authentication
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SubHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public SubHeaderMiddleware( RequestDelegate next )
        {
            _next = next;
        }

        public async Task Invoke( HttpContext context )
        {
            context.Request.Headers.TryGetValue( "X-Sub", out var headerSub );
            var sub = headerSub.FirstOrDefault();

            if( string.IsNullOrEmpty( sub ) )
            {
                await _next( context );

                return;
            }

            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Email, "test@razor.co.uk" ),
                new Claim( ClaimTypes.Name, "Test" ),
                new Claim( "sub", sub )
            };
            var claimsIdentity = new ClaimsIdentity( claims, "Test" );
            var claimsPrincipal = new ClaimsPrincipal( claimsIdentity );
            context.User = claimsPrincipal;

            await _next( context );
        }
    }
}