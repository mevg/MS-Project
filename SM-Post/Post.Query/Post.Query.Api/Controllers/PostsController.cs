using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Dtos;
using Post.Query.Api.Dtos;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

    public PostsController(ILogger<PostsController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostQuery());
            return NormalResponse(posts);
        }
        catch (Exception e)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts!";
            return ErrorResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdPostAsync(Guid id)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery() { Id = id});
            return NormalResponse(posts);
        }
        catch (Exception e)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find a post by id!";
            return ErrorResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("comments")]
    public async Task<IActionResult> GetPostByAuthorAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception e)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts with comments!";
            return ErrorResponse(e, SAFE_ERROR_MESSAGE);
        }
    }


    [HttpGet("author/{author}")]
    public async Task<IActionResult> GetPostByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery() { Author = author});
            return NormalResponse(posts);
        }
        catch (Exception e)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts by author!";
            return ErrorResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("likes/{numberOfLikes}")]
    public async Task<IActionResult> GetPostByLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQueries() { NumberOfLikes = numberOfLikes});
            return NormalResponse(posts);
        }
        catch (Exception e)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find posts with likes!";
            return ErrorResponse(e, SAFE_ERROR_MESSAGE);
        }
    }

    private IActionResult NormalResponse(List<PostEntity>? posts)
    {
        if (posts is null || !posts.Any())
        {
            return NoContent();
        }
        var count = posts.Count;
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}"
        });
    }
    private IActionResult ErrorResponse(Exception e, string safeErrorMessage)
    {
        _logger.LogError(e, safeErrorMessage);
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        });
    }
}
