using FluentAssertions;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Users.Commands.Login;
using ProntoReserva.Application.Features.Users.Commands.Register;
using ProntoReserva.Tests.Integration.Helpers;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace ProntoReserva.Tests.Integration.Messaging;

public class MessagingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public MessagingTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
    }

    private async Task AuthenticateClientAsync()
    {
        var email = $"teste-msg-{Guid.NewGuid()}@testes.com";
        var password = "SenhaForte123!";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserCommand(email, password));
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginUserCommand(email, password));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginUserResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);
    }

    [Fact]
    public async Task ConfirmarReserva_DevePublicarMensagem_EOConsumidorDeveProcessaLa()
    {
        await AuthenticateClientAsync();
        
        var createCommand = new CreateReservaCommand("Cliente Mensageria", DateTime.UtcNow.AddDays(1), 1, null);
        var createResponse = await _client.PostAsJsonAsync("/api/reservas", createCommand);
        var createdReserva = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var newId = createdReserva.GetProperty("id").GetGuid();

        _factory.ConsumerLogger.LogMessages.Clear();

        var confirmResponse = await _client.PostAsync($"/api/reservas/{newId}/confirmar", null);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var stopwatch = Stopwatch.StartNew();
        string logDeSucesso = null;
        
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(10) && logDeSucesso == null)
        {
            logDeSucesso = _factory.ConsumerLogger.LogMessages
                .FirstOrDefault(msg => msg.Contains("processada com sucesso."));
            
            await Task.Delay(200);
        }

        if (logDeSucesso == null)
        {
            _output.WriteLine("TIMEOUT: A mensagem de sucesso não foi encontrada. Logs capturados:");
            foreach (var log in _factory.ConsumerLogger.LogMessages)
            {
                _output.WriteLine(log);
            }
        }

        logDeSucesso.Should().NotBeNull("porque o consumidor deveria ter processado a mensagem e registado o sucesso dentro do tempo limite.");
        logDeSucesso.Should().Contain(newId.ToString(), "porque o log de sucesso deve incluir o ID da reserva processada.");
    }
}
