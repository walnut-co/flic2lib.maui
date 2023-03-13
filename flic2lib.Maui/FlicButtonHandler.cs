namespace flic2lib.Maui;

public partial class FlicButtonHandler
{
    public event EventHandler<FlicButtonConnectedEvent>? Connected;
    public event EventHandler<FlicButtonDisconnectedEvent>? Disconnected;
    public event EventHandler<FlicButtonUnpairedEvent>? Unparied;
    public event EventHandler<FlicButtonFailureEvent>? Failure;
    public event EventHandler<FlicButtonIsReadyEvent>? IsReadyChanged;
    public event EventHandler<FlicButtonNameChangedEvent>? NameChanged;
    public event EventHandler<FlicButtonBatteryLevelChangedEvent>? BatteryLevelChanged;
    public event EventHandler<FlicButtonUpEvent>? ButtonUp;
    public event EventHandler<FlicButtonDownEvent>? ButtonDown;
    public event EventHandler<FlicButtonClickEvent>? ButtonClick;
    public event EventHandler<FlicButtonDoubleClickEvent>? ButtonDoubleClick;
    public event EventHandler<FlicButtonHoldEvent>? ButtonHold;

    public static FlicButtonHandler Instance { get; } = new FlicButtonHandler();
    private FlicButtonHandler() { }

    private static FlicButton? GetFlicButton(string? uuid)
    {
        return FlicManager.Instance.GetButtonByUuid(uuid);
    }
}

public record FlicButtonUpEvent(FlicButton? Button, long Timestamp, bool Queued);
public record FlicButtonDownEvent(FlicButton? Button, long Timestamp, bool Queued);
public record FlicButtonClickEvent(FlicButton? Button, long Timestamp, bool Queued);
public record FlicButtonDoubleClickEvent(FlicButton? Button, long Timestamp, bool Queued);
public record FlicButtonHoldEvent(FlicButton? Button, long Timestamp, bool Queued);
public record FlicButtonConnectedEvent(FlicButton? Button);
public record FlicButtonDisconnectedEvent(FlicButton? Button);
public record FlicButtonUnpairedEvent(FlicButton? Button);
public record FlicButtonFailureEvent(FlicButton? Button, int ErrorCode, int SubCode);
public record FlicButtonIsReadyEvent(FlicButton? Button, long Timestamp);
public record FlicButtonNameChangedEvent(FlicButton? Button, string? name);
public record FlicButtonBatteryLevelChangedEvent(FlicButton? Button, float? Voltage);