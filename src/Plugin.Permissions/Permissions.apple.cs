using AVFoundation;
using CoreLocation;
using Foundation;
using Photos;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

#if __IOS__
using AddressBook;
using CoreMotion;
using Speech;
using EventKit;
using MediaPlayer;
#endif

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
		public static PermissionsImplementation Current => (PermissionsImplementation)CrossPermissions.Current;


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
#if __IOS__
					return Task.FromResult(GetEventPermissionStatus(EKEntityType.Event));
#elif __TVOS__
					return Task.FromResult(PermissionStatus.Granted);
#endif
				case Permission.Camera:
					return Task.FromResult(GetAVPermissionStatus(AVMediaType.Video));
				case Permission.Contacts:
					return Task.FromResult(ContactsPermissionStatus);
				case Permission.Location:
				case Permission.LocationAlways:
				case Permission.LocationWhenInUse:
					return Task.FromResult(GetLocationPermissionStatus(permission));
				case Permission.MediaLibrary:
					return Task.FromResult(MediaLibraryPermissionStatus);
				case Permission.Microphone:
					return Task.FromResult(GetAVPermissionStatus(AVMediaType.Audio));
				case Permission.Photos:
					return Task.FromResult(PhotosPermissionStatus);
				case Permission.Reminders:
#if __IOS__
					return Task.FromResult(GetEventPermissionStatus(EKEntityType.Reminder));
#elif __TVOS__
					return Task.FromResult(PermissionStatus.Granted);
#endif
				case Permission.Sensors:
					return Task.FromResult(SensorsPermissionStatus);
				case Permission.Speech:
					return Task.FromResult(SpeechPermissionStatus);
			}
			return Task.FromResult(PermissionStatus.Granted);
		}

		/// <summary>
		/// Requests the permissions from the users
		/// </summary>
		/// <returns>The permissions and their status.</returns>
		/// <param name="permissions">Permissions to request.</param>
		public async Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions)
		{
			var results = new Dictionary<Permission, PermissionStatus>();
			foreach (var permission in permissions)
			{
				if (results.ContainsKey(permission))
					continue;

				switch (permission)
				{
					case Permission.Calendar:
#if __IOS__
						results.Add(permission, await RequestEventPermission(EKEntityType.Event));
#endif
						break;
					case Permission.Camera:
						results.Add(permission, await RequestAVPermissionStatusAsync(AVMediaType.Video));
						break;
					case Permission.Contacts:
						results.Add(permission, await RequestContactsPermission());
						break;
					case Permission.LocationWhenInUse:
					case Permission.LocationAlways:
					case Permission.Location:
						results.Add(permission, await RequestLocationPermission(permission));
						break;
					case Permission.MediaLibrary:
						results.Add(permission, await RequestMediaLibraryPermission());
						break;
					case Permission.Microphone:
						results.Add(permission, await RequestAVPermissionStatusAsync(AVMediaType.Audio));
						break;
					case Permission.Photos:
						results.Add(permission, await RequestPhotosPermission());
						break;
					case Permission.Reminders:
#if __IOS__
						results.Add(permission, await RequestEventPermission(EKEntityType.Reminder));
#endif
						break;
					case Permission.Sensors:
						results.Add(permission, await RequestSensorsPermission());
						break;
					case Permission.Speech:
						results.Add(permission, await RequestSpeechPermission());
						break;
				}

				if (!results.ContainsKey(permission))
					results.Add(permission, PermissionStatus.Granted);
			}

			return results;
		}

		internal static async Task<PermissionStatus> RequestAVPermissionStatusAsync(NSString mediaType)
		{
#if __IOS__
			try
			{
				var auth = await AVCaptureDevice.RequestAccessForMediaTypeAsync(mediaType);
				return auth ? PermissionStatus.Granted : PermissionStatus.Denied;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to get {mediaType} permission: " + ex);
				return PermissionStatus.Unknown;
			}
#elif __TVOS__
			return PermissionStatus.Granted;
#endif
		}

#region AV: Camera and Microphone

		internal static PermissionStatus GetAVPermissionStatus(NSString mediaType)
		{
#if __IOS__
			var status = AVCaptureDevice.GetAuthorizationStatus(mediaType);
			switch (status)
			{
				case AVAuthorizationStatus.Authorized:
					return PermissionStatus.Granted;
				case AVAuthorizationStatus.Denied:
					return PermissionStatus.Denied;
				case AVAuthorizationStatus.Restricted:
					return PermissionStatus.Restricted;
				default:
					return PermissionStatus.Unknown;
			}
#elif __TVOS__
			return PermissionStatus.Granted;
#endif
		}
