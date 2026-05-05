namespace GameBackend.Core.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string queueName, T eventData) where T : class;
}