namespace ScansData.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cqs.Queries;
    using Microsoft.AspNetCore.Mvc;
    using Queries.FindSomething.Version1;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [ ApiVersion( "1" ) ]
    [ Route( "something/{somethingId}/find", Name = "FindSomething" ) ]
    public class FindSomethingController: Controller
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public FindSomethingController( IQueryDispatcher queryDispatcher )
        {
            _queryDispatcher = queryDispatcher;
        }

        [ HttpGet ]
        [ ProducesResponseType( typeof( FindSomethingProjection ), 200 ) ]
        [ SwaggerOperation( Tags = new[] { "Something" } ) ]
        public async Task<IActionResult> Index(
            [ FromRoute ] string somethingId,
            CancellationToken cancellationToken )
        {
            var query = new FindSomethingQuery( somethingId );

            var projection = await _queryDispatcher.DispatchAsync( query, cancellationToken );

            return StatusCode( 200, projection );
        }
    }
}