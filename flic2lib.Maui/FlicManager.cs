namespace flic2lib.Maui;

public static partial class FlicManager
{
    public static IEnumerable<FlicButton>? Buttons => GetButtons();

    partial void Initialize();

    public static partial void ForgetButton(FlicButton button);
    private static partial IEnumerable<FlicButton>? GetButtons();
}

