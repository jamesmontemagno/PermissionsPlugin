using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using Tizen.Security;
using System.Diagnostics;
using System.Linq;

namespace Plugin.Permissions
{
	/// <summary>
	/// Implementation for Permissions
	/// </summary>
	public class PermissionsImplementation : IPermissions
	{
		static private string privilegeCalendarRead = "http://tizen.org/privilege/calendar.read";
		static private string privilegeCalendarWrite = "http://tizen.org/privilege/calendar.write";
		static private string privilegeCamera = "http://tizen.org/privilege/camera";
		static private string privilegeContactRead = "http://tizen.org/privilege/contact.read";
		static private string privilegeContactWrite = "http://tizen.org/privilege/contact.write";
		static private string privilegeLocation = "http://tizen.org/privilege/location";
		static private string privilegeRecorder = "http://tizen.org/privilege/recorder";
		static private string privilegeCall = "http://tizen.org/privilege/call";
		static private string privilegeHealthInfo = "http://tizen.org/privilege/healthinfo";
		static private string privilegeMessageRead = "http://tizen.org/privilege/message.read";
		static private string privilegeMessageWrite = "http://tizen.org/privilege/message.write";
		static private string privilegeExternalStorage = "http://tizen.org/privilege/externalstorage";

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
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeCalendarRead) &&
							Privilege.GetPrivacyPrivilegeStatus(privilegeCalendarWrite))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Camera:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeCamera))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Contacts:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeContactRead) &&
							Privilege.GetPrivacyPrivilegeStatus(privilegeContactWrite))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Location:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeLocation))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Microphone:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeRecorder))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Phone:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeCall))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Photos:
					return Task.FromResult(PermissionStatus.Unknown);
				case Permission.Reminders:
					return Task.FromResult(PermissionStatus.Unknown);
				case Permission.Sensors:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeHealthInfo))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Sms:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeMessageRead) &&
							Privilege.GetPrivacyPrivilegeStatus(privilegeMessageWrite))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Storage:
					try
					{
						if (Privilege.GetPrivacyPrivilegeStatus(privilegeExternalStorage))
							return Task.FromResult(PermissionStatus.Granted);
						else
							return Task.FromResult(PermissionStatus.Denied);
					}
					catch
					{
						return Task.FromResult(PermissionStatus.Unknown);
					}
				case Permission.Speech:
					return Task.FromResult(PermissionStatus.Unknown);
				default:
					return Task.FromResult(PermissionStatus.Unknown);
			}
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

				var result = await CheckPermissionStatusAsync(permission).ConfigureAwait(false);
				results.Add(permission, result);
			}

			return results;
		}

		public bool OpenAppSettings() => false;

		public Task<bool> ShouldShowRequestPermissionRationaleAsync(Permission permission) => Task.FromResult(false);
	}
}
