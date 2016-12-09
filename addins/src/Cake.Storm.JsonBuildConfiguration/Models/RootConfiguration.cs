using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class RootConfiguration
	{
		[JsonProperty("build")]
		internal BuildConfiguration Build { get; set; }

		[JsonProperty("platforms")]
		internal List<PlatformConfiguration> Platforms { get; set; }

		[JsonProperty("targets")]
		internal List<TargetConfiguration> Targets { get; set; }

		[JsonProperty("apps")]
		internal List<AppConfiguration> Apps { get; set; }
	}
}
