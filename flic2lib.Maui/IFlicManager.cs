namespace flic2lib.Maui;

public interface IFlicManager
{
    IEnumerable<FlicButton>? Buttons { get; }
    void ForgetButton(FlicButton button);
    FlicButton? GetButtonByUuid(string? uuid);
    void StartScan();
    void StopScan();
}