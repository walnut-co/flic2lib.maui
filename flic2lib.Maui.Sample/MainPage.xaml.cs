using flic2lib.Maui.Sample.Services;

namespace flic2lib.Maui.Sample;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly Bluetooth _bluetooth;
    private readonly IFlicManager _flicManager;
    private readonly IFlicButtonHandler _flicButtonHandler;
    private readonly FlicEventHandler _flicEventHandler;
    private readonly BackgroundServiceManager _backgroundServiceManager;

    public MainPage(Bluetooth bluetooth, IFlicManager flicManager, IFlicButtonHandler flicButtonHandler, FlicEventHandler flicEventHandler, BackgroundServiceManager backgroundServiceManager)
    {
        InitializeComponent();
        _bluetooth = bluetooth;
        _flicManager = flicManager;
        _flicButtonHandler = flicButtonHandler;
        _flicEventHandler = flicEventHandler;
        _backgroundServiceManager = backgroundServiceManager;
    }

    private async void OnInitClicked(object sender, EventArgs e)
    {
        try
        {
            await _bluetooth.CheckOrRequestAccessAsync();

            // Background service is automatically started by MainActivity on Android
            // iOS background connectivity is handled by the Configure.KeepFlicButtonsConnected() in AppDelegate

            await DisplayAlert("Success",
                "Flic2 service initialized with background support.\n\n" +
                "• Android: Background service running for unlimited connectivity\n" +
                "• iOS: Background connectivity enabled (limited by iOS)",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to initialize: {ex.Message}", "OK");
        }
    }

    private void OnFindClicked(object sender, EventArgs e)
    {
        _flicManager.StartScan();
        DisplayAlert("Scanning", "Looking for Flic buttons...", "OK");
    }

    private async void OnStartListenersClicked(object sender, EventArgs e)
    {
        // The FlicEventHandler is already set up in the constructor
        // Just need to connect the buttons
        var connectedCount = 0;

        foreach (var btn in _flicManager.Buttons ?? Enumerable.Empty<FlicButton>())
        {
            if (btn.ConnectionState == FlicButtonConnectionState.Disconnected)
            {
                btn.Connect();
                btn.StartListen(); // Ensure we're listening for events
                connectedCount++;
            }
        }

        await DisplayAlert("Listeners Started",
            $"Connected to {connectedCount} buttons.\n\n" +
            "• Single click: Basic action\n" +
            "• Double click: Raise emergency incident\n" +
            "• Hold: Additional actions",
            "OK");
    }

    private async void OnBackgroundInfoClicked(object sender, EventArgs e)
    {
        var status = _backgroundServiceManager.GetBackgroundStatus();
        var details = _backgroundServiceManager.GetImplementationDetails();
        var optimization = _backgroundServiceManager.GetOptimizationInstructions();

        await DisplayAlert("Background Service Status",
            $"{status}\n\n" +
            $"Implementation:\n{details}\n\n" +
            $"Optimization:\n{optimization}",
            "OK");
    }

    private void OnButtonClicked(object? sender, FlicButtonClickEvent e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        // This is now handled by FlicEventHandler, but keeping for demo
        var buttonId = e.Button?.Uuid?.Substring(0, 8) ?? "Unknown";
        DisplayAlert("Button Clicked", $"Button {buttonId} was clicked", "OK");
    }
}