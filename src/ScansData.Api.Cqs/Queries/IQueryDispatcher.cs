namespace ScansData.Api.Cqs.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueryDispatcher
    {
        Task<TProjection> DispatchAsync<TProjection>( IQuery<TProjection> query, CancellationToken cancellationToken )
            where TProjection: IProjection;
    }
}