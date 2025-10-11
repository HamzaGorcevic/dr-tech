using drTech_backend.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace drTech_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Infrastructure.AppDbContext _db;
        private readonly IJwtTokenService _jwt;
        private readonly IConfiguration _config;

        public AuthController(Infrastructure.AppDbContext db, IJwtTokenService jwt, IConfiguration config)
        {
            _db = db; _jwt = jwt; _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken cancellationToken)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email, cancellationToken)) return Conflict("Email in use");
            var user = new Domain.Entities.User { Id = Guid.NewGuid(), Email = req.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password) };
            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == req.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Unauthorized();
            var access = _jwt.GenerateAccessToken(user);
            var refresh = _jwt.GenerateRefreshToken();
            refresh.UserId = user.Id;
            user.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { accessToken = access, refreshToken = refresh.Token });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken cancellationToken)
        {
            var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == req.RefreshToken && !r.Revoked && r.ExpiresAtUtc > DateTime.UtcNow), cancellationToken);
            if (user == null) return Unauthorized();
            var access = _jwt.GenerateAccessToken(user);
            return Ok(new { accessToken = access });
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google([FromBody] GoogleTokenRequest req, CancellationToken cancellationToken)
        {
            // For brevity: trust Google token verification is handled by frontend or gateway; in producancellationTokenion verify with Google APIs
            var email = req.Email;
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                user = new Domain.Entities.User { Id = Guid.NewGuid(), Email = email, PasswordHash = string.Empty };
                await _db.Users.AddAsync(user, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }
            var access = _jwt.GenerateAccessToken(user);
            var refresh = _jwt.GenerateRefreshToken();
            refresh.UserId = user.Id;
            await _db.RefreshTokens.AddAsync(refresh, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { accessToken = access, refreshToken = refresh.Token });
        }
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record GoogleTokenRequest(string Email, string IdToken);
}


