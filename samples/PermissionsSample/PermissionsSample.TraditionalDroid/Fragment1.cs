using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;

using Plugin.Geolocator;
using Plugin.Permissions;
using Android.Runtime;
using Android.Content.PM;
using Android.Widget;

namespace PermissionsSample.TraditionalDroid
{
    public class Fragment1 : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public static Fragment1 NewInstance()
        {
            var frag1 = new Fragment1 { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment1, null);

            var text = view.FindViewById<TextView>(Resource.Id.textView1);
            text.Clickable = true;

            text.Click += async (sender, args) =>
            {
                try
                {
                    var location = await CrossGeolocator.Current.GetPositionAsync(10000);
                    Toast.MakeText(Activity, $"{location.Latitude}", ToastLength.Long).Show();
                }
                catch
                {
                    Toast.MakeText(Activity, "error", ToastLength.Short).Show();
                }
            };

           

            return view;
        }
    
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}