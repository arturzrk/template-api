namespace Template.Api.Commands.UpdateSomething.Version1
{
    using Cqs.Commands;
    using Events.SomethingUpdated.Version1;

    public class UpdateSomethingCommand: ICommand<SomethingUpdatedEvent>
    {
        public UpdateSomethingCommand( string somethingId, string name )
        {
            SomethingId = somethingId;
            Name = name;
        }

        public string SomethingId { get; }
        public string Name { get; }
    }
}