namespace Plugin.Permissions.Abstractions
{
    /// <summary>
    /// Status of a permission
    /// </summary>
    public enum PermissionStatus
    {
        /// <summary>
        /// Denied by user
        /// </summary>
        Denied,
        /// <summary>
        /// Feature is disabled on device
        /// </summary>
        Disabled,
        /// <summary>
        /// Granted by user
        /// </summary>
        Granted,
        /// <summary>
        /// Restricted (only iOS)
        /// </summary>
        Restricted,
        /// <summary>
        /// Permission is in an unknown state
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Permission group that can be requested
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// The unknown permission only used for return type, never requested
        /// </summary>
        Unknown,
        /// <summary>
        /// Android: Camera
        /// iOS: Photos (Camera Roll and Camera)
        /// UWP: None
        /// </summary>
        Camera,
        /// <summary>
        /// Android: Contacts
        /// iOS: AddressBook
        /// UWP: ContactManager
        /// </summary>
        Contacts,
        /// <summary>
        /// Android: Fine and Coarse Location
        /// iOS: CoreLocation (Always and WhenInUse)
        /// UWP: Geolocator
        /// </summary>
        Location,
		/// <summary>
		/// Android: Microphone
		/// iOS: Microphone
		/// UWP: None
		/// </summary>
		Microphone,
        /// <summary>
        /// Android: Phone
        /// iOS: Nothing
        /// </summary>
        Phone,
        /// <summary>
        /// Android: Nothing
        /// iOS: Photos
        /// UWP: None
        /// </summary>
        Photos,
        /// <summary>
        /// Android: Body Sensors
        /// iOS: CoreMotion
        /// UWP: DeviceAccessInformation
        /// </summary>
        Sensors,
        /// <summary>
        /// Android: Sms
        /// iOS: Nothing
        /// UWP: None
        /// </summary>
        Sms,
        /// <summary>
        /// Android: External Storage
        /// iOS: Nothing
        /// </summary>
        Storage,
        /// <summary>
        /// Android: Microphone
        /// iOS: Speech
        /// UWP: None
        /// </summary>
        Speech,
		/// <summary>
		/// Android: Fine and Coarse Location
		/// iOS: CoreLocation - Always
		/// UWP: Geolocator
		/// </summary>
		LocationAlways,
		/// <summary>
		/// Android: Fine and Coarse Location
		/// iOS: CoreLocation - WhenInUse
		/// UWP: Geolocator
		/// </summary>
		LocationWhenInUse,
		/// <summary>
		/// Android: None
		/// iOS: MPMediaLibrary
		/// UWP: None
		/// </summary>
		MediaLibrary
	}
}
