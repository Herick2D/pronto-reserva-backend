using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ProntoReserva.Infrastructure.Messaging;

namespace ProntoReserva.Tests.Integration.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly InMemoryLogger<ReservasConfirmadasConsumer> ConsumerLogger = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ILogger<ReservasConfirmadasConsumer>>();

            services.AddSingleton<ILogger<ReservasConfirmadasConsumer>>(ConsumerLogger);
        });
    }
}