Permissions Readme

Introducing Version 5.0! There is a brand new API that introduces completely linker safe permissions for iOS, which is awesome!

Please read: https://github.com/jamesmontemagno/PermissionsPlugin/blob/master/README.md for the new API


**IMPORTANT**
Android:

public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
	PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}

## Android Setup

This plugin uses the [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/), please follow the setup guide.

```csharp
Xamarin.Essentials.Platform.Init(this, bundle);
```

It is highly recommended that you use a custom Application that are outlined in the Current Activity Plugin Documentation](https://github.com/jamesmontemagno/CurrentActivityPlugin/blob/master/README.md)

### iOS Specific
Based on what permissions you are using, you must add information into your info.plist. Please read the [Working with Security and Privacy guide for keys you will need to add](https://developer.xamarin.com/guides/ios/application_fundamentals/security-privacy-enhancements/). 