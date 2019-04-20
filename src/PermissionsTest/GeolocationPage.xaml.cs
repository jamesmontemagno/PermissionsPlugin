using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace PermissionsTest
{
	public partial class GeolocationPage : ContentPage
	{
		public GeolocationPage()
		{
			InitializeComponent();
		}


		bool busy;
		async void ButtonPermission_OnClicked(object sender, EventArgs e)
		{
			if (busy)
				return;


			busy = true;
			((Button)sender).IsEnabled = false;

			var status = PermissionStatus.Unknown;
			switch (((Button)sender).StyleId)
			{
				case "Calendar":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<CalendarPermission>();
					break;
				case "Camera":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
					break;
				case "Contacts":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<ContactsPermission>();
					break;
				case "Location":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
					break;
				case "LocationAlways":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationAlwaysPermission>();
					break;
				case "LocationWhenInUse":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationWhenInUsePermission>();
					break;
				case "Microphone":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<MicrophonePermission>();
					break;
				case "Phone":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<PhonePermission>();
					break;
				case "Reminder":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<RemindersPermission>();
					break;
				case "Photos":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<PhotosPermission>();
					break;
				case "Sensors":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<SensorsPermission>();
					break;
				case "Sms":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<SmsPermission>();
					break;
				case "Storage":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
					break;
				case "MediaLibrary":
					status = await CrossPermissions.Current.CheckPermissionStatusAsync<MediaLibraryPermission>();
					break;
			}

			await DisplayAlert("Results", status.ToString(), "OK");

			if (status != PermissionStatus.Granted)
			{
				try
				{

					switch (((Button)sender).StyleId)
					{
						case "Calendar":
							status = await CrossPermissions.Current.RequestPermissionAsync<CalendarPermission>();
							break;
						case "Camera":
							status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
							break;
						case "Contacts":
							status = await CrossPermissions.Current.RequestPermissionAsync<ContactsPermission>();
							break;
						case "Location":
							status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
							break;
						case "LocationAlways":
							status = await CrossPermissions.Current.RequestPermissionAsync<LocationAlwaysPermission>();
							break;
						case "LocationWhenInUse":
							status = await CrossPermissions.Current.RequestPermissionAsync<LocationWhenInUsePermission>();
							break;
						case "Microphone":
							status = await CrossPermissions.Current.RequestPermissionAsync<MicrophonePermission>();
							break;
						case "Phone":
							status = await CrossPermissions.Current.RequestPermissionAsync<PhonePermission>();
							break;
						case "Photos":
							status = await CrossPermissions.Current.RequestPermissionAsync<PhotosPermission>();
							break;
						case "Sensors":
							status = await CrossPermissions.Current.RequestPermissionAsync<SensorsPermission>();
							break;
						case "Sms":
							status = await CrossPermissions.Current.RequestPermissionAsync<SmsPermission>();
							break;
						case "Storage":
							status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
							break;
						case "MediaLibrary":
							status = await CrossPermissions.Current.RequestPermissionAsync<MediaLibraryPermission>();
							break;
					}
					await DisplayAlert("Results", status.ToString(), "OK");
				}
				catch (Exception ex)
				{

					await DisplayAlert("Results", ex.Message, "OK");
				}


			}

			busy = false;
			((Button)sender).IsEnabled = true;
		}

		async void Button_OnClicked(object sender, EventArgs e)
		{
			if (busy)
				return;

			busy = true;
			((Button)sender).IsEnabled = false;

		    try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
					{
						await DisplayAlert("Need location", "Gunna need that location", "OK");
					}

					status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
				}

				if (status == PermissionStatus.Granted)
				{
					//var results = await CrossGeolocator.Current.GetPositionAsync(10000);
					//LabelGeolocation.Text = "Lat: " + results.Latitude + " Long: " + results.Longitude;
				}
				else if (status != PermissionStatus.Unknown)
				{
					await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
				}
			}
			catch (Exception ex)
			{

				LabelGeolocation.Text = "Error: " + ex;
			}

			((Button)sender).IsEnabled = true;
			busy = false;
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			CrossPermissions.Current.OpenAppSettings();
		}
	}
}

