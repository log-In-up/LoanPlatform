using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LoanPlatform.ApiGateway.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGatewayAuthentication(
            this IServiceCollection services,
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

            services
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

            services.AddAuthorization();

            return services;
        }
    }
}