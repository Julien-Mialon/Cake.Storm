using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cake.Storm.JsonBuildConfiguration
{
	public abstract class Configuration
	{
		public string PlatformName { get; internal set; }

		public string TargetName { get; internal set; }

		public string Solution { get; internal set; }

		public string Project { get; internal set; }

		public Dictionary<string, string> BuildProperties { get; }

		protected Configuration()
		{

		}
	}

	public class AndroidBuildConfiguration : Configuration
	{
		public string Package { get; internal set; }

		public string Version { get; internal set; }

		public string VersionCode { get; internal set; }

		public string KeystoreFile { get; internal set; }

		public string KeystoreAuthority { get; internal set; }

		public string KeystorePassword { get; internal set; }

		public string KeystoreKeyAlias { get; internal set; }

		public string KeystoreKeyPassword { get; internal set; }
	}

	public class iOSBuildConfiguration : Configuration
	{
		public string Bundle { get; internal set; }

		public string Version { get; internal set; }

		public string BuildVersion { get; internal set; }
	}

	public class DotNetBuildConfiguration : Configuration
	{

	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum PlatformType
	{
		Undefined,
		Android,
		iOS,
		Dotnet
	}
}
