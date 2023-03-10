using Flic2lib.iOS;

namespace flic2lib.Maui;

public static partial class FlicManager
{
    static FlicManager()
    {
    }

    private static partial IEnumerable<FlicButton>? GetButtons()
    {
        return FLICManager.SharedManager()?.Buttons.Select(btn => new FlicButton(btn)).ToList();
    }
    public static partial void ForgetButton(FlicButton button)
    {
        FLICManager.SharedManager()?.ForgetButton((FLICButton)button.GetInternalButton(), (_, __) => { });
    }
}