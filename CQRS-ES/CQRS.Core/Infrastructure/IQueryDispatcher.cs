using CQRS.Core.Queries;

namespace CQRS.Core.Infrastructure;
public interface IQueryDispatcher<TEntity>
{
    void RegisterHAndler<TQuery>(Func<TQuery,Task<List<TEntity>>> handler) where TQuery : BaseQuery;
    Task<List<TEntity>> SendAsync(BaseQuery query);
}
