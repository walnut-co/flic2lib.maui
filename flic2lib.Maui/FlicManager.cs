namespace flic2lib.Maui;

public partial class FlicManager : IFlicManager
{
    private readonly IList<FlicButton> _cachedButtons;

    public static bool IsInitialized { get; private set; }
    public static FlicManager Instance { get; } = new FlicManager();
    public IEnumerable<FlicButton>? Buttons => GetButtons();

    private FlicManager()
    {
        _cachedButtons = new List<FlicButton>();
    }

    public static partial void Init();
    public partial void StartScan();
    public partial void StopScan();
    public partial FlicButton? GetButtonByUuid(string? uuid);
    public partial void ForgetButton(FlicButton button);
    private partial IEnumerable<FlicButton>? GetButtons();
}