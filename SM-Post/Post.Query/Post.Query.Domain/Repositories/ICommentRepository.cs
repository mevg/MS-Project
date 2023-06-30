using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface ICommentRepository
{
    Task CreateAsync(CommentEntity coment);
    Task UpdateAsync(CommentEntity coment);
    Task<CommentEntity> GetByIdAsync(Guid commentId);
    Task DeleteAsync(Guid commentId);
}
