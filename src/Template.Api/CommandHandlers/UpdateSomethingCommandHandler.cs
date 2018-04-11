namespace Template.Api.CommandHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Commands.UpdateSomething.Version1;
    using Cqs.Commands;
    using Events.SomethingUpdated.Version1;

    public class UpdateSomethingCommandHandler: ICommandHandler<UpdateSomethingCommand, SomethingUpdatedEvent>
    {
        public async Task<SomethingUpdatedEvent> HandleAsync( UpdateSomethingCommand command, CancellationToken cancellationToken )
        {
            // do something

            return new SomethingUpdatedEvent( command.SomethingId );
        }
    }
}