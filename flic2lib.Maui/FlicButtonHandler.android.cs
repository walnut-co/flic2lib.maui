using Flic2lib.Android;

namespace flic2lib.Maui;

public partial class FlicButtonHandler : Flic2ButtonListener
{
    public override void OnButtonUpOrDown(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isUp, bool isDown)
    {
        
        if (isUp)
        {
            ButtonUp?.Invoke(this, new FlicButtonUpEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
        if (isDown)
        {
            ButtonDown?.Invoke(this, new FlicButtonDownEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
    }

    public override void OnButtonClickOrHold(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isClick, bool isHold)
    {
        if (isClick)
        {
            ButtonClick?.Invoke(this, new FlicButtonClickEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
        if (isHold)
        {
            ButtonHold?.Invoke(this, new FlicButtonHoldEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
    }

    public override void OnButtonSingleOrDoubleClick(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isSingleClick, bool isDoubleClick)
    {
        if (isSingleClick)
        {
            ButtonClick?.Invoke(this, new FlicButtonClickEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
        if (isDoubleClick)
        {
            ButtonDoubleClick?.Invoke(this, new FlicButtonDoubleClickEvent(GetFlicButton(button?.Uuid), timestamp, wasQueued));
        }
    }

    public override void OnButtonSingleOrDoubleClickOrHold(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isSingleClick, bool isDoubleClick, bool isHold)
    {
        // Should be handled in Other methods
        //if (isSingleClick)
        //{
        //    ButtonClick?.Invoke(this, new FlicButtonClickEvent(timestamp, wasQueued));
        //}
        //if (isDoubleClick)
        //{
        //    ButtonDoubleClick?.Invoke(this, new FlicButtonDoubleClickEvent(timestamp, wasQueued));
        //}
        //if (isHold)
        //{
        //    ButtonHold?.Invoke(this, new FlicButtonHoldEvent(timestamp, wasQueued));
        //}
    }

    public override void OnBatteryLevelUpdated(Flic2Button? button, BatteryLevel? level)
    {
        BatteryLevelChanged?.Invoke(this, new FlicButtonBatteryLevelChangedEvent(GetFlicButton(button?.Uuid), level?.Voltage));
    }

    public override void OnNameUpdated(Flic2Button? button, string? newName)
    {
        NameChanged?.Invoke(this, new FlicButtonNameChangedEvent(GetFlicButton(button?.Uuid), newName));
    }

    public override void OnUnpaired(Flic2Button? button)
    {
        Unparied?.Invoke(this, new FlicButtonUnpairedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void OnConnect(Flic2Button? button)
    {
        Connected?.Invoke(this, new FlicButtonConnectedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void OnDisconnect(Flic2Button? button)
    {
        Disconnected?.Invoke(this, new FlicButtonDisconnectedEvent(GetFlicButton(button?.Uuid)));
    }

    public override void OnFailure(Flic2Button? button, int errorCode, int subCode)
    {
        Failure?.Invoke(this, new FlicButtonFailureEvent(GetFlicButton(button?.Uuid), errorCode, subCode));
    }

    public override void OnReady(Flic2Button? button, long timestamp)
    {
        IsReadyChanged?.Invoke(this, new FlicButtonIsReadyEvent(GetFlicButton(button?.Uuid), timestamp));
    }
}
