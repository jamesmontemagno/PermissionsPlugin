using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
#if WINDOWS_UWP
using Windows.Devices.Geolocation;
#endif

namespace Plugin.Permissions
{
    /// <summary>
    /// Implementation for Permissions
    /// </summary>
    public class PermissionsImplementation : IPermissions
    {
        /// <summary>
        /// Request to see if you should show a rationale for requesting permission
        /// Only on Android
        /// </summary>
        /// <returns>True or false to show rationale</returns>
        /// <param name="permission">Permission to check.</param>
        public Task<bool> ShouldShowRequestPermissionRationaleAsync(Permission permission)
        {
            return Task.FromResult(false);
        }

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
                    break;
                case Permission.Location:
                    return CheckLocationAsync();
                case Permission.Microphone:
                    break;
                //case Permission.NotificationsLocal:
                //    break;
                //case Permission.NotificationsRemote:
                //    break;
                case Permission.Photos:
                    break;
                case Permission.Reminders:
                    break;
                case Permission.Sensors:
                    break;
            }
            return Task.FromResult(PermissionStatus.Granted);
        }

        private async Task<PermissionStatus> CheckContactsAsync()
        {
#if WINDOWS_UWP || WINDOWS_PHONE_APP
            var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

            if (accessStatus == null)
                return PermissionStatus.Denied;

            return PermissionStatus.Granted;
#endif

            return PermissionStatus.Unknown;
        }

        private async Task<PermissionStatus> CheckLocationAsync()
        {
#if WINDOWS_UWP
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch(accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    return PermissionStatus.Granted;
                case GeolocationAccessStatus.Unspecified:
                    return PermissionStatus.Unknown;
                    
            }

            return PermissionStatus.Denied;
#endif

            return PermissionStatus.Unknown;
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
    }
}