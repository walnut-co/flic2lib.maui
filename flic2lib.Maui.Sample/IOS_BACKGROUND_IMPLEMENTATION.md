# iOS Background Event Handling Implementation

## Overview
This document explains how iOS background event handling works for Flic buttons and how it differs from Android implementation.

## iOS Background Limitations
iOS has strict background execution limits compared to Android:
- **30 seconds to 3 minutes**: Background execution time after app backgrounding
- **No unlimited background**: Unlike Android foreground services
- **System termination**: iOS may terminate apps to save battery
- **Background modes required**: Must be declared in Info.plist

## Implementation Architecture

### 1. AppDelegate Enhancement
The `AppDelegate.cs` has been enhanced with comprehensive background handling:

```csharp
// Key lifecycle methods implemented:
- FinishedLaunching()     // App startup initialization
- DidEnterBackground()    // When app goes to background
- WillEnterForeground()   // When app returns from background
- DidBecomeActive()       // When app becomes active
- PerformFetch()          // Background fetch events
```

### 2. Background Modes (Info.plist)
Required background modes are already configured:
- `bluetooth-central`: Maintains Bluetooth connections
- `background-processing`: Allows background tasks
- `background-fetch`: Enables periodic background refresh

### 3. Event Flow Diagram

```
App Launch
    ↓
FinishedLaunching()
    ↓
Configure.KeepFlicButtonsConnected()
    ↓
InitializeFlicEventHandler()
    ↓
[App Running - Flic Events Work Normally]
    ↓
User Backgrounds App
    ↓
DidEnterBackground()
    ↓
EnsureBackgroundEventHandling()
    ↓
BeginBackgroundTask() → [30s-3min execution]
    ↓
[Limited Background - Flic Events Still Work]
    ↓
Background Task Expires OR User Returns
    ↓
WillEnterForeground()
    ↓
Re-initialize Connections
    ↓
[App Active - Full Flic Functionality]
```

## Key Components

### 1. FlicEventHandler Initialization
```csharp
private void InitializeFlicEventHandler()
{
    try
    {
        // Get FlicEventHandler from DI container
        var mauiApp = (Application.Current as App)?.Services;
        if (mauiApp != null)
        {
            _flicEventHandler = mauiApp.GetService<FlicEventHandler>();
            // Handler now ready for background events
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"iOS: Error: {ex.Message}");
    }
}
```

### 2. Background Task Management
```csharp
private void EnsureBackgroundEventHandling()
{
    // Request extended background execution
    var taskId = UIApplication.SharedApplication.BeginBackgroundTask(() => {
        System.Diagnostics.Debug.WriteLine("iOS: Background task expired");
    });

    // Handle events for 25 seconds (before iOS kills us)
    if (taskId != UIApplication.BackgroundTaskInvalid)
    {
        DispatchQueue.MainQueue.DispatchAfter(
            new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(25)), 
            () => {
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
    }
}
```

### 3. Background Fetch Support
```csharp
public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
{
    // iOS can wake our app periodically to refresh connections
    Configure.KeepFlicButtonsConnected();
    EnsureFlicEventHandlerActive();
    completionHandler(UIBackgroundFetchResult.NewData);
}
```

## How Events Work in Background

### Scenario 1: Recent Background (0-30 seconds)
```
1. User clicks Flic button
2. iOS delivers event to app (still in memory)
3. FlicEventHandler receives event
4. Emergency incident created successfully
5. HTTP request sent to API
```

### Scenario 2: Extended Background (30s-3min)
```
1. User clicks Flic button
2. iOS may wake app with background task
3. FlicEventHandler processes event quickly
4. Emergency incident handled within time limit
5. App may be suspended again
```

### Scenario 3: Deep Background (3+ minutes)
```
1. User clicks Flic button
2. iOS may not deliver event (app terminated)
3. OR iOS wakes app with background fetch
4. Connection re-established
5. Event processing depends on iOS decision
```

## Comparison: iOS vs Android

