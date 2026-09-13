using LoanPlatform.LoanCore.Api.DependencyInjection;
using LoanPlatform.LoanCore.Api.Endpoints;

namespace LoanPlatform.LoanCore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLoanCoreServices(builder.Configuration);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            WebApplication app = builder.Build();

            app.MapLoanEndpoints();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.Run();
        }
    }
}