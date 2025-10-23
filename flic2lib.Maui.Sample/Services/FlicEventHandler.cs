using flic2lib.Maui.Sample.Services;

namespace flic2lib.Maui.Sample.Services;

public class FlicEventHandler
{
    private readonly IIncidentService _incidentService;
    private readonly IFlicButtonHandler _flicButtonHandler;
    private readonly Dictionary<string, DateTime> _lastClickTimes = new();
    private readonly Dictionary<string, int> _clickCounts = new();
    private const int DOUBLE_CLICK_TIMEOUT_MS = 500; // Maximum time between clicks for double-click

    public FlicEventHandler(IIncidentService incidentService, IFlicButtonHandler flicButtonHandler)
    {
        _incidentService = incidentService;
        _flicButtonHandler = flicButtonHandler;
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        // Handle double-click events (this is the primary method)
        _flicButtonHandler.ButtonDoubleClick += OnButtonDoubleClick;

        // Handle single clicks for other actions
        _flicButtonHandler.ButtonClick += OnButtonClick;

        // Handle hold events for additional functionality
        _flicButtonHandler.ButtonHold += OnButtonHold;

        // Handle connection events
        _flicButtonHandler.Connected += OnButtonConnected;
        _flicButtonHandler.Disconnected += OnButtonDisconnected;
    }

    private async void OnButtonDoubleClick(object? sender, FlicButtonDoubleClickEvent e)
    {
        var buttonId = e.Button?.Uuid ?? "Unknown";

        System.Diagnostics.Debug.WriteLine($"Double-click detected on button {buttonId}");

        // Show immediate feedback
        await ShowIncidentRaisingFeedback(buttonId);

        // Raise incident via API
        var success = await _incidentService.RaiseIncidentAsync(buttonId, "Emergency");

        // Show result feedback
        await ShowIncidentResult(buttonId, success);
    }

    private void OnButtonClick(object? sender, FlicButtonClickEvent e)
    {
        var buttonId = e.Button?.Uuid ?? "Unknown";
        System.Diagnostics.Debug.WriteLine($"Single click detected on button {buttonId}");

        // You can add other single-click actions here
        // For example: toggle lights, play music, etc.
    }

    private void OnButtonHold(object? sender, FlicButtonHoldEvent e)
    {
        var buttonId = e.Button?.Uuid ?? "Unknown";
        System.Diagnostics.Debug.WriteLine($"Hold detected on button {buttonId}");

        // You can add hold actions here
        // For example: start voice recording, activate panic mode, etc.
    }

    private void OnButtonConnected(object? sender, FlicButtonConnectedEvent e)
    {
        var buttonId = e.Button?.Uuid ?? "Unknown";
        System.Diagnostics.Debug.WriteLine($"Button {buttonId} connected");

        // Optional: Show connection status in UI
    }

    private void OnButtonDisconnected(object? sender, FlicButtonDisconnectedEvent e)
    {
        var buttonId = e.Button?.Uuid ?? "Unknown";
        System.Diagnostics.Debug.WriteLine($"Button {buttonId} disconnected");

        // Optional: Show disconnection status in UI
    }

    private async Task ShowIncidentRaisingFeedback(string buttonId)
    {
        // Show immediate feedback to user
        await Application.Current.MainPage.DisplayAlert(
            "Emergency",
            $"Raising incident for button {buttonId.Substring(0, 8)}...",
            "OK");
    }

    private async Task ShowIncidentResult(string buttonId, bool success)
    {
        var message = success
            ? $"Emergency incident raised successfully for button {buttonId.Substring(0, 8)}"
            : $"Failed to raise incident for button {buttonId.Substring(0, 8)}. Please try again.";

        var title = success ? "Incident Raised" : "Error";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public void Dispose()
    {
        // Unsubscribe from events
        _flicButtonHandler.ButtonDoubleClick -= OnButtonDoubleClick;
        _flicButtonHandler.ButtonClick -= OnButtonClick;
        _flicButtonHandler.ButtonHold -= OnButtonHold;
        _flicButtonHandler.Connected -= OnButtonConnected;
        _flicButtonHandler.Disconnected -= OnButtonDisconnected;
    }
}