using ITI.AuthDemo.Server;
using ITI.AuthDemo.Server.Handlers;
using ITI.GRPCLab.Server.Services;
using Microsoft.AspNetCore.Authentication;

namespace ITI.GRPCLab.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddGrpc();

            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Consts.ApiKeySchemeName;
            }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(Consts.ApiKeySchemeName, configureOptions => { });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<InventoryService>();

            app.Run();
        }
    }
}
