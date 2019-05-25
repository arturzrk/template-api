namespace ScansData.Api.Cqs.Commands
{
    using Events;

    public interface ICommand<TEvent>
        where TEvent: IEvent { }
}