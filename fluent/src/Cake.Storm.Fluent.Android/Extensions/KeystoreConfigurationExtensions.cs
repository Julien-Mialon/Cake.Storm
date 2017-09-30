using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.Android.Extensions
{
	public static class KeystoreConfigurationExtensions
	{
		public static IKeystoreConfiguration WithFile(this IKeystoreConfiguration configuration, string file)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_FILE, file);
			return configuration;
		}
		
		public static IKeystoreConfiguration WithAuthority(this IKeystoreConfiguration configuration, string authority)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_AUTHORITY, authority);
			return configuration;
		}
		
		public static IKeystoreConfiguration WithPassword(this IKeystoreConfiguration configuration, string password)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_PASSWORD, password);
			return configuration;
		}
		
		public static IKeystoreConfiguration WithKeyAlias(this IKeystoreConfiguration configuration, string alias)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_KEYALIAS, alias);
			return configuration;
		}
		
		public static IKeystoreConfiguration WithKeyPassword(this IKeystoreConfiguration configuration, string password)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_KEYPASSWORD, password);
			return configuration;
		}
		
		public static IKeystoreConfiguration WithKey(this IKeystoreConfiguration configuration, string alias, string password)
		{
			return configuration.WithKeyAlias(alias)
				.WithKeyPassword(password);
		}
		
		public static IKeystoreConfiguration AllowCreate(this IKeystoreConfiguration configuration, bool allowCreate)
		{
			configuration.Configuration.AddSimple(AndroidConstants.ANDROID_KEYSTORE_ALLOW_CREATE, allowCreate);
			return configuration;
		}
	}
}