## Permissions Plugin for Xamarin

Simple cross platform plugin to request and check permissions.

Want to read about the creation, checkout my [in-depth blog post](http://motzcod.es/post/133939517717/simplified-ios-android-runtime-permissions-with).

### The Future: [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/index?WT.mc_id=docs-github-jamont)

I have been working on Plugins for Xamarin for a long time now. Through the years I have always wanted to create a single, optimized, and official package from the Xamarin team at Microsoft that could easily be consumed by any application. The time is now with [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/index?WT.mc_id=docs-github-jamont), which offers over 50 cross-platform native APIs in a single optimized package. I worked on this new library with an amazing team of developers and I highly highly highly recommend you check it out.

I will continue to work and maintain my Plugins, but I do recommend you checkout Xamarin.Essentials to see if it is a great fit your app as it has been for all of mine!


### Setup
* Available on NuGet: http://www.nuget.org/packages/Plugin.Permissions [![NuGet](https://img.shields.io/nuget/v/Plugin.Permissions.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Permissions/)
* Install into your PCL/.NET Standard project and Client projects.
* Development NuGet: https://www.myget.org/feed/Packages/xamarin-plugins

**Platform Support**

|Platform|Version|
| ------------------- | :-----------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 14+|
|Windows 10 UWP(Beta)|10+|

*See platform notes below

Build Status: ![Build status](https://jamesmontemagno.visualstudio.com/_apis/public/build/definitions/6b79a378-ddd6-4e31-98ac-a12fcd68644c/19/badge)

### Android specific in your BaseActivity or MainActivity (for Xamarin.Forms) add this code:
```csharp
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```

## Android Setup

This plugin uses the [Xamarin.Essentials](https://docs.microsoft.com/xamarin/essentials/), please follow the setup guide.

```csharp
Xamarin.Essentials.Platform.Init(this, bundle);
```

### iOS Specific
Based on what permissions you are using, you must add information into your info.plist. Please read the [Working with Security and Privacy guide for keys you will need to add](https://developer.xamarin.com/guides/ios/application_fundamentals/security-privacy-enhancements/). 


### API Usage

You are able to check and requests permissions with just a few lines of code:

Check permission: 

```csharp
PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<CalendarPermission>();
```

Request permission:
```csharp
PermissionStatus status = await CrossPermissions.Current.RequestPermissionAsync<CalendarPermission>();
```

Additionally on Android there is a situation where you may want to detect if the user has already declined the permission and you should show your own pop up:

```csharp
bool shouldShow = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Calendar);
```

#### Available Permissions

* CalendarPermission
* CameraPermission
* ContactsPermission
* LocationPermission
* LocationAlwaysPermission
* LocationWhenInUsePermission
* MediaLibraryPermission
* MicrophonePermission
* PhonePermission
* PhotosPermission
* RemindersPermission
* SensorsPermission
* SmsPermission
* StoragePermission
* SpeechPermission


### In Action
Here is how you may use it with geolocation:

```csharp
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
		//Query permission
	}
	else if (status != PermissionStatus.Unknown)
	{
		//location denied
	}
}
catch (Exception ex)
{
  //Something went wrong
}
```




Read more about android permissions: http://developer.android.com/guide/topics/security/permissions.html#normal-dangerous


### IMPORTANT
#### Android:

You still need to request the permissions in your AndroidManifest.xml. Also ensure your MainApplication.cs was setup correctly from the CurrentActivity Plugin.

#### Windows 10 UWP
UWP has a limited set of supported permissions. You can see the documentation above, but current support: Contacts, Location, and Sensors.

#### Contributors
* Icon thanks to [Jérémie Laval](https://github.com/garuma)

Thanks!

#### License
Licensed under main repo license(MIT)

### Want To Support This Project?
All I have ever asked is to be active by submitting bugs, features, and sending those pull requests down! Want to go further? Make sure to subscribe to my weekly development podcast [Merge Conflict](http://mergeconflict.fm), where I talk all about awesome Xamarin goodies and you can optionally support the show by becoming a [supporter on Patreon](https://www.patreon.com/mergeconflictfm).
