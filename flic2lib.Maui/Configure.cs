namespace flic2lib.Maui;

public static class Configure
{
    public static MauiAppBuilder UseFlic2lib(this MauiAppBuilder builder, bool initManager = false)
    {
         builder.Services
            .AddSingleton<IFlicManager>(FlicManager.Instance)
            .AddSingleton<IFlicButtonHandler>(FlicButtonHandler.Instance);

        if (initManager)
        {
            FlicManager.Init();
        }

        return builder;
    }
}
