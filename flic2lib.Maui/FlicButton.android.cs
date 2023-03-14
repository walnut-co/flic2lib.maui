using Flic2lib.Android;

namespace flic2lib.Maui;

public partial class FlicButton
{
    private readonly Flic2Button _button;

    public FlicButton(Flic2Button flic2Button)
    {
        _button = flic2Button;
        _button.AddListener(FlicButtonHandler.Instance);
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
        _button.AddListener(FlicButtonHandler.Instance);
    }

    public partial void StopListen()
    {
        _button.RemoveListener(FlicButtonHandler.Instance);
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
}