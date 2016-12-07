using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	public class Configuration
	{
		[JsonProperty("build")]
		internal BuildConfiguration BuildConfiguration { get; set; }

		[JsonProperty("platforms")]
		internal List<PlatformConfiguration> PlatformsConfiguration { get; set; }

		[JsonProperty("apps")]
		internal List<AppConfiguration> AppsConfiguration { get; set; }
	}

	internal class BuildConfiguration
	{
		[JsonProperty("solution")]
		public string Solution { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("properties")]
		public List<BuildProperty> Properties { get; set; } = new List<BuildProperty>();
	}

	internal class BuildProperty
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }
	}

	internal class PlatformConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("build")]
		public BuildConfiguration BuildConfiguration { get; set; }
	}

	internal class AppConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("build")]
		public BuildConfiguration BuildConfiguration { get; set; }

		[JsonProperty("android")]
		public AndroidAppConfiguration AndroidConfigurations { get; set; }
	}

	internal class AndroidAppConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("configurations")]
		public List<AndroidConfiguration> Configurations { get; set; }
	}

	internal class AndroidConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("package")]
		public string Package { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("version_code")]
		public string VersionCode { get; set; }

		[JsonProperty("keystore")]
		public AndroidKeystoreConfiguration Keystore { get; set; }
	}

	internal class AndroidKeystoreConfiguration
	{
		[JsonProperty("file")]
		public string File { get; set; }

		[JsonProperty("authority")]
		public string Authority { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("key_alias")]
		public string KeyAlias { get; set; }

		[JsonProperty("key_password")]
		public string KeyPassword { get; set; }
	}
}
