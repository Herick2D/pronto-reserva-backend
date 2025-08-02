using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProntoReserva.Application.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProntoReserva.Infrastructure.Messaging;

public class LembretesConsumer : BackgroundService
{
    private readonly ILogger<LembretesConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "lembretes-prontos-queue";

    public LembretesConsumer(ILogger<LembretesConsumer> logger)
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
                var evento = JsonSerializer.Deserialize<LembreteReservaEvent>(message);

                _logger.LogWarning("--> [LEMBRETES] Lembrete agendado recebido para o Cliente: '{NomeCliente}', Reserva ID: {ReservaId}", evento.NomeCliente, evento.ReservaId);
                _logger.LogInformation("--> Simulando envio de SMS/Push de lembrete...");

                Thread.Sleep(1000);

                _logger.LogInformation("--> Lembrete para a Reserva ID: {ReservaId} enviado com sucesso.", evento.ReservaId);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Erro ao processar mensagem de lembrete.");
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("--> [RabbitMQ] Consumidor de Lembretes iniciado.");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
