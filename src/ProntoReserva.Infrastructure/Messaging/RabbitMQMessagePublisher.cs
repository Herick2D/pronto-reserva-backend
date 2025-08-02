using ProntoReserva.Application.Abstractions.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProntoReserva.Infrastructure.Messaging
{
    public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QUEUE_NAME = "reservas-confirmadas-queue";

        public RabbitMQMessagePublisher()
        {
            //TODO tirar do hardedcode para consumir através do arquivo de configuração.
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public Task PublishAsync<T>(T evento) where T : class
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));
            _channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: null,
                body: body
            );
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}