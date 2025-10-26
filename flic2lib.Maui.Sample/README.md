# Flic2lib MAUI Sample Project

This sample project demonstrates how to implement **background connectivity** for Flic2 buttons in a .NET MAUI application, including **emergency incident handling** via double-click events.

## 🚀 Features Demonstrated

### ✅ **Emergency Incident Handling**
- **Double-click** any Flic button to automatically raise an emergency incident via API
- Built-in HTTP client with error handling and device info collection
- Configurable API endpoint in `appsettings.json`

### ✅ **Cross-Platform Background Connectivity**
- **Android**: Unlimited background execution with foreground service
- **iOS**: Limited background execution (30s-3min) with automatic reconnection

### ✅ **Real-World Implementation**
- **No Java callable wrapper errors** - Android components in main project
- **Template code** for easy customization
- **Production-ready** service lifecycle management

---

## 📱 Platform-Specific Implementation

### **Android Background Service**

The sample includes a complete **local implementation** of Android background services:

#### **Files Added:**
- `Platforms/Android/FlicBackgroundService.cs` - Foreground service for persistent connectivity
- `Platforms/Android/FlicBootReceiver.cs` - Auto-restart service after device reboot
- `Platforms/Android/Resources/drawable/flic_notification.xml` - Notification icon

#### **Why Local Implementation?**
✅ **Avoids build errors** - No Java callable wrapper issues  
✅ **Full customization** - Modify notification, behavior, lifecycle  
✅ **Production ready** - Proper foreground service with notification  
✅ **Auto-restart** - Service restarts after device reboot/app updates  

#### **Android Manifest Updates:**
```xml
<!-- Background service -->
<service android:name=".FlicBackgroundService"
         android:enabled="true"
         android:exported="false"
         android:foregroundServiceType="connectedDevice" />

<!-- Boot receiver -->
<receiver android:name=".FlicBootReceiver"
          android:enabled="true"
          android:exported="true">
    <intent-filter android:priority="1000">
        <action android:name="android.intent.action.BOOT_COMPLETED" />
        <action android:name="android.intent.action.MY_PACKAGE_REPLACED" />
    </intent-filter>
</receiver>

<!-- Required permissions -->
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
```

### **iOS Background Support**

iOS background connectivity is handled by the library with proper configuration:

#### **AppDelegate Setup:**
```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    var result = base.FinishedLaunching(application, launchOptions);
    
    // Enable iOS background connectivity
    Configure.KeepFlicButtonsConnected();
    
    return result;
}
```

#### **Info.plist Background Modes:**
```xml
<key>UIBackgroundModes</key>
<array>
    <string>bluetooth-central</string>
    <string>background-processing</string>
    <string>background-fetch</string>
</array>
```

---

## 🛠️ Setup Instructions

### **1. Prerequisites**
- .NET 9.0 MAUI
- Physical Flic2 buttons for testing
- Android device (API 21+) or iOS device (14.0+)

### **2. Configuration**
Update `appsettings.json` with your incident API endpoint:
```json
{
  "IncidentApi": {
    "BaseUrl": "https://your-api-endpoint.com"
  }
}
```

### **3. Android Setup**
- Deploy to Android device
- **Grant permissions** when prompted (Bluetooth, Location, Notifications)
- The background service starts automatically via `MainActivity`
- A **persistent notification** indicates the service is running

### **4. iOS Setup**
- Deploy to iOS device
- **Enable Background App Refresh** in iOS Settings → General → Background App Refresh
- Background connectivity is automatically configured

---

## 🎯 Usage Guide

### **1. Initialize Flic Service**
Tap **"Init Flic"** to:
- Request Bluetooth permissions
- Initialize the Flic manager
- Confirm background services are running

### **2. Connect Flic Buttons**
Tap **"Find Buttons"** to scan for nearby Flic2 buttons and pair them.

### **3. Start Event Listeners**
Tap **"Start Listeners"** to enable button event handling:
- **Single click**: Basic action (increments counter)
- **Double click**: **Raises emergency incident** via API
- **Hold**: Additional actions

