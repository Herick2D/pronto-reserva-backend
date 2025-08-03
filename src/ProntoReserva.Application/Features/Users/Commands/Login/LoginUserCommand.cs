namespace ProntoReserva.Application.Features.Users.Commands.Login;

public record LoginUserResponse(string Token);

public record LoginUserCommand(string Email, string Password);