| Feature | iOS Implementation | Android Implementation |
|---------|-------------------|----------------------|
| **Background Time** | 30s-3min limited | Unlimited (foreground service) |
| **Event Reliability** | High (recent), Medium (extended) | Very High (always) |
| **Battery Impact** | Minimal (iOS managed) | Low (controlled foreground service) |
| **User Experience** | May miss events if app terminated | Never misses events |
| **Setup Complexity** | Medium (background modes) | Medium (service configuration) |

## Testing Scenarios

### Test 1: Recent Background
```
1. Open app, connect Flic button
2. Press home button (app backgrounded)
3. Within 30 seconds, press Flic button
4. Expected: Event processed, emergency created
```

### Test 2: Extended Background
```
1. Open app, connect Flic button
2. Background app for 2 minutes
3. Press Flic button
4. Expected: Event may be processed (iOS dependent)
```

### Test 3: Background Fetch
```
1. Background app for 10+ minutes
2. iOS triggers background fetch
3. Press Flic button during/after fetch
4. Expected: Improved event processing chance
```

## Optimization Tips

### 1. User Education
Inform users that iOS background limitations mean:
- Events work best when app recently backgrounded
- For critical use, keep app in foreground
- iOS may occasionally miss events

### 2. App Store Guidelines
Ensure background modes are justified:
- `bluetooth-central`: "Maintains connection to Flic emergency button"
- `background-processing`: "Processes emergency incidents"
- `background-fetch`: "Updates emergency system connectivity"

### 3. Alternative Strategies
Consider these approaches for critical scenarios:
- **Local Notifications**: Alert user to open app
- **Shortcuts Integration**: Siri shortcuts for emergency
- **Apple Watch**: Companion app for better background
- **Critical Alerts**: Special notification permissions

## Debugging

### Debug Output
The implementation includes extensive debug logging:
```csharp
System.Diagnostics.Debug.WriteLine("iOS: App entered background - Flic events will work for limited time");
System.Diagnostics.Debug.WriteLine("iOS: Background task started for Flic event handling");
System.Diagnostics.Debug.WriteLine("iOS: FlicEventHandler initialized successfully");
```

### Testing Tools
1. **Xcode Console**: View debug output
2. **Background App Refresh**: iOS Settings > General > Background App Refresh
3. **iOS Simulator**: Test background scenarios
4. **Battery Usage**: Monitor background activity

## Troubleshooting

### Issue: Events Not Working in Background
**Possible Causes:**
1. Background App Refresh disabled
2. App terminated by iOS
3. Bluetooth connection lost
4. Background modes not configured

**Solutions:**
1. Check iOS Settings > General > Background App Refresh
2. Ensure Info.plist has required background modes
3. Re-implement connection logic in WillEnterForeground
4. Add more aggressive reconnection attempts

### Issue: App Terminated Quickly
**Possible Causes:**
1. Excessive background processing
2. Memory usage too high
3. iOS low power mode

**Solutions:**
1. Minimize background work
2. End background tasks promptly
3. Optimize memory usage

## Best Practices

### 1. Quick Background Processing
```csharp
// Process events quickly in background
private async void HandleFlicEvent()
{
    try
    {
        // Minimal processing only
        await emergencyService.CreateIncident();
    }
    catch
    {
        // Handle silently, don't crash
    }
}
```

### 2. Graceful Degradation
```csharp
// Accept that some events may be missed
if (backgroundTimeRemaining < 5.0)
{
    // Queue for foreground processing
    pendingEvents.Add(eventData);
}
```

### 3. User Feedback
```csharp
// Inform user of background limitations
if (UIApplication.SharedApplication.BackgroundTimeRemaining < 30.0)
{
    // Show notification about bringing app to foreground
}
```

## Conclusion

The iOS implementation provides **best-effort** background event handling within iOS system limitations. While it cannot match Android's unlimited background execution, it maximizes the chances of handling Flic button events when the app is backgrounded.

For critical emergency scenarios, users should be educated about iOS limitations and encouraged to keep the app active or use alternative triggering methods when possible.