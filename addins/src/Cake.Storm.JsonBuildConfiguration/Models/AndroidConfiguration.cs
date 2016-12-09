using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class AndroidConfiguration
	{
		[JsonProperty("package")]
		public string Package { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("version_code")]
		public string VersionCode { get; set; }

		[JsonProperty("keystore")]
		public AndroidKeystoreConfiguration Keystore { get; set; }

		public static AndroidConfiguration Merge(AndroidConfiguration source, AndroidConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new AndroidConfiguration
			{
				Package = overwrite.Package ?? source.Package,
				Version = overwrite.Version ?? source.Version,
				VersionCode = overwrite.VersionCode ?? source.VersionCode,
				Keystore = AndroidKeystoreConfiguration.Merge(source.Keystore, overwrite.Keystore)
			};
		}
	}

}