### **4. Check Background Status**
Tap **"Background Service Info"** to see:
- Current background connectivity status
- Platform-specific implementation details
- Optimization tips for your device

---

## 🚨 Emergency Incident Handling

The sample includes a complete emergency response system:

### **How It Works:**
1. **Double-click** any connected Flic button
2. `FlicEventHandler` captures the double-click event
3. `IncidentService` automatically calls your API with:
   - Device information (model, OS version, location if available)
   - Timestamp and incident type
   - JSON payload for easy API integration

### **API Integration:**
```csharp
public async Task<bool> RaiseIncidentAsync(string incidentType = "Emergency")
{
    var incident = new
    {
        Type = incidentType,
        Timestamp = DateTime.UtcNow,
        DeviceInfo = new
        {
            Model = DeviceInfo.Model,
            Platform = DeviceInfo.Platform.ToString(),
            Version = DeviceInfo.VersionString
        }
    };

    // POST to your incident API
    var response = await _httpClient.PostAsJsonAsync("/api/incidents", incident);
    return response.IsSuccessStatusCode;
}
```

---

## 🔧 Background Service Details

### **Android Service Lifecycle:**
1. **MainActivity.OnCreate()** → Starts `FlicBackgroundService`
2. **Service creates notification** → Ensures foreground service compliance
3. **FlicManager.Init()** → Maintains button connections
4. **Device reboot** → `FlicBootReceiver` auto-restarts service

### **iOS Background Limitations:**
- **30 seconds to 3 minutes** of background execution time
- **System-managed** - iOS controls when background time is granted
- **Automatic reconnection** when app returns to foreground
- **Background App Refresh required** - User must enable in Settings

### **Battery Optimization:**
- **Android**: Users may need to disable battery optimization for the app
- **iOS**: System automatically manages background time to preserve battery

---

## 🧑‍💻 Developer Notes

### **Customizing the Background Service:**
The `FlicBackgroundService.cs` file can be customized to:
- Change notification text/icon
- Add custom logging
- Implement different lifecycle behaviors
- Handle service shutdown gracefully

### **Template Code Available:**
Use `AndroidBackgroundServiceHelper` to get template code for new projects:
```csharp
// Get complete template implementations
var serviceTemplate = AndroidBackgroundServiceHelper.GetBackgroundServiceTemplate();
var receiverTemplate = AndroidBackgroundServiceHelper.GetBootReceiverTemplate();
var manifestEntries = AndroidBackgroundServiceHelper.GetManifestEntries();
```

### **Why This Architecture Works:**
✅ **No build errors** - Android components in main app, not NuGet package  
✅ **Reliable connectivity** - Proper foreground service implementation  
✅ **Production ready** - Handles device reboot, app updates, battery optimization  
✅ **Cross-platform** - Same API works on both Android and iOS  
✅ **Emergency ready** - Double-click incident handling built-in  

---

## 🐛 Troubleshooting

### **Android Issues:**
- **Service not starting**: Check manifest permissions and service declaration
- **Buttons disconnecting**: Verify foreground service notification is visible
- **No auto-restart**: Ensure `RECEIVE_BOOT_COMPLETED` permission is granted

### **iOS Issues:**
- **Quick disconnection**: Enable Background App Refresh in iOS Settings
- **No background time**: Verify Info.plist background modes are correct
- **Reconnection problems**: FlicManager reinitializes on app activation

### **Emergency API Issues:**
- **API not called**: Check network connectivity and API endpoint configuration
- **Double-click not detected**: Ensure buttons are properly connected and listening

---

## 📚 Additional Resources

- [Android Background Service Documentation](../docs/android_background_solution.md)
- [Cross-Platform Background Guide](../docs/cross_platform_background_execution.md)
- [Platform Bindings Guide](../docs/platforms_readme.md)

---

This sample demonstrates a **production-ready** implementation that balances reliability, battery efficiency, and emergency response capabilities across both Android and iOS platforms.