using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProntoReserva.Application.Features.Reservas.Commands.CreateReserva;
using ProntoReserva.Application.Features.Reservas.Queries.GetAllReservas;
using ProntoReserva.Application.Features.Reservas.Queries.GetReservaById;
using ProntoReserva.Domain.Repositories;
using ProntoReserva.Infrastructure.Persistence;
using ProntoReserva.Infrastructure.Repositories;

namespace ProntoReserva.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<IReservaRepository>(sp => new ReservaRepository(connectionString!));
        
        services.AddScoped<CreateReservaCommandHandler>();
        
        services.AddScoped<GetReservaByIdQueryHandler>();
        services.AddScoped<GetAllReservasQueryHandler>();

        return services;
    }
}