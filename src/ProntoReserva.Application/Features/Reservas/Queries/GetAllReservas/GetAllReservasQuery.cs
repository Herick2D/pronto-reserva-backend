namespace ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;

public record GetAllReservasQuery(int PageNumber = 1, int PageSize = 10);