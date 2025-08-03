using ProntoReserva.Application.Abstractions.Authentication;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Users.Commands.Login;

public class LoginUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }
        
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var token = _tokenService.GenerateToken(user);

        return new LoginUserResponse(token);
    }
}