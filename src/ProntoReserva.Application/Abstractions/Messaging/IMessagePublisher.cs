namespace ProntoReserva.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T evento) where T : class;
    
    Task PublishWithDelayAsync<T>(T evento, int ttl) where T : class;
}