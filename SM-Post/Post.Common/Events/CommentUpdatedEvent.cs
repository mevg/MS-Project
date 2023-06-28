using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentUpdatedEvent : BaseEvent
{
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent)) { }
    public Guid CommentId { get; set; }
    public required string Comment { get; set; }
    public required string Author { get; set; }
    public DateTime EditDate { get; set; }
}
