using CQRS.Core.Messages;

namespace CQRS.Core.Events;

public class BaseEvent : Message
{
    protected BaseEvent(string type)
    {
        Type = type;
    }
    public int Version { get; set; }
    public required string Type { get; set; }
}
