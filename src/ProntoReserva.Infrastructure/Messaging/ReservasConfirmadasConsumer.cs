using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProntoReserva.Application.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProntoReserva.Infrastructure.Messaging;

public class ReservasConfirmadasConsumer : BackgroundService
{
    private readonly ILogger<ReservasConfirmadasConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "reservas-confirmadas-queue";

    public ReservasConfirmadasConsumer(ILogger<ReservasConfirmadasConsumer> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
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
                _logger.LogInformation("--> [RabbitMQ] Recebida mensagem: {Message}", message);

                var evento = JsonSerializer.Deserialize<ReservaConfirmadaEvent>(message);

                _logger.LogInformation("--> Processando evento de reserva confirmada para o ID: {ReservaId}", evento.ReservaId);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Erro ao processar mensagem do RabbitMQ.");
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("--> [RabbitMQ] Consumidor iniciado. A aguardar por mensagens...");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
