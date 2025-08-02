namespace ProntoReserva.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T evento) where T : class;
}