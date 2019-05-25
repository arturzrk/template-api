namespace ScansData.Api.Queries.FindSomething.Version1
{
    using Cqs.Queries;

    public class FindSomethingQuery : IQuery<FindSomethingProjection>
    {
        public FindSomethingQuery( string somethingId )
        {
            SomethingId = somethingId;
        }

        public string SomethingId { get; }
    }
}