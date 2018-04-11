namespace Template.Api.Events.SomethingUpdated.Version1
{
    using Cqs.Events;

    public class SomethingUpdatedEvent: IEvent
    {
        public SomethingUpdatedEvent( string somethingId )
        {
            SomethingId = somethingId;
        }

        public string SomethingId { get; }
    }
}