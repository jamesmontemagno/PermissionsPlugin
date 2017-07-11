Permissions Readme

You can find a full change log here: https://github.com/jamesmontemagno/PermissionsPlugin/blob/master/CHANGELOG.md

## News
- Plugins have moved to .NET Standard and have some important changes! Please read my blog:
http://motzcod.es/post/162402194007/plugins-for-xamarin-go-dotnet-standard

**IMPORTANT**
Android:
You must set your app to compile against API 25 or higher. It is required that you add the following override to any Activity that you will be requesting permissions from:

public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
	base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}

Additionally, Plugin.CurrentActivity was installed to propogate the current Activity up to this plugin. Please ensure that your Application class is correct configured.

iOS:

When building against the iOS 10 SDK (Xcode 8) please be aware of the platform privacy changes. Based on what permissions you are using, you must add information into your info.plist. Please read the following blog for more information. https://blog.xamarin.com/new-ios-10-privacy-permission-settings/
