using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{

	public class AndroidBuildConfiguration : Configuration
	{
		//from AndroidConfiguration
		public string Package { get; internal set; }

		public string Version { get; internal set; }

		public string VersionCode { get; internal set; }

		//from AndroidKeystoreConfiguration
		public string KeystoreFile { get; internal set; }

		public string KeystoreAuthority { get; internal set; }

		public string KeystorePassword { get; internal set; }

		public string KeystoreKeyAlias { get; internal set; }

		public string KeystoreKeyPassword { get; internal set; }

		internal AndroidBuildConfiguration(BuildConfiguration build, PlatformConfiguration platform, TargetConfiguration target, AppConfiguration app)
			: base(build, platform, target, app)
		{
			AndroidConfiguration android = target.Android;

			if (android == null)
			{
				return;
			}
			Package = android.Package;
			Version = android.Version;
			VersionCode = android.VersionCode;

			AndroidKeystoreConfiguration keystore = android.Keystore;

			if (keystore == null)
			{
				return;
			}

			KeystoreFile = keystore.File;
			KeystoreAuthority = keystore.Authority;
			KeystorePassword = keystore.Password;
			KeystoreKeyAlias = keystore.KeyAlias;
			KeystoreKeyPassword = keystore.KeyPassword;
		}
	}

}
