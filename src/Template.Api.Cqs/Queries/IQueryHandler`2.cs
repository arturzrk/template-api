namespace Template.Api.Cqs.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueryHandler<in TQuery, TProjection>
        where TQuery: IQuery<TProjection>
        where TProjection: IProjection
    {
        Task<TProjection> HandleAsync( TQuery query, CancellationToken cancellationToken );
    }
}