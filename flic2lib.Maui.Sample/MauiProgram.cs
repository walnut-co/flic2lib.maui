using flic2lib.Maui.Sample.Services;
using Microsoft.Extensions.Configuration;

namespace flic2lib.Maui.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFlic2lib(initManager: true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register HTTP client for API calls
        builder.Services.AddHttpClient<IIncidentService, IncidentService>();

        // Register services
        builder.Services
            .AddTransient<MainPage>()
            .AddTransient<Bluetooth>()
            .AddSingleton<IIncidentService, IncidentService>()
            .AddSingleton<FlicEventHandler>()
            .AddSingleton<BackgroundServiceManager>();

        // Add configuration (you can add appsettings.json if needed)
        builder.Configuration.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("IncidentApi:BaseUrl", "https://your-api-endpoint.com")
        });

        return builder.Build();
    }
}