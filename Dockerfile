FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ProntoReserva.Api/ProntoReserva.Api.csproj", "src/ProntoReserva.Api/"]
COPY ["src/ProntoReserva.Application/ProntoReserva.Application.csproj", "src/ProntoReserva.Application/"]
COPY ["src/ProntoReserva.Domain/ProntoReserva.Domain.csproj", "src/ProntoReserva.Domain/"]
COPY ["src/ProntoReserva.Infrastructure/ProntoReserva.Infrastructure.csproj", "src/ProntoReserva.Infrastructure/"]

RUN dotnet restore "src/ProntoReserva.Api/ProntoReserva.Api.csproj"

COPY ./src ./src

WORKDIR "/src/src/ProntoReserva.Api"
RUN dotnet publish "ProntoReserva.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProntoReserva.Api.dll"]
