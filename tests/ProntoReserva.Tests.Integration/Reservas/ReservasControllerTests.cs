using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ProntoReserva.Tests.Integration.Reservas;

public class ReservasControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReservasControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostReserva_ComDadosValidos_DeveRetornarCreatedComLocalizacao()
    {

        var command = new CreateReservaCommand(
            "Cliente de Integração",
            DateTime.UtcNow.AddDays(10),
            4,
            "Teste de integração"
        );

        var jsonContent = JsonSerializer.Serialize(command);

        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/reservas", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();

        var createdResourceResponse = await _client.GetAsync(response.Headers.Location);
        createdResourceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostReserva_ComDadosInvalidos_DeveRetornarBadRequest()
    {

        var command = new CreateReservaCommand(
            "Cliente Inválido",
            DateTime.UtcNow.AddDays(-1),
            2,
            null
        );

        var response = await _client.PostAsJsonAsync("/api/reservas", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
