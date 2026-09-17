using LoanPlatform.ApiGateway.DependencyInjection;
using LoanPlatform.ApiGateway.Endpoints;

namespace LoanPlatform.ApiGateway
{
    public class Program
    {
        private const string ReverseProxyKey = "ReverseProxy";

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGatewayAuthentication(builder.Configuration);

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

            app.MapAuthenticationEndpoints(builder.Configuration);

            app.MapReverseProxy()
                .RequireAuthorization();

            app.Run();
        }
    }
}