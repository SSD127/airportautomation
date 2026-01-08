using AirportAutomation.Core.Data;
using System.Collections.Generic;
using AirportAutomation.Interface;
using AirportAutomation.Service;
using AirportAutomation.UI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AirportAutomation.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Uygulama içi yapılandırma: yerel API anahtarı (istek üzerine gömülü)
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Api:Key"] = "AIzaSyDJt9o1sU0pXx3vehM0LKBdjsyy8H_wEXI",
        });
        builder.Services.AddSingleton(sp =>
        {
            var settings = new ApiSettings
            {
                ApiKey = builder.Configuration["Api:Key"] ?? string.Empty
            };
            return settings;
        });

        // DI: DbContext ve servisler
        builder.Services.AddDbContext<AirportDbContext>();
        builder.Services.AddScoped<IFlightService, SFlightService>();
        builder.Services.AddScoped<IPassengerService, SPassengerService>();
        builder.Services.AddScoped<IGateService, SGateService>();
        builder.Services.AddScoped<IAssistantService, SAssistantService>();
        builder.Services.AddScoped<IBookingService, SBookingService>();
        builder.Services.AddScoped<IAuthService, SAuthService>();
        builder.Services.AddSingleton<Services.AuthContext>();
        builder.Services.AddSingleton<Services.IGoogleAuthService, Services.GoogleAuthService>();
        builder.Services.AddSingleton<Services.AiChatService>();

        // Pages
        builder.Services.AddTransient<Pages.LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Service helper
        Helpers.ServiceHelper.Services = app.Services;

        // İlk açılışta veritabanını ve seed işlemini tamamla
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AirportDbContext>();
            var seedError = DbSeeder.Seed(db);
            if (!string.IsNullOrEmpty(seedError))
            {
                var logger = scope.ServiceProvider.GetService<ILogger<MauiApp>>();
                logger?.LogError("Seed error: {Error}", seedError);
            }
        }

        return app;
    }
}
