using IO.Flic.Flic2libandroid;

namespace flic2lib.Maui;

public partial class FlicButton : Flic2ButtonListener
{
    private readonly Flic2Button _button;

    public FlicButton(Flic2Button flic2Button)
    {
        _button = flic2Button;
        _button.AddListener(this);
    }

    public partial void Connect()
    {
        _button.Connect();
    }

    public partial void Disconnect()
    {
        _button.DisconnectOrAbortPendingConnection();
    }

    public partial void StartListen()
    {
        _button.AddListener(this);
    }

    public partial void StopListen()
    {
        _button.RemoveListener(this);
    }

    public partial object GetInternalButton() => _button;
    private partial string? GetBluetoothAddress() => _button.BdAddr;
    private partial string? GetName() => _button.Name;
    private partial string? GetNickName() => _button.Name;
    private partial void SetNickName(string? value) => _button.Name = value;
    private partial string? GetSerialNumber() => _button.SerialNumber;
    private partial string? GetUuid() => _button.Uuid;
    private partial int GetFirmwareVersion() => _button.FirmwareVersion;
    private partial int GetPressCount() => _button.PressCount;
    private partial float? GetBatteryVoltage() => _button.LastKnownBatteryLevel?.Voltage;
    private partial bool GetIsReady() => _button.ReadyTimestamp > 0;
    private partial bool GetIsUnpaired() => _button.IsUnpaired;
    private partial string? GetIdentifier() => _button.SerialNumber;
    private partial FlicButtonConnectionState GetConnectionState() => (FlicButtonConnectionState)_button.ConnectionState;

    public override void OnButtonUpOrDown(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isUp, bool isDown)
    {
        if (isUp)
        {
           ButtonUp?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.Up));
        }
        if (isDown)
        {
            ButtonDown?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.Down));
        }
    }

    public override void OnButtonClickOrHold(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isClick, bool isHold)
    {
        if (isClick)
        {
            ButtonClick?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.SingleClick));
        }
        if (isHold)
        {
            ButtonHold?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.Hold));
        }
    }

    public override void OnButtonSingleOrDoubleClick(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isSingleClick, bool isDoubleClick)
    {
        if (isSingleClick)
        {
            ButtonClick?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.SingleClick));
        }
        if (isDoubleClick)
        {
            ButtonDoubleClick?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.DoubleClick));
        }
    }

    public override void OnButtonSingleOrDoubleClickOrHold(Flic2Button? button, bool wasQueued, bool lastQueued, long timestamp, bool isSingleClick, bool isDoubleClick, bool isHold)
    {
        if (isSingleClick)
        {
            ButtonClick?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.SingleClick));
        }
        if (isDoubleClick)
        {
            ButtonDoubleClick?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.DoubleClick));
        }
        if (isHold)
        {
            ButtonHold?.Invoke(this, new FlicButtonEvent(timestamp, wasQueued, ClickActions.Hold));
        }
    }

    public override void OnBatteryLevelUpdated(Flic2Button? button, BatteryLevel? level)
    {
        BatteryLevelChanged?.Invoke(this, level?.Voltage);
    }

    public override void OnNameUpdated(Flic2Button? button, string? newName)
    {
        NameChanged?.Invoke(this, newName);
    }

    public override void OnUnpaired(Flic2Button? button)
    {
        Unparied?.Invoke(this, EventArgs.Empty);
    }

    public override void OnConnect(Flic2Button? button)
    {
        Connected?.Invoke(this, EventArgs.Empty);
    }

    public override void OnDisconnect(Flic2Button? button)
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    public override void OnFailure(Flic2Button? button, int errorCode, int subCode)
    {
        Failure?.Invoke(this, (errorCode, subCode));
    }

    public override void OnReady(Flic2Button? button, long timestamp)
    {
        IsReadyChanged?.Invoke(this, timestamp);
    }
}