#endregion

#region Contacts
		internal static PermissionStatus ContactsPermissionStatus
		{
			get
			{
#if __IOS__
				var status = ABAddressBook.GetAuthorizationStatus();
				switch (status)
				{
					case ABAuthorizationStatus.Authorized:
						return PermissionStatus.Granted;
					case ABAuthorizationStatus.Denied:
						return PermissionStatus.Denied;
					case ABAuthorizationStatus.Restricted:
						return PermissionStatus.Restricted;
					default:
						return PermissionStatus.Unknown;
				}
#elif __TVOS__
				return PermissionStatus.Granted;
#endif
			}
		}

		internal static Task<PermissionStatus> RequestContactsPermission()
		{
#if __IOS__
			if (ContactsPermissionStatus != PermissionStatus.Unknown)
				return Task.FromResult(ContactsPermissionStatus);

			var addressBook = new ABAddressBook();

			var tcs = new TaskCompletionSource<PermissionStatus>();

			addressBook.RequestAccess((success, error) =>
			{
				tcs.TrySetResult((success ? PermissionStatus.Granted : PermissionStatus.Denied));
			});

			return tcs.Task;
#elif __TVOS__
			return Task.FromResult(PermissionStatus.Granted);
#endif
		}
#endregion

#region Location

		internal static CLLocationManager locationManager;
		public static TimeSpan LocationPermissionTimeout { get; set; } = new TimeSpan(0, 0, 8);
		internal static Task<PermissionStatus> RequestLocationPermission(Permission permission = Permission.Location)
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse && permission == Permission.LocationAlways)
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

				if (previousState == CLAuthorizationStatus.AuthorizedWhenInUse && permission == Permission.LocationAlways)
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
				if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
#if __IOS__
					locationManager.RequestAlwaysAuthorization();
				else if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
#elif __TVOS__
				if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
#endif
					locationManager.RequestWhenInUseAuthorization();
				else
					throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
			}
			else if (permission == Permission.LocationAlways)
			{
#if __IOS__
				if (info.ContainsKey(new NSString("NSLocationAlwaysUsageDescription")))
					locationManager.RequestAlwaysAuthorization();
				else
					throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");
#endif
			}
			else
			{
				if (info.ContainsKey(new NSString("NSLocationWhenInUseUsageDescription")))
					locationManager.RequestWhenInUseAuthorization();
				else
					throw new UnauthorizedAccessException("On iOS 8.0 and higher you must set either NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription in your Info.plist file to enable Authorization Requests for Location updates!");

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

#region Sensors

		internal static PermissionStatus SensorsPermissionStatus
		{
			get
			{
#if __IOS__
				var sensorStatus = PermissionStatus.Unknown;

				//return disabled if not avaialble.
				if (!CMMotionActivityManager.IsActivityAvailable)
					sensorStatus = PermissionStatus.Disabled;
				else if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
				{
					switch (CMMotionActivityManager.AuthorizationStatus)
					{
						case CMAuthorizationStatus.Authorized:
							sensorStatus = PermissionStatus.Granted;
							break;
						case CMAuthorizationStatus.Denied:
							sensorStatus = PermissionStatus.Denied;
							break;
						case CMAuthorizationStatus.NotDetermined:
							sensorStatus = PermissionStatus.Unknown;
							break;
						case CMAuthorizationStatus.Restricted:
							sensorStatus = PermissionStatus.Restricted;
							break;
					}
				}

				return sensorStatus;
#elif __TVOS__
				return PermissionStatus.Granted;
#endif
			}
		}
		internal static async Task<PermissionStatus> RequestSensorsPermission()
		{
			if (SensorsPermissionStatus != PermissionStatus.Unknown)
				return SensorsPermissionStatus;

#if __IOS__
			var activityManager = new CMMotionActivityManager();

			try
			{
				var results = await activityManager.QueryActivityAsync(NSDate.DistantPast, NSDate.DistantFuture, NSOperationQueue.MainQueue);
				if (results != null)
					return PermissionStatus.Granted;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to query activity manager: " + ex.Message);
				return PermissionStatus.Denied;
			}
#endif

			return PermissionStatus.Unknown;
		}
#endregion

#region Speech
		internal static Task<PermissionStatus> RequestSpeechPermission()
		{
			if (SpeechPermissionStatus != PermissionStatus.Unknown)
				return Task.FromResult(SpeechPermissionStatus);


			if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				return Task.FromResult(PermissionStatus.Unknown);
			}

#if __IOS__
			var tcs = new TaskCompletionSource<PermissionStatus>();

			SFSpeechRecognizer.RequestAuthorization(status =>
			{
				switch (status)
				{
					case SFSpeechRecognizerAuthorizationStatus.Authorized:
						tcs.TrySetResult(PermissionStatus.Granted);
						break;
					case SFSpeechRecognizerAuthorizationStatus.Denied:
						tcs.TrySetResult(PermissionStatus.Denied);
						break;
					case SFSpeechRecognizerAuthorizationStatus.Restricted:
						tcs.TrySetResult(PermissionStatus.Restricted);
						break;
					default:
						tcs.TrySetResult(PermissionStatus.Unknown);
						break;
				}
			});
			return tcs.Task;
#elif __TVOS__
			return Task.FromResult(PermissionStatus.Granted);
#endif
		}

		internal static PermissionStatus SpeechPermissionStatus
		{
			get
			{
#if __IOS__
				var status = SFSpeechRecognizer.AuthorizationStatus;
				switch (status)
				{
					case SFSpeechRecognizerAuthorizationStatus.Authorized:
						return PermissionStatus.Granted;
					case SFSpeechRecognizerAuthorizationStatus.Denied:
						return PermissionStatus.Denied;
					case SFSpeechRecognizerAuthorizationStatus.Restricted:
						return PermissionStatus.Restricted;
					default:
						return PermissionStatus.Unknown;
				}
#elif __TVOS__
				return PermissionStatus.Granted;
#endif
			}
		}
