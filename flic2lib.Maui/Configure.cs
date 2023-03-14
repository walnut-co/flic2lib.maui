namespace flic2lib.Maui;

public static class Configure
{
    public static MauiAppBuilder UseFlic2lib(this MauiAppBuilder builder)
    {
         builder.Services
            .AddSingleton<IFlicManager>(FlicManager.Instance)
            .AddSingleton<IFlicButtonHandler>(FlicButtonHandler.Instance);

        return builder;
    }
}
