using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ProntoReserva.Application.Features.Users.Commands.Login;
using ProntoReserva.Application.Features.Users.Commands.Register;
using System.Net;
using System.Net.Http.Json;

namespace ProntoReserva.Tests.Integration.Auth;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithNewEmail_ShouldReturnCreated()
    {
        var email = $"novo-utilizador-{Guid.NewGuid()}@teste.com";
        var command = new RegisterUserCommand(email, "SenhaForte123!");

        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnConflict()
    {
        var email = $"utilizador-existente-{Guid.NewGuid()}@teste.com";
        var command = new RegisterUserCommand(email, "SenhaForte123!");
        await _client.PostAsJsonAsync("/api/auth/register", command);

        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        var email = $"utilizador-valido-{Guid.NewGuid()}@teste.com";
        var password = "SenhaForte123!";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserCommand(email, password));
        var loginCommand = new LoginUserCommand(email, password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResult = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginResult.Should().NotBeNull();
        loginResult?.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        var email = $"senha-invalida-{Guid.NewGuid()}@teste.com";
        var password = "SenhaForte123!";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserCommand(email, password));
        var loginCommand = new LoginUserCommand(email, "senha-errada");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
