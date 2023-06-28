using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommandId { get; set; }
    public required string Username { get; set; }
}
