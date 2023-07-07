using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;
    public class FindPostsWithLikesQueries : BaseQuery
    {
        public int NumberOfLikes { get; set; }
    }
