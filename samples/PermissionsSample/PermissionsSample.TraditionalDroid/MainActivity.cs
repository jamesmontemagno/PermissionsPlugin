using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Geolocator;

namespace PermissionsSample.TraditionalDroid
{
    [Activity(Label = "PermissionsSample.TraditionalDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {

        protected override int LayoutResource
        {
            get { return Resource.Layout.main; }
        }
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Get our button from the layout resource,
            // and attach an event to it
            var clickButton = FindViewById<Button>(Resource.Id.my_button);

            clickButton.Click += async (sender, args) =>
              {
                  try
                  {
                      var location = await CrossGeolocator.Current.GetPositionAsync(10000);
                      Toast.MakeText(this, $"{location.Latitude}", ToastLength.Long).Show();
                  }
                  catch
                  {
                      Toast.MakeText(this, "error", ToastLength.Short).Show();
                  }

                  clickButton.Text = string.Format("{0} clicks!", count++);
              };

            var navigationButton = FindViewById<Button>(Resource.Id.nav_button);

            navigationButton.Click += (sender, args) =>
              {
                  var intent = new Intent(this, typeof(SecondActivity));
                  intent.PutExtra("clicks", count);
                  StartActivity(intent);
              };


            Android.Support.V4.App.Fragment fragment = null;

                    fragment = Fragment1.NewInstance();
                

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();


            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(false);

        }
    }
}

