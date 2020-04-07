using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PermissionsSample
{
	public static class Utils
	{
		public static async Task<PermissionStatus> CheckPermissions(BasePermission permission)
		{
			var permissionStatus = await permission.CheckPermissionStatusAsync();
			bool request = false;
			if (permissionStatus == PermissionStatus.Denied)
			{
				if (Device.RuntimePlatform == Device.iOS)
				{

					var title = $"{permission} Permission";
					var question = $"To use this plugin the {permission} permission is required. Please go into Settings and turn on {permission} for the app.";
					var positive = "Settings";
					var negative = "Maybe Later";
					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return permissionStatus;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}

					return permissionStatus;
				}

				request = true;

			}

			if (request || permissionStatus != PermissionStatus.Granted)
			{
				permissionStatus = await permission.RequestPermissionAsync();
				

				if (permissionStatus != PermissionStatus.Granted)
				{
					var title = $"{permission} Permission";
					var question = $"To use the plugin the {permission} permission is required.";
					var positive = "Settings";
					var negative = "Maybe Later";
					var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					if (task == null)
						return permissionStatus;

					var result = await task;
					if (result)
					{
						CrossPermissions.Current.OpenAppSettings();
					}
					return permissionStatus;
				}
			}

			return permissionStatus;
		}
	}
}
