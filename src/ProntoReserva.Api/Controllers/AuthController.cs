using Microsoft.AspNetCore.Mvc;
using ProntoReserva.Application.Features.Users.Commands.Login;
using ProntoReserva.Application.Features.Users.Commands.Register;

namespace ProntoReserva.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly LoginUserCommandHandler _loginUserCommandHandler;

    public AuthController(
        RegisterUserCommandHandler registerUserCommandHandler,
        LoginUserCommandHandler loginUserCommandHandler)
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _loginUserCommandHandler = loginUserCommandHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            await _registerUserCommandHandler.Handle(command);
            return StatusCode(201);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado durante o registo.", details = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        try
        {
            var response = await _loginUserCommandHandler.Handle(command);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro inesperado durante o login.", details = ex.Message });
        }
    }
}
