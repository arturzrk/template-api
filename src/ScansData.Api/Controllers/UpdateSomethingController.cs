namespace ScansData.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Commands.UpdateSomething.Version1;
    using Cqs.Commands;
    using Microsoft.AspNetCore.Mvc;
    using Models.UpdateSomething.Version1;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Validation;

    [ ApiVersion( "1" ) ]
    [ Route( "something/{somethingId}/update", Name = "UpdateSomething" ) ]
    public class UpdateSomethingController: Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public UpdateSomethingController( ICommandDispatcher commandDispatcher )
        {
            _commandDispatcher = commandDispatcher;
        }

        [ HttpPost ]
        [ ProducesResponseType( typeof( object ), 204 ) ]
        [ SwaggerOperation( Tags = new[] { "Something" } ) ]
        [ ValidateModelState ]
        public async Task<IActionResult> Index(
            [ FromRoute ] string somethingId,
            [ FromBody ] UpdateSomethingRequest request,
            CancellationToken cancellationToken )
        {
            var command = new UpdateSomethingCommand( somethingId, request.Name );

            await _commandDispatcher.DispatchAsync( command, cancellationToken );

            return StatusCode( 204 );
        }
    }
}