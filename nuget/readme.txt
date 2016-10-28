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

