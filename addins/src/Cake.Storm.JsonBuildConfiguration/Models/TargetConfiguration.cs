using System.Collections.Generic;
using Cake.Storm.JsonBuildConfiguration.Helpers;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class TargetConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("build")]
		public BuildConfiguration Build { get; set; }

		[JsonProperty("platforms")]
		public List<PlatformConfiguration> Platforms { get; set; }

		[JsonProperty("android")]
		public AndroidConfiguration Android { get; set; }

		[JsonProperty("iOS")]
		public iOSConfiguration iOS { get; set; }

		[JsonProperty("dotnet")]
		public DotNetConfiguration DotNet { get; set; }

		public static bool AreEqual(TargetConfiguration a, TargetConfiguration b)
		{
			return a.Name == b.Name;
		}

		public static TargetConfiguration Merge(TargetConfiguration source, TargetConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}
			return new TargetConfiguration
			{
				Name = source.Name,
				Build = BuildConfiguration.Merge(source.Build, overwrite.Build),
				Platforms = MergeHelper.MergeList(source.Platforms, overwrite.Platforms, PlatformConfiguration.AreEqual, PlatformConfiguration.Merge),
				Android = AndroidConfiguration.Merge(source.Android, overwrite.Android),
				iOS = iOSConfiguration.Merge(source.iOS, overwrite.iOS),
				DotNet = DotNetConfiguration.Merge(source.DotNet, overwrite.DotNet)
			};
		}
	}
}
