using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
    {
        _config = config.Value;
        _eventHandler = eventHandler;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
                                    .SetKeyDeserializer(Deserializers.Utf8)
                                    .SetValueDeserializer(Deserializers.Utf8)
                                    .Build();
        consumer.Subscribe(topic);
        while (true)
        {
            var consumeResult = consumer.Consume();
            if (consumeResult?.Message is null) continue;

            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
            var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
            if (handleMethod is null)
            {
                throw new ArgumentNullException(nameof(handleMethod), "could not find event handler methos!");
            }
            handleMethod.Invoke(_eventHandler, new object[] { @event });
            consumer.Commit(consumeResult);
        }
    }
}
