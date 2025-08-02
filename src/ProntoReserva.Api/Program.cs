using Microsoft.EntityFrameworkCore;
using ProntoReserva.Infrastructure;
using ProntoReserva.Infrastructure.Persistence;
using ProntoReserva.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ReservasConfirmadasConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }