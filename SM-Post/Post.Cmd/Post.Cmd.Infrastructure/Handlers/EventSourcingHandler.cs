using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(aggregateId);

        if(events is null || !events.Any()){
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(e => e.Version).Max();
        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await _eventStore.GetAggregateIdsAsync();
        if (aggregateIds is null || !aggregateIds.Any()) return;
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
        var tasks = new List<Task>();
        foreach (var id in aggregateIds)
        {
            var aggregate = await GetByIdAsync(id);
            if(aggregate is null || !aggregate.Active) continue;
            var events = await _eventStore.GetEventsAsync(id);
            foreach (var @event in events)
            {
                tasks.Add(_eventProducer.ProduceAsync(topic, @event));
            }
        }
        await Task.WhenAll(tasks);
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommitted();
    }
}
