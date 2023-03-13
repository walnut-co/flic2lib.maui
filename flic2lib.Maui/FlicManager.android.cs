using Flic2lib.Android;
using Android.OS;

namespace flic2lib.Maui;

public partial class FlicManager
{
    public static partial void Init()
    {
        if (IsInitialized) return;
        Flic2Manager.Init(Platform.AppContext, new Handler(Looper.MainLooper!));
        IsInitialized = true;
    }

    private partial IEnumerable<FlicButton>? GetButtons()
    {
        if (!IsInitialized) return null;

        if (_cachedButtons.Any())
        {
            var allButtons = Flic2Manager.Instance?.Buttons?.ToList() ?? Enumerable.Empty<Flic2Button>();
            foreach(var btn in allButtons)
            {
                if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == btn.Uuid) == null)
                {
                    _cachedButtons.Add(new FlicButton(btn));
                }
            }
        }

        if (!_cachedButtons.Any())
        {
            var btns = Flic2Manager.Instance?.Buttons?.Select(btn => new FlicButton(btn))?.ToList() ?? Enumerable.Empty<FlicButton>();
            foreach(var btn in btns)
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
            btn = Flic2Manager.Instance?.Buttons?.FirstOrDefault(b => b.Uuid == uuid) is Flic2Button flic2Button ? new FlicButton(flic2Button) : null;
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
        Flic2Manager.Instance?.ForgetButton((Flic2Button)button.GetInternalButton());
    }

    public partial void StartScan()
    {
        if (!IsInitialized) return;

        Flic2Manager.Instance?.StartScan(ScanCallbacks.Instance);
    }

    internal void OnScanComplete(int result, int subCode, Flic2Button? button)
    {
        if (result == (int)FlicScanResult.SCAN_RESULT_SUCCESS)
        {
            if (button == null) return;
            // Success!
            // The button object can now be used
            if (_cachedButtons.FirstOrDefault(cb => cb.Uuid == button?.Uuid) is FlicButton { } cachedButton)
            {
                _cachedButtons.Remove(cachedButton);
            }

            _cachedButtons.Add(new FlicButton(button));
        }
        else
        {
            // Failed
            // oh no ¯\_(ツ)_ /¯
        }
    }

    public partial void StopScan()
    {
        if (!IsInitialized) return;

        Flic2Manager.Instance?.StopScan();
    }

    public class ScanCallbacks : Java.Lang.Object, IFlic2ScanCallback
    {
        public static ScanCallbacks Instance { get; } = new ScanCallbacks();
        private ScanCallbacks() { }

        public void OnComplete(int result, int subCode, Flic2Button? button)
        {
            FlicManager.Instance.OnScanComplete(result, subCode, button);
        }

        public void OnConnected()
        {
        }

        public void OnDiscovered(string? bdAddr)
        {
        }

        public void OnDiscoveredAlreadyPairedButton(Flic2Button? button)
        {
        }
    }
}