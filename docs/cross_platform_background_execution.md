# Flic2 Background Execution - Cross-Platform Guide

This guide covers background execution implementation for Flic2 buttons across both Android and iOS platforms, providing developers with a unified approach to maintaining button connectivity when apps are backgrounded.

## Overview

The Flic2 MAUI library provides **built-in background execution support** for both platforms:
- **Android**: Robust foreground service with persistent notification
- **iOS**: Best-effort background execution within Apple's limitations

## Quick Start - Universal Implementation

### 1. Android Setup
Add to `Platforms/Android/AndroidManifest.xml`:

```xml
<application android:allowBackup="true" android:icon="@mipmap/appicon"
    android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true">
    
    <!-- Flic2 Background Service (from library) -->
    <service android:name="flic2lib.Maui.Platforms.Android.Flic2BackgroundService"
        android:enabled="true"
        android:exported="false"
        android:foregroundServiceType="connectedDevice" />

    <!-- Flic2 Boot Receiver (from library) -->
    <receiver android:name="flic2lib.Maui.Platforms.Android.Flic2BootReceiver"
        android:enabled="true"
        android:exported="true">
        <intent-filter android:priority="1000">
            <action android:name="android.intent.action.BOOT_COMPLETED" />
            <action android:name="android.intent.action.MY_PACKAGE_REPLACED" />
            <category android:name="android.intent.category.DEFAULT" />
        </intent-filter>
    </receiver>
</application>

<!-- Required permissions for background execution -->
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE" />
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
<uses-permission android:name="android.permission.WAKE_LOCK" />
```

### 2. iOS Setup
Add to `Platforms/iOS/Info.plist`:

```xml
<key>UIBackgroundModes</key>
<array>
    <string>bluetooth-central</string>
    <string>background-processing</string>
    <string>background-fetch</string>
</array>
```

### 3. Platform-Specific Initialization

**Android - MainActivity.cs:**
```csharp
using flic2lib.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Keep Flic2 buttons connected when app is in background
        Configure.KeepFlicButtonsConnected(this);
    }
}
```

**iOS - AppDelegate.cs:**
```csharp
using flic2lib.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);
        
        // Keep Flic2 buttons connected when app is in background
        Configure.KeepFlicButtonsConnected();
        
        return result;
    }
}
```

## Platform Differences

| Feature | Android | iOS |
|---------|---------|-----|
| **Background Duration** | Unlimited (foreground service) | Limited (30s-3min) |
| **User Configuration** | Optional (notification settings) | Required (Background App Refresh) |
| **Setup Complexity** | Low | Medium |
| **Reliability** | High | Limited by iOS |
| **Battery Impact** | Visible notification | System-managed |
| **Auto-restart** | Yes (after boot/updates) | No (user must reopen app) |

## Development Strategies Without Mac

### 1. **Unified API Design** ⭐ (Implemented)
Both platforms now use the same method calls:
```csharp
// Works on both platforms with appropriate parameters
Configure.KeepFlicButtonsConnected(context); // Android
Configure.KeepFlicButtonsConnected();        // iOS
```

### 2. **Cloud Build Solutions**
For iOS testing and building without a Mac:

#### **Visual Studio App Center**
```yaml
# appcenter-post-build.sh
#!/usr/bin/env bash
echo "Building iOS app with Flic2 background execution"
# Build process handles iOS-specific compilation
```

#### **GitHub Actions**
```yaml
name: iOS Build
on: [push]
jobs:
  ios:
    runs-on: macos-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
    - name: Build iOS
      run: dotnet build -f net9.0-ios
```

#### **Azure DevOps**
```yaml
trigger:
- main

pool:
  vmImage: 'macOS-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '9.0.x'
- script: dotnet build -f net9.0-ios
  displayName: 'Build iOS Project'
```

### 3. **Testing Strategies**

