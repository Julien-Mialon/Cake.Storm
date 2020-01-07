using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.Android.Extensions
{
	internal static class PackageFormatHelper
	{
		public static bool IsAndroidAppBundleEnabled(this IConfiguration configuration)
		{
			return configuration.TryGetSimple(AndroidConstants.ANDROID_USE_AAB, out bool appBundleEnabled) && appBundleEnabled;
		}

		public static bool IsAndroidApkEnabled(this IConfiguration configuration)
		{
			return (configuration.TryGetSimple(AndroidConstants.ANDROID_USE_APK, out bool apkEnabled) && apkEnabled)
			       || !configuration.Has(AndroidConstants.ANDROID_USE_AAB);
		}
	}
}