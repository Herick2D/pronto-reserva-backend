using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProntoReserva.Application.Abstractions.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProntoReserva.Infrastructure.Messaging;

public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string CONFIRMATION_QUEUE_NAME = "reservas-confirmadas-queue";
    private const string REMINDER_EXCHANGE = "lembretes.exchange";
    private const string REMINDER_READY_QUEUE = "lembretes-prontos-queue";
    private const string REMINDER_WAIT_QUEUE = "lembretes-espera-queue";

    public RabbitMQMessagePublisher(ILogger<RabbitMQMessagePublisher> logger, IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            UserName = configuration["RabbitMq:UserName"],
            Password = configuration["RabbitMq:Password"]
        };

        var retries = 5;
        while (retries > 0)
        {
            try
            {
                _connection = factory.CreateConnection();
                logger.LogInformation("--> [RabbitMQ] Conexão do Publicador estabelecida com sucesso.");
                break;
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                retries--;
                logger.LogWarning(ex, "--> [RabbitMQ] Publicador não conseguiu conectar. Tentando novamente em 5s... ({retries} tentativas restantes)", retries);
                Thread.Sleep(5000);
            }
        }
        if (_connection is null) throw new Exception("Não foi possível conectar ao RabbitMQ após várias tentativas.");
        
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: CONFIRMATION_QUEUE_NAME,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.ExchangeDeclare(REMINDER_EXCHANGE, ExchangeType.Direct);
        _channel.QueueDeclare(queue: REMINDER_READY_QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", REMINDER_EXCHANGE },
            { "x-dead-letter-routing-key", REMINDER_READY_QUEUE }
        };
        _channel.QueueDeclare(queue: REMINDER_WAIT_QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: args);

        _channel.QueueBind(queue: REMINDER_WAIT_QUEUE, exchange: REMINDER_EXCHANGE, routingKey: REMINDER_WAIT_QUEUE);
        _channel.QueueBind(queue: REMINDER_READY_QUEUE, exchange: REMINDER_EXCHANGE, routingKey: REMINDER_READY_QUEUE);
    }

    public Task PublishAsync<T>(T evento) where T : class
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));
        _channel.BasicPublish(
            exchange: "",
            routingKey: CONFIRMATION_QUEUE_NAME,
            basicProperties: null,
            body: body
        );
        return Task.CompletedTask;
    }

    public Task PublishWithDelayAsync<T>(T evento, int ttl) where T : class
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));
        var properties = _channel.CreateBasicProperties();
        properties.Expiration = ttl.ToString();
        properties.Persistent = true;
        _channel.BasicPublish(
            exchange: REMINDER_EXCHANGE,
            routingKey: REMINDER_WAIT_QUEUE,
            basicProperties: properties,
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