Permissions Readme

Changelog:
[1.2.0]
-Add Microphone Permission
-Initial UWP support
-Build against 23.3.0 Support Packages

**IMPORTANT**
Android:
You must set your app to compile against API 23 or higher. It is required that you add the following override to any Activity that you will be requesting permissions from:

public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
{
    Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}

Additionally, Plugin.CurrentActivity was installed to propogate the current Activity up to this plugin. Please ensure that your Application class is correct configured.

iOS:

When building against the iOS 10 SDK (Xcode 8) please be aware of the platform privacy changes. Based on what permissions you are using, you must add information into your info.plist. Please read the following blog for more information. https://blog.xamarin.com/new-ios-10-privacy-permission-settings/
