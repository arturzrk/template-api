namespace ScansData.Api.Cqs.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Events;

    public interface ICommandDispatcher
    {
        Task<TEvent> DispatchAsync<TEvent>(ICommand<TEvent> command, CancellationToken cancellationToken)
            where TEvent : IEvent;
    }
}