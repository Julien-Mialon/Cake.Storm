using System;
using Newtonsoft.Json;

namespace Cake.Storm.Nuget
{
	public class NugetAppConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("releaseNoteFile")]
		public string ReleaseNoteFile { get; set; }

		[JsonProperty("nuspecFile")]
		public string NuspecFile { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("packOutput")]
		public string PackOutput { get; set; }

		[JsonProperty("pushSource")]
		public string PushSource { get; set; }
	}
}
