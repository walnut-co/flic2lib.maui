namespace flic2lib.Maui;

public partial class FlicManager : IFlicManager
{
    private readonly IList<FlicButton> _cachedButtons;
    private readonly static Lazy<FlicManager> _lazyInstance = new(() => new FlicManager(), LazyThreadSafetyMode.ExecutionAndPublication);
    public static bool IsInitialized { get; private set; }
    internal static FlicManager Instance => _lazyInstance.Value;
    public IEnumerable<FlicButton>? Buttons => GetButtons();

    private FlicManager()
    {
        _cachedButtons = new List<FlicButton>();
    }

    public void ClearCache()
    {
        _cachedButtons.Clear();
    }

    public static partial void Init();
    public partial void StartScan();
    public partial void StopScan();
    public partial FlicButton? GetButtonByUuid(string? uuid);
    public partial void ForgetButton(FlicButton button);
    private partial IEnumerable<FlicButton>? GetButtons();
}