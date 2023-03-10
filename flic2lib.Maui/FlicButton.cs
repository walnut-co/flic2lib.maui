namespace flic2lib.Maui;

public enum FlicButtonConnectionState
{
    Disconnected = 0,
    Connecting,
    Connected,
    Disconnecting
}

public class FlicButtonClick
{

}

public partial class FlicButton
{
    public event EventHandler Connected;
    public event EventHandler Disconnected;
    public event EventHandler Unparied;
    public event EventHandler<(int errorCode, int subCode)> Failure;
    public event EventHandler<long> IsReadyChanged;
    public event EventHandler<string> NameChanged;
    public event EventHandler<float?> BatteryLevelChanged;

    public string BluetoothAddress => GetBluetoothAddress();
    public string Name => GetName();
    public string NickName { get => GetNickName(); set => SetNickName(value); }
    public string SerialNumber => GetSerialNumber();
    public string Uuid => GetUuid();
    public int FirmwareVersion => GetFirmwareVersion();
    public int PressCount => GetPressCount();
    public float? BatteryVoltage => GetBatteryVoltage();
    public bool IsReady => GetIsReady();
    public bool IsUnpaired => GetIsUnpaired();
    public string Identifier => GetIdentifier();
    public FlicButtonConnectionState ConnectionState => GetConnectionState();

    /// <summary>
    /// Gets the underlying platform button impl
    /// </summary>
    /// <returns></returns>
    public partial object GetInternalButton();
    public partial void Connect();
    public partial void Disconnect();
    private partial string GetBluetoothAddress();
    private partial string GetName();
    private partial string GetNickName();
    private partial void SetNickName(string value);
    private partial string GetSerialNumber();
    private partial string GetUuid();
    private partial int GetFirmwareVersion();
    private partial int GetPressCount();
    private partial float? GetBatteryVoltage();
    private partial bool GetIsReady();
    private partial bool GetIsUnpaired();
    private partial string GetIdentifier();
    private partial FlicButtonConnectionState GetConnectionState();
}