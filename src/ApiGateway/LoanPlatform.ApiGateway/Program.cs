using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LoanPlatform.ApiGateway
{
    public record LoginRequest(string Username, string Password);

    public class Program
    {
        private const string ReverseProxyKey = "ReverseProxy";

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder =
                WebApplication.CreateBuilder(args);

            string jwtKey =
                builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key is not configured.");

            string jwtIssuer =
                builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "Jwt:Issuer is not configured.");

            string jwtAudience =
                builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "Jwt:Audience is not configured.");

            string demoUsername =
                builder.Configuration["DemoUser:Username"]
                ?? throw new InvalidOperationException(
                    "DemoUser:Username is not configured.");

            string demoPassword =
                builder.Configuration["DemoUser:Password"]
                ?? throw new InvalidOperationException(
                    "DemoUser:Password is not configured.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtIssuer,
                            ValidAudience = jwtAudience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(jwtKey))
                        };
                });

            builder.Services.AddAuthorization();

            builder.Services
                .AddReverseProxy()
                .LoadFromConfig(
                    builder.Configuration.GetSection(ReverseProxyKey));

            builder.Services.AddOpenApi();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/api/auth/check", () =>
                {
                    return Results.Ok(new
                    {
                        message = "You are authenticated."
                    });
                })
                .RequireAuthorization();

            app.MapPost("/api/auth/token", (LoginRequest request) =>
            {
                if (request.Username != demoUsername ||
                    request.Password != demoPassword)
                {
                    return Results.Unauthorized();
                }

                List<Claim> claims =
                [
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "User")
                ];

                SymmetricSecurityKey securityKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey));

                SigningCredentials credentials =
                    new SigningCredentials(
                        securityKey,
                        SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: jwtIssuer,
                        audience: jwtAudience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: credentials);

                string tokenString =
                    new JwtSecurityTokenHandler()
                        .WriteToken(token);

                return Results.Ok(new
                {
                    accessToken = tokenString,
                    tokenType = "Bearer",
                    expiresIn = 3600
                });
            });

            app.MapReverseProxy()
                .RequireAuthorization();

            app.Run();
        }
    }
}