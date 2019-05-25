namespace ScansData.Api.QueryHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cqs.Queries;
    using Queries.FindSomething.Version1;

    public class FindSomethingQueryHandler: IQueryHandler<FindSomethingQuery, FindSomethingProjection>
    {
        public async Task<FindSomethingProjection> HandleAsync( FindSomethingQuery query, CancellationToken cancellationToken )
        {
            return new FindSomethingProjection( query.SomethingId, "Foo Bar" );
        }
    }
}