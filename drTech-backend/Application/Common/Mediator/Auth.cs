using MediatR;
using drTech_backend.Infrastructure.Abstractions;
using drTech_backend.Infrastructure.Auth;

namespace drTech_backend.Application.Common.Mediator
{
    // Auth Commands
    public record RegisterCommand(string Email, string Password) : IRequest<Unit>;
    public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
    public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshResponse>;
    public record GoogleLoginCommand(string Email, string IdToken) : IRequest<LoginResponse>;

    // Auth Responses
    public record LoginResponse(string AccessToken, string RefreshToken);
    public record RefreshResponse(string AccessToken);

    // Auth Handlers
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly IDatabaseService<Domain.Entities.User> _users;

        public RegisterCommandHandler(IDatabaseService<Domain.Entities.User> users)
        {
            _users = users;
        }

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existing = (await _users.FindAsync(u => u.Email == request.Email, cancellationToken)).FirstOrDefault();
            if (existing != null) throw new InvalidOperationException("Email already in use");

            var user = new Domain.Entities.User 
            { 
                Id = Guid.NewGuid(), 
                Email = request.Email, 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User" // Default role
            };

            await _users.AddAsync(user, cancellationToken);
            await _users.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IDatabaseService<Domain.Entities.User> _users;
        private readonly IDatabaseService<Domain.Entities.RefreshToken> _refreshTokens;
        private readonly IJwtTokenService _jwt;

        public LoginCommandHandler(
            IDatabaseService<Domain.Entities.User> users,
            IDatabaseService<Domain.Entities.RefreshToken> refreshTokens,
            IJwtTokenService jwt)
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = (await _users.FindAsync(u => u.Email == request.Email, cancellationToken)).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            await _refreshTokens.AddAsync(refreshToken, cancellationToken);
            await _refreshTokens.SaveChangesAsync(cancellationToken);

            return new LoginResponse(accessToken, refreshToken.Token);
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshResponse>
    {
        private readonly IDatabaseService<Domain.Entities.User> _users;
        private readonly IDatabaseService<Domain.Entities.RefreshToken> _refreshTokens;
        private readonly IJwtTokenService _jwt;

        public RefreshTokenCommandHandler(
            IDatabaseService<Domain.Entities.User> users,
            IDatabaseService<Domain.Entities.RefreshToken> refreshTokens,
            IJwtTokenService jwt)
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _jwt = jwt;
        }

        public async Task<RefreshResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = (await _users.FindAsync(u => u.RefreshTokens.Any(r => r.Token == request.RefreshToken && !r.Revoked && r.ExpiresAtUtc > DateTime.UtcNow), cancellationToken)).FirstOrDefault();
            if (user == null) throw new UnauthorizedAccessException("Invalid refresh token");

            var accessToken = _jwt.GenerateAccessToken(user);
            return new RefreshResponse(accessToken);
        }
    }

    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, LoginResponse>
    {
        private readonly IDatabaseService<Domain.Entities.User> _users;
        private readonly IDatabaseService<Domain.Entities.RefreshToken> _refreshTokens;
        private readonly IJwtTokenService _jwt;

        public GoogleLoginCommandHandler(
            IDatabaseService<Domain.Entities.User> users,
            IDatabaseService<Domain.Entities.RefreshToken> refreshTokens,
            IJwtTokenService jwt)
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            // TODO: Verify Google ID token server-side using Google.Apis.Auth
            // For now, trusting the email from client (not recommended for production)
            var email = request.Email;

            var user = (await _users.FindAsync(u => u.Email == email, cancellationToken)).FirstOrDefault();
            if (user == null)
            {
                user = new Domain.Entities.User 
                { 
                    Id = Guid.NewGuid(), 
                    Email = email, 
                    PasswordHash = string.Empty,
                    Role = "User"
                };
                await _users.AddAsync(user, cancellationToken);
                await _users.SaveChangesAsync(cancellationToken);
            }

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            await _refreshTokens.AddAsync(refreshToken, cancellationToken);
            await _refreshTokens.SaveChangesAsync(cancellationToken);

            return new LoginResponse(accessToken, refreshToken.Token);
        }
    }
}
