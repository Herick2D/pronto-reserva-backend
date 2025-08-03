using ProntoReserva.Domain.Common;

namespace ProntoReserva.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    
    private User() { }

    public User(Guid id, string email, string passwordHash) : base(id)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail não pode ser vazio.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("O hash da senha não pode ser vazio.", nameof(passwordHash));
            
        Email = email;
        PasswordHash = passwordHash;
    }
}