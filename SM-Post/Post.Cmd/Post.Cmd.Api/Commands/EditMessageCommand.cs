using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class EdirMessageCommand : BaseCommand
{
    public required string Message { get; set; }
}
