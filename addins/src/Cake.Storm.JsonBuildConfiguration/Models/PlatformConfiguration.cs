using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class PlatformConfiguration
	{
		[JsonProperty("name")]
		public PlatformType Name { get; set; }

		[JsonProperty("build")]
		public BuildConfiguration Build { get; set; }

		public static bool AreEqual(PlatformConfiguration a, PlatformConfiguration b)
		{
			return a.Name == b.Name;
		}

		public static PlatformConfiguration Merge(PlatformConfiguration source, PlatformConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}
			return new PlatformConfiguration
			{
				Name = source.Name,
				Build = BuildConfiguration.Merge(source.Build, overwrite.Build)
			};
		}
	}
}
