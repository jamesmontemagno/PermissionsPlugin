using System;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace Plugin.Permissions
{
	public class BasePermission
	{
		protected Permission permission;
		public BasePermission(Permission permission)
		{
			this.permission = permission;
		}



#pragma warning disable CS0618 // Type or member is obsolete
		public virtual Task<PermissionStatus> CheckPermissionStatusAsync() =>
#if __IOS__ || __TVOS__
			throw new NotImplementedException();
#else
			CrossPermissions.Current.CheckPermissionStatusAsync(permission);
#endif


		public virtual async Task<PermissionStatus> RequestPermissionAsync()
		{
#if __IOS__ || __TVOS__
			throw new NotImplementedException();
#else
			var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
			if (results.ContainsKey(permission))
				return results[permission];

			return PermissionStatus.Unknown;
#endif
		}
#pragma warning restore CS0618 // Type or member is obsolete
	}

	public class CalendarPermission : BasePermission
	{
		public CalendarPermission() : base(Permission.Calendar)
		{

		}

#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetEventPermissionStatus(EventKit.EKEntityType.Event));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestEventPermission(EventKit.EKEntityType.Event);
#endif
	}

	public class CameraPermission : BasePermission
	{
		public CameraPermission() : base(Permission.Camera)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetAVPermissionStatus(AVFoundation.AVMediaType.Video));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestAVPermissionStatusAsync(AVFoundation.AVMediaType.Video);
#endif
	}

	public class ContactsPermission : BasePermission
	{
		public ContactsPermission() : base(Permission.Contacts)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.ContactsPermissionStatus);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestContactsPermission();
#endif
	}

	public class LocationPermission : BasePermission
	{
		public LocationPermission() : base(Permission.Location)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetLocationPermissionStatus(permission));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestLocationPermission(permission);
#endif
	}

	public class LocationAlwaysPermission : BasePermission
	{
		public LocationAlwaysPermission() : base(Permission.LocationAlways)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetLocationPermissionStatus(permission));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestLocationPermission(permission);
#endif
	}

	public class LocationWhenInUsePermission : BasePermission
	{
		public LocationWhenInUsePermission() : base(Permission.LocationWhenInUse)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetLocationPermissionStatus(permission));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestLocationPermission(permission);
#endif
	}

	public class MediaLibraryPermission : BasePermission
	{
		public MediaLibraryPermission() : base(Permission.MediaLibrary)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.MediaLibraryPermissionStatus);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestMediaLibraryPermission();
#endif
	}

	public class MicrophonePermission : BasePermission
	{
		public MicrophonePermission() : base(Permission.Microphone)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetAVPermissionStatus(AVFoundation.AVMediaType.Audio));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestAVPermissionStatusAsync(AVFoundation.AVMediaType.Audio);
#endif
	}

	public class PhonePermission : BasePermission
	{
		public PhonePermission() : base(Permission.Phone)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionStatus.Granted);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			Task.FromResult(PermissionStatus.Granted);
#endif
	}

	public class PhotosPermission : BasePermission
	{
		public PhotosPermission() : base(Permission.Photos)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.PhotosPermissionStatus);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestPhotosPermission();
#endif
	}

	public class RemindersPermission : BasePermission
	{
		public RemindersPermission() : base(Permission.Reminders)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.GetEventPermissionStatus(EventKit.EKEntityType.Reminder));


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestEventPermission(EventKit.EKEntityType.Reminder);
#endif
	}

	public class SensorsPermission : BasePermission
	{
		public SensorsPermission() : base(Permission.Sensors)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.SensorsPermissionStatus);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestSensorsPermission();
#endif
	}

	public class SmsPermission : BasePermission
	{
		public SmsPermission() : base(Permission.Sms)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionStatus.Granted);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			Task.FromResult(PermissionStatus.Granted);
#endif
	}

	public class StoragePermission : BasePermission
	{
		public StoragePermission() : base(Permission.Storage)
		{

		}
#if __IOS__ || __TVOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionStatus.Granted);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			Task.FromResult(PermissionStatus.Granted);
#endif
	}

	public class SpeechPermission : BasePermission
	{
		public SpeechPermission() : base(Permission.Speech)
		{

		}
#if __IOS__
		public override Task<PermissionStatus> CheckPermissionStatusAsync() =>
			Task.FromResult(PermissionsImplementation.SpeechPermissionStatus);


		public override Task<PermissionStatus> RequestPermissionAsync() =>
			PermissionsImplementation.RequestSpeechPermission();
#endif
	}
}
