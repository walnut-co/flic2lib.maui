using Flic2lib.iOS;

namespace flic2lib.Maui;

public partial class FlicButton
{
    private readonly FLICButton _button;

    public FlicButton(FLICButton fLICButton)
    {
        _button = fLICButton;
        _button.Delegate = FlicButtonHandler.Instance;
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
        _button.Delegate = FlicButtonHandler.Instance;
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
}