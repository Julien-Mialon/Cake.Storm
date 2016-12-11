using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class DotNetConfiguration
	{
		[JsonProperty("output")]
		public string OutputPath { get; set; }

		public static DotNetConfiguration Merge(DotNetConfiguration source, DotNetConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new DotNetConfiguration
			{
				OutputPath = overwrite.OutputPath ?? source.OutputPath
			};
		}
	}
}
