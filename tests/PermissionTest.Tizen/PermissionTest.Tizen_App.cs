using Tizen.Applications;
using ElmSharp;
using System.Diagnostics;
using System;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace PermissionTest.Tizen
{
    class App : CoreUIApplication
    {
		private ToastMessage toast;

		protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        void Initialize()
        {
			toast = new ToastMessage();

			Window window = new Window("ElmSharpApp")
            {
                AvailableRotations = DisplayRotation.Degree_0 | DisplayRotation.Degree_180 | DisplayRotation.Degree_270 | DisplayRotation.Degree_90
            };
            window.BackButtonPressed += (s, e) =>
            {
                Exit();
            };
            window.Show();

            var box = new Box(window)
            {
                AlignmentX = -1,
                AlignmentY = -1,
                WeightX = 1,
                WeightY = 1,
            };
            box.Show();

            var bg = new Background(window)
            {
                Color = Color.White
            };
            bg.SetContent(box);

            var conformant = new Conformant(window);
            conformant.Show();
            conformant.SetContent(bg);

			var CheckPermissionStatus = new Button(window)
			{
				Text = "CheckPermissionStatus",
				AlignmentX = -1,
				AlignmentY = -1,
				WeightX = 1,
			};
			CheckPermissionStatus.Clicked += CheckPermissionStatus_ClickedAsync;
			CheckPermissionStatus.Show();
			box.PackEnd(CheckPermissionStatus);
		}

        static void Main(string[] args)
        {
            Elementary.Initialize();
            Elementary.ThemeOverlay();
            App app = new App();
            app.Run(args);
        }

		private void PostToastMessage(string message)
		{
			toast.Message = message;
			toast.Post();
			Debug.WriteLine(message);
		}

		private async void CheckPermissionStatus_ClickedAsync(object sender, EventArgs e)
		{
			try
			{
				PostToastMessage("Check Calendar Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Calendar));
				PostToastMessage("Check Camera Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera));
				PostToastMessage("Check Contacts Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Contacts));
				PostToastMessage("Check Location Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location));
				PostToastMessage("Check Microphone Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone));
				PostToastMessage("Check Phone Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone));
				PostToastMessage("Check Photos Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos));
				PostToastMessage("Check Reminders Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Reminders));
				PostToastMessage("Check Sensors Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors));
				PostToastMessage("Check Sms Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sms));
				PostToastMessage("Check Storage Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage));
				PostToastMessage("Check Speech Permission : " + await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Speech));
			}
			catch (Exception ex)
			{
				PostToastMessage(ex.Message);
			}
		}
	}
}
