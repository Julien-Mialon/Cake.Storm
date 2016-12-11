using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{

	internal class iOSConfiguration
	{
		[JsonProperty("bundle")]
		public string Bundle { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("build_version")]
		public string BuildVersion { get; set; }

		public static iOSConfiguration Merge(iOSConfiguration source, iOSConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new iOSConfiguration
			{
				Bundle = overwrite.Bundle ?? source.Bundle,
				Version = overwrite.Version ?? source.Version,
				BuildVersion = overwrite.BuildVersion ?? source.BuildVersion
			};
		}
	}
	
}
