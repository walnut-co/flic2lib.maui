using Flic2lib.iOS;
using Foundation;

namespace flic2lib.Maui;

public partial class FlicButton : FLICButtonDelegate
{
    private readonly FLICButton _button;

    public FlicButton(FLICButton fLICButton)
    {
        _button = fLICButton;
        _button.Delegate = this;
    }

    public partial void Connect()
    {
        _button.Connect();
    }

    public partial void Disconnect()
    {
        _button.Disconnect();
    }

    public partial void StartListen()
    {
        _button.Delegate = this;
    }

    public partial void StopListen()
    {
        _button.Delegate = null;
    }

    public partial object GetInternalButton() => _button;
    private partial string? GetBluetoothAddress() => _button.BluetoothAddress;
    private partial string? GetName() => _button.Name;
    private partial string? GetNickName() => _button.Nickname;
    private partial void SetNickName(string? value) => _button.Nickname = value;
    private partial string? GetSerialNumber() => _button.SerialNumber;
    private partial string? GetUuid() => _button.Uuid;
    private partial int GetFirmwareVersion() => (int)_button.FirmwareRevision;
    private partial int GetPressCount() => (int)_button.PressCount;
    private partial float? GetBatteryVoltage() => _button.BatteryVoltage;
    private partial bool GetIsReady() => _button.IsReady;
    private partial bool GetIsUnpaired() => _button.IsUnpaired;
    private partial string? GetIdentifier() => _button.Identifier.AsString();
    private partial FlicButtonConnectionState GetConnectionState() => (FlicButtonConnectionState)_button.State;

    public override void ButtonDidConnect(FLICButton button)
    {
        Connected?.Invoke(this, EventArgs.Empty);
    }

    public override void ButtonIsReady(FLICButton button)
    {
        IsReadyChanged?.Invoke(this, DateTime.Now.Ticks);
    }

    public override void DidDisconnectWithError(FLICButton button, NSError? error)
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    public override void DidFailToConnectWithError(FLICButton button, NSError? error)
    {
        Failure?.Invoke(this, (error != null ? (int)error.Code : 0, 0));
    }

    public override void DidReceiveButtonDown(FLICButton button, bool queued, nint age)
    {
        ButtonDown?.Invoke(this, new FlicButtonEvent(age, queued, ClickActions.Down));
    }

    public override void DidReceiveButtonUp(FLICButton button, bool queued, nint age)
    {
        ButtonUp?.Invoke(this, new FlicButtonEvent(age, queued, ClickActions.Up));
    }

    public override void DidReceiveButtonClick(FLICButton button, bool queued, nint age)
    {
        ButtonClick?.Invoke(this, new FlicButtonEvent(age, queued, ClickActions.SingleClick));
    }

    public override void DidReceiveButtonDoubleClick(FLICButton button, bool queued, nint age)
    {
        ButtonDoubleClick?.Invoke(this, new FlicButtonEvent(age, queued, ClickActions.DoubleClick));
    }

    public override void DidReceiveButtonHold(FLICButton button, bool queued, nint age)
    {
        ButtonHold?.Invoke(this, new FlicButtonEvent(age, queued, ClickActions.Hold));
    }

    public override void DidUnpairWithError(FLICButton button, NSError? error)
    {
        Unparied?.Invoke(this, EventArgs.Empty);
    }

    public override void DidUpdateBatteryVoltage(FLICButton button, float voltage)
    {
        BatteryLevelChanged?.Invoke(this, voltage);
    }

    public override void DidUpdateNickname(FLICButton button, string nickname)
    {
        NameChanged?.Invoke(this, nickname);
    }
}