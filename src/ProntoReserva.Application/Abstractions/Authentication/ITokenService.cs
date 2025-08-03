using ProntoReserva.Domain.Entities;

namespace ProntoReserva.Application.Abstractions.Authentication;

public interface ITokenService
{
    string GenerateToken(User user);
}