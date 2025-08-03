using System;

namespace ProntoReserva.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid? GetUserId();
}