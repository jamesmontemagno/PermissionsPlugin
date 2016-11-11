using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace PermissionsSample.TraditionalDroid
{
    [Activity(Label = "SecondActivity", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class SecondActivity : BaseActivity
    {
        protected override int LayoutResource
        {
            get { return Resource.Layout.second; }
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            int count = Intent.GetIntExtra("clicks", 0);
            var text = FindViewById<TextView>(Resource.Id.textView1);
            text.Text = string.Format("You clicked {0} times!", count);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}