#### **Unit Testing (Cross-Platform)**
```csharp
[Test]
public void Configure_KeepFlicButtonsConnected_Android_DoesNotThrow()
{
    // Mock Android context
    var mockContext = new Mock<Android.Content.Context>();
    
    // Should not throw exception
    Assert.DoesNotThrow(() => Configure.KeepFlicButtonsConnected(mockContext.Object));
}

[Test]
public void Configure_KeepFlicButtonsConnected_iOS_DoesNotThrow()
{
    // Should not throw exception on iOS
    Assert.DoesNotThrow(() => Configure.KeepFlicButtonsConnected());
}
```

#### **Static Analysis**
Use cross-platform tools to validate iOS code:
```bash
# Analyze iOS-specific code without Mac
dotnet build -f net9.0-ios --verbosity normal
```

### 4. **Partnership/Community Testing**

#### **TestFlight Distribution**
1. Use cloud build to create iOS IPA
2. Distribute via TestFlight to iOS users
3. Collect feedback on background execution behavior
4. Iterate based on real-world testing

#### **Community Collaboration**
```markdown
# iOS Testing Request
We've implemented iOS background execution for Flic2 buttons but need Mac testing.

**What to test:**
1. Enable Background App Refresh for the app
2. Connect Flic2 buttons in foreground
3. Background the app
4. Test button connectivity for 1-3 minutes
5. Return to foreground and verify reconnection

**Expected behavior:**
- Buttons work for limited time in background
- Automatic reconnection when returning to foreground
- No crashes or memory issues
```

## Implementation Status

### ✅ Completed
- **Android**: Full foreground service implementation with auto-restart
- **iOS**: Background execution within Apple's limitations
- **Cross-platform API**: Unified method names and patterns
- **Documentation**: Comprehensive guides for both platforms
- **Sample Integration**: Working examples for both platforms

### 🔄 Ready for Testing
- **Android**: Fully tested and working
- **iOS**: Code complete, needs Mac-based testing

### 📋 Next Steps for iOS
1. **Cloud Build Setup**: Configure automated iOS builds
2. **TestFlight Testing**: Distribute to iOS users for testing
3. **Feedback Collection**: Gather real-world usage data
4. **Optimization**: Refine based on testing results

## Troubleshooting Guide

### Android Issues
- **Service not starting**: Check manifest permissions and service declaration
- **Buttons disconnecting**: Verify foreground service notification is visible
- **Boot receiver not working**: Ensure RECEIVE_BOOT_COMPLETED permission

### iOS Issues
- **Quick disconnection**: Check Background App Refresh settings
- **No background time**: Verify Info.plist background modes
- **Reconnection problems**: FlicManager reinitializes on app activation

### Cross-Platform
- **Build errors**: Ensure .NET 9.0 is properly configured for all targets
- **Missing references**: Verify platform-specific code is properly conditionally compiled

## Code Examples

### Check Background Capabilities
```csharp
public async Task<string> GetBackgroundStatus()
{
#if ANDROID
    var isRecommended = Platforms.Android.Flic2BackgroundManager.IsBackgroundServiceRecommended();
    return isRecommended ? "Android background service available" : "Android background service not needed";
#elif IOS
    return Platforms.iOS.Flic2BackgroundManager.GetBackgroundStatus();
#else
    return "Background execution not implemented for this platform";
#endif
}
```

### User Education
```csharp
public async Task ShowBackgroundSetupGuide()
{
#if ANDROID
    await DisplayAlert("Background Execution", 
        "Flic buttons will stay connected in the background automatically. " +
        "You'll see a notification indicating the background service is running.", 
        "OK");
#elif IOS
    var isEnabled = Platforms.iOS.Flic2BackgroundManager.IsBackgroundAppRefreshEnabled();
    if (!isEnabled)
    {
        await DisplayAlert("Background App Refresh Required", 
            "To keep Flic buttons connected in background:\n" +
            "1. Open iOS Settings\n" +
            "2. Go to General → Background App Refresh\n" +
            "3. Enable for this app\n\n" +
            "Note: iOS limits background time to preserve battery.", 
            "OK");
    }
#endif
}
```

This cross-platform implementation provides the best possible background execution experience on both platforms while working within each platform's constraints and capabilities.