using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProntoReserva.Application.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProntoReserva.Infrastructure.Messaging;

public class ReservasConfirmadasConsumer : BackgroundService
{
    private readonly ILogger<ReservasConfirmadasConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "reservas-confirmadas-queue";

    public ReservasConfirmadasConsumer(ILogger<ReservasConfirmadasConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
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
                logger.LogInformation("--> [RabbitMQ] Conexão do Consumer de Confirmações estabelecida com sucesso.");
                break;
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                retries--;
                logger.LogWarning(ex, "--> [RabbitMQ] Consumer de Confirmações não conseguiu conectar. Tentando novamente em 5s... ({retries} tentativas restantes)", retries);
                Thread.Sleep(5000);
            }
        }
        if (_connection is null) throw new Exception("Não foi possível conectar ao RabbitMQ após várias tentativas.");

        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var evento = JsonSerializer.Deserialize<ReservaConfirmadaEvent>(message);
                if (evento is null)
                {
                    _logger.LogError("--> [CONSUMIDOR] Não foi possível desserializar a mensagem recebida.");
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }
                _logger.LogInformation("--> [CONSUMIDOR] Evento de confirmação recebido para a Reserva ID: {ReservaId}", evento.ReservaId);
                _logger.LogInformation("--> Simulando envio de e-mail...");
                Thread.Sleep(3000);
                _logger.LogInformation("--> Notificação para a Reserva ID: {ReservaId} processada com sucesso.", evento.ReservaId);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Erro ao processar mensagem do RabbitMQ.");
            }
        };
        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("--> [RabbitMQ] Consumidor de Confirmações iniciado. A aguardar por mensagens...");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}