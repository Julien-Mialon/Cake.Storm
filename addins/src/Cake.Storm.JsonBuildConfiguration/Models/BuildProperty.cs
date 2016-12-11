using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class BuildProperty
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		public static bool AreEqual(BuildProperty a, BuildProperty b)
		{
			return a.Key == b.Key;
		}

		public static BuildProperty Merge(BuildProperty source, BuildProperty overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new BuildProperty
			{
				Key = source.Key,
				Value = overwrite.Value ?? source.Value
			};
		}
	}
}
