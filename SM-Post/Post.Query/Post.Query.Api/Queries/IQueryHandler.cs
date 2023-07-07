
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;
public interface IQueryHandler
{
    Task<List<PostEntity>> HandleAsync(FindAllPostQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQueries query);
    Task<List<PostEntity>> HandleAsync(FindPostWithCommentsQuery query);
}
