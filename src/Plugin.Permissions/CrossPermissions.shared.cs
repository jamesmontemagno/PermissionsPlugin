using Plugin.Permissions.Abstractions;
using System;

// peymanr34 created this snippet! April 19, 2019

namespace Plugin.Permissions
{
    /// <summary>
    /// Cross platform Permissions implemenations
    /// </summary>
    public static class CrossPermissions
    {
        static Lazy<IPermissions> implementation = new Lazy<IPermissions>(CreatePermissions, System.Threading.LazyThreadSafetyMode.PublicationOnly);
		/// <summary>
		/// Gets if the plugin is supported on the current platform.
		/// </summary>
		public static bool IsSupported => implementation.Value == null ? false : true;

		/// <summary>
		/// Current plugin implementation to use
		/// </summary>
		public static IPermissions Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IPermissions CreatePermissions()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
            return new PermissionsImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
			new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        
    }
}
