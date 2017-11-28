### 2.1.1
* Android: Fix edgecase when permission is inserted twice into returned dictionary

### 2.1.0
* Add MediaLibrary permission for iOS's MPMediaLibrary
* Add LocationAlways & LocationWhenInUse permissions for iOS to directly ask
* Fix some issues with locking on iOS

### 2.0.0
* Re-target .NET Standard
* Dependent on Android 25.3.1 Support Libraries

### 1.2.0
* Add microphone permission
* Initial UWP permissions
* Use 23.3.0 Android Support Library

### 1.1.7
* Fix issue for Windows Config

###  1.1.6
* Fix for checking photos permission on iOS #192

### 1.1.5
* Fix odd instance where can't check manifest because current activity is null, now use application context.
* If Activity is null when requesting then go ahead and return list of unknown permissions

### 1.1.4
* Fix for #169, odd freeze for certain iOS permissions when requesting a second time in a denied state.

### 1.1.3
* Remove help file, but add readme.txt

### 1.1.1
* Fix odd edgecase in Android for nothing in Manifest.
* Use Context instead of Activity when checking permisson if we can.
* Change to params of permissions when requesting permissions.
