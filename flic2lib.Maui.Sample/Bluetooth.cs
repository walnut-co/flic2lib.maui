#if ANDROID
using Android;
#elif IOS
using CoreBluetooth;
using UIKit;
#endif

namespace flic2lib.Maui.Sample;
public partial class Bluetooth
{
    public async Task CheckOrRequestAccessAsync()
    {
#if ANDROID
        var perms = await Permissions.CheckStatusAsync<BluetoothPermissions>();
        if (perms != PermissionStatus.Granted)
        {
            var retry = await Permissions.RequestAsync<BluetoothPermissions>();
            if (retry != PermissionStatus.Granted)
            {
                throw new Exception("No bluetooth perm...");
            }
        }
#elif IOS
        if (!HasBluetoothPerm())
        {
            var perm = await RequestBluetoothPermissionAsync();
            if (perm != PermissionStatus.Granted)
            {
                throw new Exception("No bluetooth perm...");
            }
        }
#endif
    }

#if IOS
    private async Task<PermissionStatus> RequestBluetoothPermissionAsync()
    {
        // Initializing CBCentralManager will present the Bluetooth permission dialog.
        new CBCentralManager();
        PermissionStatus status;
        do
        {
            status = CheckPermissionAsync();
            await Task.Delay(200);
        } while (status == PermissionStatus.Unknown);
        return status;
    }
    private PermissionStatus CheckPermissionAsync()
    {
        return CBCentralManager.Authorization switch
        {
            CBManagerAuthorization.AllowedAlways => PermissionStatus.Granted,
            CBManagerAuthorization.Restricted => PermissionStatus.Restricted,
            CBManagerAuthorization.NotDetermined => PermissionStatus.Unknown,
            _ => PermissionStatus.Denied
        };
    }

    private bool HasBluetoothPerm()
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        {
            return CBManager.Authorization == CBManagerAuthorization.AllowedAlways;
        }
        return true;
    }
#endif
}

#if ANDROID
public class BluetoothPermissions : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
    {
        get
        {
            var perms = new List<(string androidPermission, bool isRuntime)>();

            var sdk = (int)Android.OS.Build.VERSION.SdkInt;
            if (sdk >= 31)
            {
                perms.Add((Manifest.Permission.BluetoothScan, true));
                perms.Add((Manifest.Permission.BluetoothConnect, true));
                perms.Add((Manifest.Permission.AccessFineLocation, true));
            }
            else
            {
                perms.Add((Manifest.Permission.Bluetooth, true));
                perms.Add((Manifest.Permission.BluetoothAdmin, true));

                perms.Add(sdk >= 29
                    ? (Manifest.Permission.AccessFineLocation, true)
                    : (Manifest.Permission.AccessCoarseLocation, true));
            }

            return perms.ToArray();
        }
    }
}
#endif