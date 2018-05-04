Permissions Readme


**IMPORTANT**
Android:

public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
	PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}

## Android Current Activity Setup

This plugin uses the [Current Activity Plugin](https://github.com/jamesmontemagno/CurrentActivityPlugin/blob/master/README.md) to get access to the current Android Activity. Be sure to complete the full setup if a MainApplication.cs file was not automatically added to your application. Please fully read through the [Current Activity Plugin Documentation](https://github.com/jamesmontemagno/CurrentActivityPlugin/blob/master/README.md). At an absolute minimum you must set the following in your Activity's OnCreate method:

```csharp
Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
```

It is highly recommended that you use a custom Application that are outlined in the Current Activity Plugin Documentation](https://github.com/jamesmontemagno/CurrentActivityPlugin/blob/master/README.md)

### iOS Specific
Based on what permissions you are using, you must add information into your info.plist. Please read the [Working with Security and Privacy guide for keys you will need to add](https://developer.xamarin.com/guides/ios/application_fundamentals/security-privacy-enhancements/). 

Due to API usage it is required to add the Calendar permission :(
```
<key>NSCalendarsUsageDescription</key>
<string>Needs Calendar Permission</string>
```
Even though your app may not use calendar at all. I am looking into a workaround for this in the future.