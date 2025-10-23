using Android.OS;
using Flic2lib.Android;
using flic2lib.android;

namespace flic2lib.Maui;

public partial class FlicManager
{
    public static partial void Init()
    {
        if (IsInitialized) return;

        // TODO: Implement proper initialization using GetInstance with callbacks
        // FlicManager.GetInstance(Platform.AppContext, new InitializedCallback());
        IsInitialized = true;
    }

    private partial IEnumerable<FlicButton>? GetButtons()
    {
        if (!IsInitialized) return null;

        if (_cachedButtons.Any())
        {
            // TODO: Update to use proper FlicManager instance from GetInstance callback
            // var allButtons = FlicManager.Instance?.Buttons?.ToList() ?? Enumerable.Empty<FlicButton>();
            var allButtons = Enumerable.Empty<Flic2lib.Android.FlicButton>();
            foreach (var btn in allButtons)
            {
                if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == btn.ButtonId) == null)
                {
                    _cachedButtons.Add(new FlicButton(btn));
                }
            }
        }

        if (!_cachedButtons.Any())
        {
            // TODO: Update to use proper FlicManager instance from GetInstance callback
            // var btns = FlicManager.Instance?.Buttons?.Select(btn => new flic2lib.Maui.FlicButton(btn))?.ToList() ?? Enumerable.Empty<flic2lib.Maui.FlicButton>();
            var btns = Enumerable.Empty<flic2lib.Maui.FlicButton>();
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
            // TODO: Update to use proper FlicManager instance from GetInstance callback
            // btn = FlicManager.Instance?.Buttons?.FirstOrDefault(b => b.Uuid == uuid) is Flic2lib.Android.FlicButton flicButton ? new flic2lib.Maui.FlicButton(flicButton) : null;
            btn = null;
            if (btn != null)
            {
                _cachedButtons?.Add(btn);
            }
        }
        return btn;
    }

    public partial void ForgetButton(FlicButton button)
    {
        if (!IsInitialized) return;

        _cachedButtons.Remove(button);
        // TODO: Update to use proper FlicManager instance from GetInstance callback
        // FlicManager.Instance?.ForgetButton((Flic2lib.Android.FlicButton)button.GetInternalButton());
    }

    public partial void StartScan()
    {
        if (!IsInitialized) return;

        // TODO: Update to use current Flic library scanning approach
        // FlicManager.Instance?.StartScan(ScanCallbacks.Instance);
    }

    internal void OnScanComplete(int result, int subCode, Flic2lib.Android.FlicButton? button)
    {
        if (result == (int)FlicScanResult.SCAN_RESULT_SUCCESS)
        {
            if (button == null) return;
            if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == button?.ButtonId) is FlicButton { } cachedButton)
            {
                _cachedButtons.Remove(cachedButton);
            }
            var btn = new flic2lib.Maui.FlicButton(button);
            _cachedButtons.Add(btn);
            ButtonDiscovered?.Invoke(this, new FlicScanButtonDiscoveredEvent(btn));
            ScanEnded?.Invoke(this, new FlicScanEndedEvent((FlicScanResult)result));
        }
        else
        {
            // Failed
            ScanFailed?.Invoke(this, new FlicScanFailedEvent((FlicScanResult)result));
        }
    }

    public partial void StopScan()
    {
        if (!IsInitialized) return;

        // TODO: Update to use current Flic library scanning approach
        // FlicManager.Instance?.StopScan();
    }

    public class ScanCallbacks : Java.Lang.Object // TODO: implement proper interface
    {
        private readonly static Lazy<ScanCallbacks> _lazyInstance = new(() => new ScanCallbacks(), LazyThreadSafetyMode.ExecutionAndPublication);
        internal static ScanCallbacks Instance => _lazyInstance.Value;
        private ScanCallbacks() { }

        public void OnComplete(int result, int subCode, Flic2lib.Android.FlicButton? button)
        {
            // TODO: Update for new Flic library
            // FlicManager.Instance.OnScanComplete(result, subCode, button);
        }

        public void OnConnected()
        {
            // ignored
        }

        public void OnDiscovered(string? bdAddr)
        {
            // ignored
        }

        public void OnDiscoveredAlreadyPairedButton(Flic2lib.Android.FlicButton? button)
        {
            // ignored
        }
    }
}