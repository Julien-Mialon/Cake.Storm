using System;
using Newtonsoft.Json;

namespace Cake.Storm.HockeyApp
{
	public class HockeyAppConfiguration
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("app_id")]
		public string AppId { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("type")]
		public AppType Type { get; set; }

		[JsonProperty("apk")]
		public string ApkFile { get; set; }

		[JsonProperty("ipa")]
		public string IpaFile { get; set; }

		[JsonProperty("dsym")]
		public string DsymFile { get; set; }

		[JsonProperty("release_note")]
		public string ReleaseNoteFile { get; set; }
	}
}