#endregion

#region MediaLib	
		internal static PermissionStatus MediaLibraryPermissionStatus
		{
			get
			{
#if __IOS__
				//Opening settings only open in iOS 9.3+	
				if (!UIDevice.CurrentDevice.CheckSystemVersion(9, 3))
					return PermissionStatus.Unknown;

				var status = MPMediaLibrary.AuthorizationStatus;
				switch (status)
				{
					case MPMediaLibraryAuthorizationStatus.Authorized:
						return PermissionStatus.Granted;
					case MPMediaLibraryAuthorizationStatus.Denied:
						return PermissionStatus.Denied;
					case MPMediaLibraryAuthorizationStatus.Restricted:
						return PermissionStatus.Restricted;
					default:
						return PermissionStatus.Unknown;
				}
#elif __TVOS__
				return PermissionStatus.Granted;
#endif
			}
		}

		internal static Task<PermissionStatus> RequestMediaLibraryPermission()
		{

			//Opening settings only open in iOS 9.3+	
			if (!UIDevice.CurrentDevice.CheckSystemVersion(9, 3))
				return Task.FromResult(PermissionStatus.Unknown);

			if (MediaLibraryPermissionStatus != PermissionStatus.Unknown)
				return Task.FromResult(MediaLibraryPermissionStatus);

#if __IOS__
			var tcs = new TaskCompletionSource<PermissionStatus>();

			MPMediaLibrary.RequestAuthorization(status =>
			{
				switch (status)
				{
					case MPMediaLibraryAuthorizationStatus.Authorized:
						tcs.TrySetResult(PermissionStatus.Granted);
						break;
					case MPMediaLibraryAuthorizationStatus.Denied:
						tcs.TrySetResult(PermissionStatus.Denied);
						break;
					case MPMediaLibraryAuthorizationStatus.Restricted:
						tcs.TrySetResult(PermissionStatus.Restricted);
						break;
					default:
						tcs.TrySetResult(PermissionStatus.Unknown);
						break;
				}
			});

			return tcs.Task;
#elif __TVOS__
			return Task.FromResult(PermissionStatus.Granted);
#endif
		}

#endregion

#region Events
#if __IOS__
		internal static PermissionStatus GetEventPermissionStatus(EKEntityType eventType)
		{
			var status = EKEventStore.GetAuthorizationStatus(eventType);
			switch (status)
			{
				case EKAuthorizationStatus.Authorized:
					return PermissionStatus.Granted;
				case EKAuthorizationStatus.Denied:
					return PermissionStatus.Denied;
				case EKAuthorizationStatus.Restricted:
					return PermissionStatus.Restricted;
				default:
					return PermissionStatus.Unknown;
			}

		}

		internal static async Task<PermissionStatus> RequestEventPermission(EKEntityType eventType)
		{

			if (GetEventPermissionStatus(eventType) == PermissionStatus.Granted)
				return PermissionStatus.Granted;

			var eventStore = new EKEventStore();

			var results = await eventStore.RequestAccessAsync(eventType);

			return results.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
		}
#endif
#endregion

		public bool OpenAppSettings()
		{
			//Opening settings only open in iOS 8+
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
