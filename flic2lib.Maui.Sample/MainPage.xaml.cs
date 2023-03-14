namespace flic2lib.Maui.Sample;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly Bluetooth _bluetooth;
    private readonly IFlicManager _flicManager;
    private readonly IFlicButtonHandler _flicButtonHandler;

    public MainPage(Bluetooth bluetooth, IFlicManager flicManager, IFlicButtonHandler flicButtonHandler)
    {
        InitializeComponent();
        _bluetooth = bluetooth;
        _flicManager = flicManager;
        _flicButtonHandler = flicButtonHandler;
    }

    private async void OnInitClicked(object sender, EventArgs e)
    {
        try
        {
            await _bluetooth.CheckOrRequestAccessAsync();
            //FlicManager.Init();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void OnFindClicked(object sender, EventArgs e)
    {
        _flicManager.StartScan();
    }

    private void OnStartListenersClicked(object sender, EventArgs e)
    {
        _flicButtonHandler.ButtonClick += OnButtonClicked;
        foreach(var btn in _flicManager.Buttons)
        {
            if (btn.ConnectionState == FlicButtonConnectionState.Disconnected)
            {
                btn.Connect();
            }
        }
    }

    private void OnButtonClicked(object? sender, FlicButtonClickEvent e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";
    }
}