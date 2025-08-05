# Pronto-Reserva Backend

Este é o backend da aplicação Pronta-Reserva, uma API desenvolvida em .NET para gerenciar toda a lógica de negócio, persistência de dados e comunicação assíncrona do sistema.

## Funcionalidades

- **Arquitetura Limpa e DDD:** Estrutura baseada em Domain-Driven Design para um código organizado, testável e escalável.  
- **Autenticação com JWT:** Sistema de segurança completo com registro, login e validação de tokens JWT para proteger os endpoints.  
- **CRUD Seguro e Multi-usuário:** Operações de Criar, Ler, Atualizar e Apagar reservas, garantindo que cada usuário só possa acessar e gerenciar os seus próprios dados.  
- **Soft Delete:** As reservas nunca são permanentemente apagadas, permitindo histórico e recuperação de dados.  
- **Mensageria Assíncrona com RabbitMQ:**  
  I. **Notificações Imediatas:** Desacoplamento de tarefas (como o envio de e-mails de confirmação) para uma API mais rápida e resiliente.  
  II. **Lembretes Agendados:** Sistema avançado de mensagens atrasadas para enviar lembretes aos clientes na véspera da reserva.  
- **Ambiente Dockerizado:** Toda as dependências da aplicação (PostgreSQL, RabbitMQ) são orquestradas com Docker Compose para um ambiente de desenvolvimento e produção consistente e isolado.

## Tecnologias Utilizadas

- **[.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)** - Plataforma de desenvolvimento moderna e de alta performance.
- **[C#](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)** - Linguagem de programação principal.
- **[ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)** - Framework para a construção da API web.
- **[Domain-Driven Design(DDD)](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)** - Abordagem arquitetónica para sistemas complexos.
- **[Dapper](https://www.learndapper.com/)** - ORM de alta performance para acesso a dados.
- **[Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)** - Usado para gerir as migrações do esquema da base de dados de forma automática.
- **[PostgreSQL](https://www.postgresql.org/)** - Sistema de gestão de base de dados relacional.
- **[RabbitMQ](https://www.rabbitmq.com/tutorials)** - Message broker para comunicação assíncrona.
- **[xUnit](https://xunit.net/?tabs=cs)** - Framework para testes unitários e de integração.
- **[JWT (JSON Web Tokens)](https://www.jwt.io/introduction)** - Padrão para a criação de tokens de acesso.
- **[Docker](https://www.docker.com/)** - Para a contentorização e orquestração da aplicação.

## Como Executar o Projeto

A forma mais simples e recomendada de executar o projeto é utilizando Docker Compose, que irá orquestrar todos os serviços necessários.

### Pré-requisitos

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker](https://www.docker.com/)

### Instalação

1. Clone o repositório;
    ```
        git clone https://github.com/Herick2D/pronto-reserva-backend.git
    ```

2. Dentro do repositório, execute os seguintes comandos;

   _obs: este primeiro comando mantém o terminal ocupado. Para executar os próximos passos, abra outra aba ou janela do terminal._
    ```
        docker-compose up
    ```
    
    ```
        dotnet restore ProntoReserva.sln
    ```
    ```
        dotnet run --project .\src\ProntoReserva.API\ProntoReserva.API.csproj
    ```
    Com isso a API estará rodando e pronta para receber requisições!

## Rodando os Testes

Este projeto possui uma suite de testes robusta que cobre a lógica de negócio (testes unitários) e o fluxo completo da API (testes de integração).

### Rodar Testes Unitários

Estes testes validam as regras de negócio e a lógica dos handlers de forma isolada. Não requerem que a aplicação esteja a correr.

```
    dotnet test ./tests/ProntoReserva.Tests.Unit/
```

### Rodar Testes de Integração

Estes testes validam o fluxo completo da API, incluindo a interação com a base de dados e a autenticação.

```
    dotnet test ./tests/ProntoReserva.Tests.Integration/
```

## Observações:
Este projeto foi desenvolvido dentro de prazos de entrega pré-estabelecidos e com objetivos avaliativos definidos. As escolhas de tecnologias e as metodologias de trabalho seguiram tanto as diretrizes do enunciado quanto critérios pessoais de eficiência e familiaridade.
Devido ao tempo limitado, algumas funcionalidades podem apresentar bugs ou comportamentos inesperados. Relatórios de issues, sugestões de melhorias e contribuições são muito bem-vindos!
