namespace flic2lib.Maui;

public interface IFlicButtonHandler
{
    event EventHandler<FlicButtonBatteryLevelChangedEvent>? BatteryLevelChanged;
    event EventHandler<FlicButtonClickEvent>? ButtonClick;
    event EventHandler<FlicButtonDoubleClickEvent>? ButtonDoubleClick;
    event EventHandler<FlicButtonDownEvent>? ButtonDown;
    event EventHandler<FlicButtonHoldEvent>? ButtonHold;
    event EventHandler<FlicButtonUpEvent>? ButtonUp;
    event EventHandler<FlicButtonConnectedEvent>? Connected;
    event EventHandler<FlicButtonDisconnectedEvent>? Disconnected;
    event EventHandler<FlicButtonFailureEvent>? Failure;
    event EventHandler<FlicButtonIsReadyEvent>? IsReadyChanged;
    event EventHandler<FlicButtonNameChangedEvent>? NameChanged;
    event EventHandler<FlicButtonUnpairedEvent>? Unparied;
}