using Flic2lib.iOS;
using Foundation;

namespace flic2lib.Maui;

public partial class FlicManager
{
    public static partial void Init()
    {
        if (IsInitialized) return;
        FLICManager.ConfigureWithDelegate(null, FlicButtonHandler.Instance, true);
        IsInitialized = true;
    }

    public partial void ForgetButton(FlicButton button)
    {
        if (!IsInitialized) return;

        FLICManager.SharedManager()?.ForgetButton((FLICButton)button.GetInternalButton(), (_, __) => { });
    }

    public partial void StartScan()
    {
        if (!IsInitialized) return;

        FLICManager.SharedManager()?.ScanForButtonsWithStateChangeHandler(ScanStateHandler, ScanCompletionHandler);
    }

    private void ScanStateHandler(FLICButtonScannerStatusEvent obj)
    {
        // ignored
    }

    private void ScanCompletionHandler(FLICButton button, NSError error)
    {
        if (error != null)
        {
            // TODO: test on ios
            ScanFailed?.Invoke(this, new FlicScanFailedEvent((FlicScanResult)error.Code));
            return;
        }
        if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == button.Uuid) is FlicButton { } cachedButton)
        {
            _cachedButtons.Remove(cachedButton);
        }
        var btn = new FlicButton(button);
        _cachedButtons.Add(btn);
        ButtonDiscovered?.Invoke(this, new FlicScanButtonDiscoveredEvent(btn));
        ScanEnded?.Invoke(this, new FlicScanEndedEvent(FlicScanResult.SCAN_RESULT_SUCCESS));
    }

    public partial void StopScan()
    {
        if (!IsInitialized) return;

        FLICManager.SharedManager()?.StopScan();
    }

    private partial IEnumerable<FlicButton>? GetButtons()
    {
        if (!IsInitialized) return null;

        if (_cachedButtons.Any())
        {
            var allButtons = FLICManager.SharedManager()?.Buttons?.ToList() ?? Enumerable.Empty<FLICButton>();
            foreach (var btn in allButtons)
            {
                if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == btn.Uuid) == null)
                {
                    _cachedButtons.Add(new FlicButton(btn));
                }
            }
        }

        if (!_cachedButtons.Any())
        {
            var btns = FLICManager.SharedManager()?.Buttons?.Select(btn => new FlicButton(btn))?.ToList() ?? Enumerable.Empty<FlicButton>();
            foreach (var btn in btns)
            {
                _cachedButtons.Add(btn);
            }
        }

        return _cachedButtons;
    }

    public partial FlicButton? GetButtonByUuid(string? uuid)
    {
        if (!IsInitialized) return null;

        var btn = _cachedButtons?.FirstOrDefault(b => b.Uuid == uuid);
        if (btn == null)
        {
            btn = FLICManager.SharedManager()?.Buttons?.FirstOrDefault(b => b.Uuid == uuid) is FLICButton flic2Button ? new FlicButton(flic2Button) : null;
            if (btn != null)
            {
                _cachedButtons?.Add(btn);
            }
        }
        return btn;
    }
}