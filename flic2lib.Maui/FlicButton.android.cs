using Flic2lib.Android;

namespace flic2lib.Maui;

public partial class FlicButton
{
    private readonly Flic2lib.Android.FlicButton _button;

    public FlicButton(Flic2lib.Android.FlicButton flicButton)
    {
        _button = flicButton;
        FlicButtonHandler.Instance.InitializeAndroid();
        _button.AddFlicButtonCallback(FlicButtonHandler.Instance.GetAndroidCallback());
    }

    public partial void Connect()
    {
        // TODO: Check if there's a Connect method in the new API
        // _button.Connect();
    }

    public partial void Disconnect()
    {
        // TODO: Check if there's a Disconnect method in the new API
        // _button.DisconnectOrAbortPendingConnection();
    }

    public partial void StartListen()
    {
        FlicButtonHandler.Instance.InitializeAndroid();
        _button.AddFlicButtonCallback(FlicButtonHandler.Instance.GetAndroidCallback());
    }

    public partial void StopListen()
    {
        _button.RemoveAllFlicButtonCallbacks();
    }

    public partial object GetInternalButton() => _button;
    private partial string? GetBluetoothAddress() => _button.ButtonId;
    private partial string? GetName() => _button.Name;
    private partial string? GetNickName() => _button.Name;
    private partial void SetNickName(string? value) { /* TODO: Check if Name can be set */ }
    private partial string? GetSerialNumber() => _button.ButtonId; // Using ButtonId as equivalent
    private partial string? GetUuid() => _button.ButtonId; // Using ButtonId as equivalent
    private partial int GetFirmwareVersion() => 0; // TODO: Check if available in new API
    private partial int GetPressCount() => 0; // TODO: Check if available in new API
    private partial float? GetBatteryVoltage() => null; // TODO: Check if available in new API
    private partial bool GetIsReady() => _button.ConnectionStatus == 2; // Assuming 2 means ready
    private partial bool GetIsUnpaired() => _button.ConnectionStatus == 0; // Assuming 0 means unpaired
    private partial string? GetIdentifier() => _button.ButtonId;
    private partial FlicButtonConnectionState GetConnectionState() => (FlicButtonConnectionState)_button.ConnectionStatus;
}