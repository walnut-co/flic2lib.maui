using Flic2lib.iOS;
using Foundation;

namespace flic2lib.Maui;

public partial class FlicButtonHandler : FLICButtonDelegate
{
    public override void ButtonDidConnect(FLICButton button)
    {
        Connected?.Invoke(this, new FlicButtonConnectedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void ButtonIsReady(FLICButton button)
    {
        IsReadyChanged?.Invoke(this, new FlicButtonIsReadyEvent(GetFlicButton(button?.Uuid), DateTime.Now.Ticks));
    }

    public override void DidDisconnectWithError(FLICButton button, NSError? error)
    {
        Disconnected?.Invoke(this, new FlicButtonDisconnectedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void DidFailToConnectWithError(FLICButton button, NSError? error)
    {
        Failure?.Invoke(this, new FlicButtonFailureEvent(GetFlicButton(button?.Uuid), error != null ? (int)error.Code : 0, 0));
    }

    public override void DidReceiveButtonDown(FLICButton button, bool queued, nint age)
    {
        ButtonDown?.Invoke(this, new FlicButtonDownEvent(GetFlicButton(button?.Uuid), age, queued));
    }

    public override void DidReceiveButtonUp(FLICButton button, bool queued, nint age)
    {
        ButtonUp?.Invoke(this, new FlicButtonUpEvent(GetFlicButton(button?.Uuid), age, queued));
    }

    public override void DidReceiveButtonClick(FLICButton button, bool queued, nint age)
    {
        ButtonClick?.Invoke(this, new FlicButtonClickEvent(GetFlicButton(button?.Uuid), age, queued));
    }

    public override void DidReceiveButtonDoubleClick(FLICButton button, bool queued, nint age)
    {
        ButtonDoubleClick?.Invoke(this, new FlicButtonDoubleClickEvent(GetFlicButton(button?.Uuid), age, queued));
    }

    public override void DidReceiveButtonHold(FLICButton button, bool queued, nint age)
    {
        ButtonHold?.Invoke(this, new FlicButtonHoldEvent(GetFlicButton(button?.Uuid), age, queued));
    }

    public override void DidUnpairWithError(FLICButton button, NSError? error)
    {
        Unparied?.Invoke(this, new FlicButtonUnpairedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void DidUpdateBatteryVoltage(FLICButton button, float voltage)
    {
        BatteryLevelChanged?.Invoke(this, new FlicButtonBatteryLevelChangedEvent(GetFlicButton(button?.Uuid), voltage));
    }

    public override void DidUpdateNickname(FLICButton button, string nickname)
    {
        NameChanged?.Invoke(this, new FlicButtonNameChangedEvent(GetFlicButton(button?.Uuid), nickname));
    }
}
