using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameBackend.Core.Interfaces;

namespace GameBackend.Infrastructure.Messaging;

public class ServiceBusEventPublisher : IEventPublisher
{
    private readonly ServiceBusClient _client;

    public ServiceBusEventPublisher(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync<T>(string queueName, T eventData) where T : class
    {
        var sender = _client.CreateSender(queueName);
        var json = JsonSerializer.Serialize(eventData);
        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            Subject = typeof(T).Name
        };
        await sender.SendMessageAsync(message);
    }
}