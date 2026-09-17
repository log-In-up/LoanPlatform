using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoanPlatform.ApiGateway.Models;
using Microsoft.IdentityModel.Tokens;

namespace LoanPlatform.ApiGateway.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static IEndpointRouteBuilder MapAuthenticationEndpoints(
            this IEndpointRouteBuilder endpoints,
            IConfiguration configuration)
        {
            string jwtKey =
                configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key is not configured.");

            string jwtIssuer =
                configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "Jwt:Issuer is not configured.");

            string jwtAudience =
                configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "Jwt:Audience is not configured.");

            string demoUsername =
                configuration["DemoUser:Username"]
                ?? throw new InvalidOperationException(
                    "DemoUser:Username is not configured.");

            string demoPassword =
                configuration["DemoUser:Password"]
                ?? throw new InvalidOperationException(
                    "DemoUser:Password is not configured.");

            endpoints.MapGet("/api/auth/check", () =>
                    Results.Ok(new
                    {
                        message = "You are authenticated."
                    }))
                .RequireAuthorization();

            endpoints.MapPost(
                "/api/auth/token",
                (LoginRequest request) =>
                {
                    Console.WriteLine(
                        $"Login attempt: received username = '{request.Username}', " +
                        $"received password length = {request.Password?.Length ?? 0}, " +
                        $"expected username = '{demoUsername}', " +
                        $"expected password length = {demoPassword.Length}");

                    if (request.Username != demoUsername ||
                        request.Password != demoPassword)
                    {
                        return Results.Unauthorized();
                    }

                    List<Claim> claims =
                    [
                        new Claim(
                            JwtRegisteredClaimNames.Sub,
                            request.Username),

                        new Claim(
                            ClaimTypes.Name,
                            request.Username),

                        new Claim(
                            ClaimTypes.Role,
                            "User")
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

            return endpoints;
        }
    }
}