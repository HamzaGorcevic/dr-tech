using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using drTech_backend.Application.Common.Mediator;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new RegisterCommand(req.Email, req.Password), cancellationToken);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new LoginCommand(req.Email, req.Password), cancellationToken);
                return Ok(new { accessToken = response.AccessToken, refreshToken = response.RefreshToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new RefreshTokenCommand(req.RefreshToken), cancellationToken);
                return Ok(new { accessToken = response.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google([FromBody] GoogleTokenRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new GoogleLoginCommand(req.Email, req.IdToken), cancellationToken);
                return Ok(new { accessToken = response.AccessToken, refreshToken = response.RefreshToken });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record GoogleTokenRequest(string Email, string IdToken);
}


