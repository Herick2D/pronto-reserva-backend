using ProntoReserva.Domain.Entities;
using ProntoReserva.Domain.Repositories;

namespace ProntoReserva.Application.Features.Users.Commands.Register;

public class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(RegisterUserCommand command)
    {
        var existingUser = await _userRepository.GetByEmailAsync(command.Email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Um utilizador com este e-mail já existe.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var user = new User(Guid.NewGuid(), command.Email, passwordHash);

        await _userRepository.AddAsync(user);
    }
}