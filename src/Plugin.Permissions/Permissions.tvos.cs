using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;
using Photos;

// r2_au cheered 1000 April 19 2019

[assembly: LinkerSafe]

namespace Plugin.Permissions
{
	/// <summary>
	/// Implementation for Permissions
	/// </summary>
	public class PermissionsImplementation : IPermissions
	{
		/// <summary>
		/// Gets the current permissions implementation
		/// </summary>
		/// <value>The current.</value>
		public static PermissionsImplementation Current => (PermissionsImplementation) CrossPermissions.Current;


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
				case Permission.Location:
				case Permission.LocationAlways:
				case Permission.LocationWhenInUse:
					return Task.FromResult(GetLocationPermissionStatus(permission));
				case Permission.Photos:
					return Task.FromResult(PhotosPermissionStatus);
			}

			return Task.FromResult(PermissionStatus.Granted);
		}

		/// <summary>
		/// Requests the permissions from the users
		/// </summary>
		/// <returns>The permissions and their status.</returns>
		/// <param name="permissions">Permissions to request.</param>
		public async Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(
			params Permission[] permissions)
		{
			var results = new Dictionary<Permission, PermissionStatus>();

			foreach (var permission in permissions)
			{
				if (results.ContainsKey(permission))
					continue;

				switch (permission)
				{
					case Permission.LocationWhenInUse:
					case Permission.LocationAlways:
					case Permission.Location:
						results.Add(permission, await RequestLocationPermission(permission));

						break;
					case Permission.Photos:
						results.Add(permission, await RequestPhotosPermission());

						break;
				}

				if (!results.ContainsKey(permission))
					results.Add(permission, PermissionStatus.Granted);
			}

			return results;
		}

		#region Location

		internal static CLLocationManager locationManager;
		public static TimeSpan LocationPermissionTimeout { get; set; } = new TimeSpan(0, 0, 8);

		internal static Task<PermissionStatus> RequestLocationPermission(Permission permission = Permission.Location)
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse &&
			    permission == Permission.LocationAlways)
			{
				//dont' do anything and request it
			}
			else if (GetLocationPermissionStatus(permission) != PermissionStatus.Unknown)
				return Task.FromResult(GetLocationPermissionStatus(permission));

			if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				return Task.FromResult(PermissionStatus.Unknown);
			}

			locationManager = new CLLocationManager();

			var tcs = new TaskCompletionSource<PermissionStatus>();

			var previousState = CLLocationManager.Status;

			locationManager.AuthorizationChanged += AuthorizationChanged;

			void AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
			{
				Console.WriteLine(e.Status);

				if (e.Status == CLAuthorizationStatus.NotDetermined)
					return;

				if (previousState == CLAuthorizationStatus.AuthorizedWhenInUse &&
				    permission == Permission.LocationAlways)
				{
					if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
					{
						WithTimeout(tcs.Task, LocationPermissionTimeout).ContinueWith((t) =>
						{
							//wait 10 seconds and check to see if it is completed or not.
							if (!tcs.Task.IsCompleted)
							{
								locationManager.AuthorizationChanged -= AuthorizationChanged;
								tcs.TrySetResult(GetLocationPermissionStatus(permission));
							}
						});

						return;
					}
				}

				locationManager.AuthorizationChanged -= AuthorizationChanged;

				tcs.TrySetResult(GetLocationPermissionStatus(permission));
			}

			var info = NSBundle.MainBundle.InfoDictionary;

			if (permission == Permission.Location)
			{
				if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
					locationManager.RequestWhenInUseAuthorization();
				else
					throw new UnauthorizedAccessException(
						"On tvOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
			}
			else if (permission == Permission.LocationAlways)
			{
				throw new UnauthorizedAccessException(
						"On tvOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
			}
			else
			{
				if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
					locationManager.RequestWhenInUseAuthorization();
				else
					throw new UnauthorizedAccessException(
						"On tvOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
			}

			return tcs.Task;
		}

		internal static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeSpan)
		{
			var retTask = await Task.WhenAny(task, Task.Delay(timeSpan))
				.ConfigureAwait(false);

			return retTask is Task<T> ? task.Result : default(T);
		}


		internal static PermissionStatus GetLocationPermissionStatus(Permission permission)
		{
			if (!CLLocationManager.LocationServicesEnabled)
				return PermissionStatus.Disabled;

			var status = CLLocationManager.Status;

			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				//if checking for always then check to see if we have it really, else denied
				if (permission == Permission.LocationAlways)
				{
					switch (status)
					{
						case CLAuthorizationStatus.AuthorizedAlways:
							return PermissionStatus.Granted;
						case CLAuthorizationStatus.AuthorizedWhenInUse:
						case CLAuthorizationStatus.Denied:
							return PermissionStatus.Denied;
						case CLAuthorizationStatus.Restricted:
							return PermissionStatus.Restricted;
						default:
							return PermissionStatus.Unknown;
					}
				}

				switch (status)
				{
					case CLAuthorizationStatus.AuthorizedAlways:
					case CLAuthorizationStatus.AuthorizedWhenInUse:
						return PermissionStatus.Granted;
					case CLAuthorizationStatus.Denied:
						return PermissionStatus.Denied;
					case CLAuthorizationStatus.Restricted:
						return PermissionStatus.Restricted;
					default:
						return PermissionStatus.Unknown;
				}
			}

			switch (status)
			{
				case CLAuthorizationStatus.Authorized:
					return PermissionStatus.Granted;
				case CLAuthorizationStatus.Denied:
					return PermissionStatus.Denied;
				case CLAuthorizationStatus.Restricted:
					return PermissionStatus.Restricted;
				default:
					return PermissionStatus.Unknown;
			}
		}

		#endregion

		#region Notifications

		/*PermissionStatus NotificationLocalPermissionState
        {
            get
            {
                var currentSettings = UIApplication.SharedApplication.CurrentUserNotificationSettings;

                if (currentSettings == null || notificationLocalSettings.Types == UIUserNotificationType.None)
                {
                    return PermissionStatus.Denied;
                }

                return PermissionStatus.Granted;
            }
        }

        Task<PermissionStatus> RequestNotificationLocalPermission()
        {
            if (NotificationLocalPermissionState == PermissionStatus.Granted)
                return Task.FromResult(PermissionStatus.Granted);

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("DidRegisterUserNotificationSettings")
        }*/

		#endregion

		#region Photos

		internal static PermissionStatus PhotosPermissionStatus
		{
			get
			{
				var status = PHPhotoLibrary.AuthorizationStatus;

				switch (status)
				{
					case PHAuthorizationStatus.Authorized:
						return PermissionStatus.Granted;
					case PHAuthorizationStatus.Denied:
						return PermissionStatus.Denied;
					case PHAuthorizationStatus.Restricted:
						return PermissionStatus.Restricted;
					default:
						return PermissionStatus.Unknown;
				}
			}
		}

		internal static Task<PermissionStatus> RequestPhotosPermission()
		{
			if (PhotosPermissionStatus != PermissionStatus.Unknown)
				return Task.FromResult(PhotosPermissionStatus);

			var tcs = new TaskCompletionSource<PermissionStatus>();

			PHPhotoLibrary.RequestAuthorization(status =>
			{
				switch (status)
				{
					case PHAuthorizationStatus.Authorized:
						tcs.TrySetResult(PermissionStatus.Granted);

						break;
					case PHAuthorizationStatus.Denied:
						tcs.TrySetResult(PermissionStatus.Denied);

						break;
					case PHAuthorizationStatus.Restricted:
						tcs.TrySetResult(PermissionStatus.Restricted);

						break;
					default:
						tcs.TrySetResult(PermissionStatus.Unknown);

						break;
				}
			});

			return tcs.Task;
		}

		#endregion

		public bool OpenAppSettings()
		{
			//Opening settings only open in tvOS 8+
			if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				return false;

			try
			{
				UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));

				return true;
			}
			catch
			{
				return false;
			}
		}

		public Task<PermissionStatus> CheckPermissionStatusAsync<T>() where T : BasePermission, new() =>
			new T().CheckPermissionStatusAsync();

		public Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new() =>
			new T().RequestPermissionAsync();
	}
}