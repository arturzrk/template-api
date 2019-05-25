namespace ScansData.Api.EventHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cqs.Events;
    using Events.SomethingUpdated.Version1;

    public class SomethingUpdatedEventHandler: IEventHandler<SomethingUpdatedEvent>
    {
        public async Task HandleAsync( SomethingUpdatedEvent @event, CancellationToken cancellationToken )
        {
            // do something
        }
    }
}