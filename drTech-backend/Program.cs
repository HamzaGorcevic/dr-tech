
using drTech_backend.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetEnv;

namespace drTech_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load .env file first
            Env.Load();
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Override configuration with environment variables
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_PROVIDER")))
                builder.Configuration["DatabaseProvider"] = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")))
                builder.Configuration["ConnectionStrings:Postgres"] = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_KEY")))
                builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_KEY");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_ISSUER")))
                builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_AUDIENCE")))
                builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES")))
                builder.Configuration["Jwt:AccessTokenMinutes"] = Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DAYS")))
                builder.Configuration["Jwt:RefreshTokenDays"] = Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DAYS");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")))
                builder.Configuration["Authentication:Google:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")))
                builder.Configuration["Authentication:Google:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")))
                builder.Configuration["Mongo:ConnectionString"] = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_DATABASE")))
                builder.Configuration["Mongo:Database"] = Environment.GetEnvironmentVariable("MONGO_DATABASE");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO4J_URI")))
                builder.Configuration["Neo4j:Uri"] = Environment.GetEnvironmentVariable("NEO4J_URI");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO4J_USER")))
                builder.Configuration["Neo4j:User"] = Environment.GetEnvironmentVariable("NEO4J_USER");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO4J_PASSWORD")))
                builder.Configuration["Neo4j:Password"] = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO4J_DATABASE")))
                builder.Configuration["Neo4j:Database"] = Environment.GetEnvironmentVariable("NEO4J_DATABASE");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_LIMIT")))
                builder.Configuration["Throttling:Limit"] = Environment.GetEnvironmentVariable("THROTTLING_LIMIT");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_WINDOW")))
                builder.Configuration["Throttling:Window"] = Environment.GetEnvironmentVariable("THROTTLING_WINDOW");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_BLOCK_DURATION")))
                builder.Configuration["Throttling:BlockDuration"] = Environment.GetEnvironmentVariable("THROTTLING_BLOCK_DURATION");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_STRICT_PATHS")))
                builder.Configuration["Throttling:StrictPaths"] = Environment.GetEnvironmentVariable("THROTTLING_STRICT_PATHS");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_STRICT_LIMIT")))
                builder.Configuration["Throttling:StrictLimit"] = Environment.GetEnvironmentVariable("THROTTLING_STRICT_LIMIT");
                
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("THROTTLING_STRICT_WINDOW")))
                builder.Configuration["Throttling:StrictWindow"] = Environment.GetEnvironmentVariable("THROTTLING_STRICT_WINDOW");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Infrastructure + MediatR + AutoMapper
            builder.Services.AddInfrastructure(builder.Configuration);

            // Auth (JWT + Google) placeholders; actual keys from config
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
            });

            builder.Services.AddAuthorization();

            builder.Services.Configure<drTech_backend.Infrastructure.Mongo.MongoSettings>(builder.Configuration.GetSection("Mongo"));
            builder.Services.Configure<drTech_backend.Infrastructure.Neo4j.Neo4jSettings>(builder.Configuration.GetSection("Neo4j"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<Middleware.ErrorHandlingMiddleware>();
            app.UseMiddleware<Middleware.RequestLoggingMiddleware>();
            app.UseMiddleware<Middleware.ThrottlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Middleware.AuditMiddleware>();

            app.MapControllers();

            var activeProvider = (builder.Configuration["DatabaseProvider"] ?? "PostgreSQL").Trim();
            if (string.Equals(activeProvider, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                app.MigrateAndSeedAsync(app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Seeder")).GetAwaiter().GetResult();
            }

            if (string.Equals(activeProvider, "MongoDB", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(activeProvider, "Neo4j", StringComparison.OrdinalIgnoreCase))
            {
                drTech_backend.Infrastructure.Bootstrap.NoSqlBootstrapper.InitializeAsync(app, app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Bootstrap")).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}
