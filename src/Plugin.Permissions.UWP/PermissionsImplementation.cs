using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Enumeration;


namespace Plugin.Permissions
{
    /// <summary>
    /// Implementation for Permissions
    /// </summary>
    public class PermissionsImplementation : IPermissions
    {
        Guid ActivitySensorClassId = new Guid("9D9E0118-1807-4F2E-96E4-2CE57142E196");
        /// <summary>
        /// Request to see if you should show a rationale for requesting permission
        /// Only on Android
        /// </summary>
        /// <returns>True or false to show rationale</returns>
        /// <param name="permission">Permission to check.</param>
        public Task<bool> ShouldShowRequestPermissionRationaleAsync(Permission permission) => Task.FromResult(false);

        /// <summary>
        /// Determines whether this instance has permission the specified permission.
        /// </summary>
        /// <returns><c>true</c> if this instance has permission the specified permission; otherwise, <c>false</c>.</returns>
        /// <param name="permission">Permission to check.</param>
        public Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            switch (permission)
            {
                case Permission.Calendar:
                    break;
                case Permission.Camera:
                    break;
                case Permission.Contacts:
                    return CheckContactsAsync();
                case Permission.Location:
				case Permission.LocationAlways:
				case Permission.LocationWhenInUse:
                    return CheckLocationAsync();
                case Permission.Microphone:
                    break;
                case Permission.Phone:
                    break;
                case Permission.Photos:
                    break;
                case Permission.Reminders:
                    break;
                case Permission.Sensors:
                    {
                        // Determine if the user has allowed access to activity sensors
                        var deviceAccessInfo = DeviceAccessInformation.CreateFromDeviceClassId(ActivitySensorClassId);
                        switch(deviceAccessInfo.CurrentStatus)
                        {
                            case DeviceAccessStatus.Allowed:
                                return Task.FromResult(PermissionStatus.Granted);
                            case DeviceAccessStatus.DeniedBySystem:
                            case DeviceAccessStatus.DeniedByUser:
                                return Task.FromResult(PermissionStatus.Denied);
                            default:
                                return Task.FromResult(PermissionStatus.Unknown);
                        }
                    }
                case Permission.Sms:
                    break;
                case Permission.Storage:
                    break;
                case Permission.Speech:
                    break;
            }
            return Task.FromResult(PermissionStatus.Granted);
        }

        private async Task<PermissionStatus> CheckContactsAsync()
        {

            var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

            if (accessStatus == null)
                return PermissionStatus.Denied;

            return PermissionStatus.Granted;
        }

        private async Task<PermissionStatus> CheckLocationAsync()
        {

            var accessStatus = await Geolocator.RequestAccessAsync();

            switch(accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    return PermissionStatus.Granted;
                case GeolocationAccessStatus.Unspecified:
                    return PermissionStatus.Unknown;

            }

            return PermissionStatus.Denied;
        }

        /// <summary>
        /// Requests the permissions from the users
        /// </summary>
        /// <returns>The permissions and their status.</returns>
        /// <param name="permissions">Permissions to request.</param>
        public Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions)
        {
            var results = permissions.ToDictionary(permission => permission, permission => PermissionStatus.Granted);
            return Task.FromResult(results);
        }

        public bool OpenAppSettings() => false;
    }
}