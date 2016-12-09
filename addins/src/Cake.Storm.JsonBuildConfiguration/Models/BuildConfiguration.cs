using System.Collections.Generic;
using Cake.Storm.JsonBuildConfiguration.Helpers;
using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class BuildConfiguration
	{
		[JsonProperty("solution")]
		public string Solution { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("properties")]
		public List<BuildProperty> Properties { get; set; } = new List<BuildProperty>();

		public static BuildConfiguration Merge(BuildConfiguration source, BuildConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new BuildConfiguration
			{
				Solution = overwrite.Solution ?? source.Solution,
				Project = overwrite.Project ?? source.Project,
				Properties = MergeHelper.MergeList(source.Properties, overwrite.Properties, BuildProperty.AreEqual, BuildProperty.Merge)
			};
		}
	}
}
