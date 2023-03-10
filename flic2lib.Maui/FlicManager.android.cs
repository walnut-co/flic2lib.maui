using IO.Flic.Flic2libandroid;

namespace flic2lib.Maui;

public static partial class FlicManager
{
    static FlicManager()
    {
    }

    private static partial IEnumerable<FlicButton>? GetButtons()
    {
        return Flic2Manager.Instance?.Buttons?.Select(btn => new FlicButton(btn));
    }

    public static partial void ForgetButton(FlicButton button)
    {
        Flic2Manager.Instance?.ForgetButton((Flic2Button)button.GetInternalButton());
    }
}