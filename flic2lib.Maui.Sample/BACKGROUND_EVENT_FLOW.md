# Background Flic Button Event Flow

This document explains how Flic button events are handled when the app is in the background.

## Architecture Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────────┐    ┌─────────────────┐
│ Flic Button     │───▶│ FlicManager      │───▶│ IFlicButtonHandler  │───▶│ FlicEventHandler│
│ (Physical)      │    │ (Native Library) │    │ (Event Dispatcher)  │    │ (Your Logic)    │
└─────────────────┘    └──────────────────┘    └─────────────────────┘    └─────────────────┘
                                                                                       │
                                                                                       ▼
                                                                            ┌─────────────────┐
                                                                            │ IncidentService │
                                                                            │ (API Calls)     │
                                                                            └─────────────────┘
```

## Component Responsibilities

### 1. FlicBackgroundService (Android Foreground Service)
- **Location**: `Platforms/Android/FlicBackgroundService.cs`
- **Purpose**: Keeps the app process alive in the background
- **Key Functions**:
  - Calls `FlicManager.Init()` to initialize native Flic library
  - Ensures `FlicEventHandler` is initialized and listening
  - Shows persistent notification to prevent system kill
  - Auto-restarts if killed by system (StartCommandResult.Sticky)

### 2. FlicManager (Native Library Wrapper)
- **Location**: `flic2lib.Maui` package
- **Purpose**: Manages connection to physical Flic buttons
- **Key Functions**:
  - Maintains Bluetooth connections to paired buttons
  - Receives raw button events from hardware
  - Dispatches events through `IFlicButtonHandler`

### 3. IFlicButtonHandler (Event Dispatcher)
- **Location**: `flic2lib.Maui` package
- **Purpose**: Event subscription interface
- **Key Events**:
  - `ButtonClick` - Single click
  - `ButtonDoubleClick` - Double click (your emergency trigger)
  - `ButtonHold` - Long press
  - `Connected`/`Disconnected` - Connection status

### 4. FlicEventHandler (Your Business Logic)
- **Location**: `Services/FlicEventHandler.cs`
- **Purpose**: Handle button events and trigger actions
- **Key Functions**:
  - Subscribes to all button events
  - Handles double-click → emergency incident
  - Calls `IncidentService` to raise API alerts
  - Shows user feedback

### 5. IncidentService (API Integration)
- **Location**: `Services/IncidentService.cs`
- **Purpose**: Make HTTP calls to your incident API
- **Key Functions**:
  - `RaiseIncidentAsync()` - Posts emergency to your API
  - Includes button ID, timestamp, device info

## Event Flow When App is Backgrounded

### Step-by-Step Process:

1. **User backgrounds the app**
   - `FlicBackgroundService` continues running as foreground service
   - `FlicManager` maintains Bluetooth connections
   - `FlicEventHandler` singleton remains in memory

2. **User double-clicks Flic button**
   - Physical button sends Bluetooth signal
   - `FlicManager` receives hardware event
   - Event flows through `IFlicButtonHandler`

3. **FlicEventHandler processes event**
   ```csharp
   private async void OnButtonDoubleClick(object? sender, FlicButtonDoubleClickEvent e)
   {
       var buttonId = e.Button?.Uuid ?? "Unknown";
       
       // Show immediate feedback (if app is visible)
       await ShowIncidentRaisingFeedback(buttonId);
       
       // Call your incident API
       var success = await _incidentService.RaiseIncidentAsync(buttonId, "Emergency");
       
       // Show result (if app is visible)
       await ShowIncidentResult(buttonId, success);
   }
   ```

4. **IncidentService makes API call**
   - HTTP POST to your emergency endpoint
   - Includes button ID, timestamp, device info
   - Returns success/failure status

5. **User feedback (if app is foreground)**
   - Shows alert dialogs for immediate feedback
   - If backgrounded, notifications could be used instead

## Key Implementation Details

### Singleton Pattern
All services are registered as singletons in `MauiProgram.cs`:
```csharp
builder.Services
    .AddSingleton<IIncidentService, IncidentService>()
    .AddSingleton<FlicEventHandler>()
```

This ensures:
- One instance handles all button events
- Event subscriptions persist throughout app lifecycle
- No duplicate API calls from multiple instances

### Service Initialization
The background service ensures event handling is active:
```csharp
private void EnsureEventHandlerInitialized()
{
    // Gets FlicEventHandler from DI container
    // Ensures it's constructed and event subscriptions are active
    var flicEventHandler = services.GetService<FlicEventHandler>();
}
```

### Platform Differences

**Android (Full Background Support)**:
- Foreground service keeps process alive indefinitely
- All events work normally when backgrounded
- No system limitations on event processing

**iOS (Limited Background Support)**:
- Background execution limited to 30 seconds - 3 minutes
- System may suspend app and miss events
- Requires "Background App Refresh" enabled by user

## Testing the Background Flow

### 1. Build and Deploy
```bash
dotnet build flic2lib.Maui.Sample/flic2lib.Maui.Sample.csproj -c Release
```

### 2. Start the App
- Tap "Initialize Flic Service" 
- Tap "Find Buttons" and pair your Flic
- Tap "Start Listeners"

### 3. Background the App
- Press home button or switch to another app
- Verify persistent notification shows "Flic Buttons Connected"

### 4. Test Button Events
- Double-click your Flic button
- Check debug logs for event processing
- Verify API call in your incident system

### 5. Monitor Status
- Return to app and tap "Background Service Info"
- Check service status and optimization tips

## Troubleshooting

### Events Not Working in Background
1. Check if foreground service is running (notification visible)
2. Verify FlicEventHandler is initialized (check debug logs)
3. Ensure button is connected (`btn.ConnectionState`)
4. Check battery optimization settings

### API Calls Failing
1. Verify network connectivity in background
2. Check IncidentService configuration
3. Review HTTP client timeout settings
4. Monitor exception logs in IncidentService

### Service Killed by System
1. Check if app is battery optimized (disable it)
2. Verify notification channel importance (Low/Default)
3. Review foreground service type permissions
4. Consider implementing service restart logic

## Customizing Event Handling

### Adding New Button Actions
```csharp
private void OnButtonClick(object? sender, FlicButtonClickEvent e)
{
    // Add your single-click logic
    // Example: Toggle smart lights, play music, etc.
}

private void OnButtonHold(object? sender, FlicButtonHoldEvent e)
{
    // Add your hold logic  
    // Example: Start voice recording, activate panic mode
}
```

### Customizing Emergency Response
```csharp
private async void OnButtonDoubleClick(object? sender, FlicButtonDoubleClickEvent e)
{
    var buttonId = e.Button?.Uuid ?? "Unknown";
    
    // Your custom emergency logic
    await _incidentService.RaiseIncidentAsync(buttonId, "Emergency");
    
    // Additional actions:
    // - Send SMS to emergency contacts
    // - Trigger local alarm sound
    // - Activate camera recording
    // - Share GPS location
}
```

This architecture ensures your emergency system works reliably even when the app isn't visible to the user.