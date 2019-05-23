namespace Template.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [ AllowAnonymous ]
    [ ApiVersion( "1" ) ]
    [ Route( "ping", Name = "Ping" ) ]
    public class PingController: Controller
    {
        private readonly string _secret;

        public PingController()
        {
            // TODO: from appsettings
            _secret = "7929e345-46b9-49d9-a5ed-1412bdb7deba";
        }

        [ HttpGet ]
        [ ProducesResponseType( typeof( object ), 200 ) ]
        [ SwaggerOperation( Tags = new[] { "Health" } ) ]
        public IActionResult Ping( [ FromQuery ] string secret )
        {
            if( string.IsNullOrWhiteSpace( _secret ) || secret != _secret )
            {
                return NotFound();
            }

            return StatusCode( 200 );
        }
    }
}