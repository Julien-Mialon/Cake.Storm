using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class AppConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("build")]
		public BuildConfiguration Build { get; set; }

		[JsonProperty("platforms")]
		public List<PlatformConfiguration> Platforms { get; set; }

		[JsonProperty("targets")]
		public List<TargetConfiguration> Targets { get; set; }
	}
}
