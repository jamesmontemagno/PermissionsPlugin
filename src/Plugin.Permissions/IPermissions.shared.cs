using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Plugin.Permissions.Abstractions
{
	/// <summary>
	/// Interface for Permissions
	/// </summary>
	public interface IPermissions
    {
		/// <summary>
		/// Determines whether this the permission has been granted
		/// </summary>
		/// <returns>The permission status for requested permission</returns>
		Task<PermissionStatus> CheckPermissionStatusAsync<T>() where T : BasePermission, new();

		/// <summary>
		/// Requests the permissions from the users
		/// </summary>
		/// <returns>The permissions and their status.</returns>
		Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new();

		/// <summary>
		/// Request to see if you should show a rationale for requesting permission
		/// Only on Android
		/// </summary>
		/// <returns>True or false to show rationale</returns>
		/// <param name="permission">Permission to check.</param>
		Task<bool> ShouldShowRequestPermissionRationaleAsync(Permission permission);

		/// <summary>
		/// Determines whether this instance has permission the specified permission.
		/// </summary>
		/// <returns><c>true</c> if this instance has permission the specified permission; otherwise, <c>false</c>.</returns>
		/// <param name="permission">Permission to check.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("CheckPermissionStatusAsync is deprecated, please use CheckPermissionStatusAsync<T> instead.")]
		Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission);

		/// <summary>
		/// Requests the permissions from the users
		/// </summary>
		/// <returns>The permissions and their status.</returns>
		/// <param name="permissions">Permissions to request.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RequestPermissionsAsync is deprecated, please use RequestPermissionAsync<T> instead.")]
		Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions);


		/// <summary>
		/// Attempts to open the app settings to adjust the permissions.
		/// </summary>
		/// <returns>true if success, else false and not supported</returns>
		bool OpenAppSettings();
    }
}
