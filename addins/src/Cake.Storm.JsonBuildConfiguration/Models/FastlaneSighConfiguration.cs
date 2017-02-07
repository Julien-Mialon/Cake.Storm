using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	public class FastlaneSighConfiguration
	{
		[JsonProperty("user_name")]
		public string UserName { get; set; }

		[JsonProperty("team_name")]
		public string TeamName { get; set; }

		[JsonProperty("provisioning_name")]
		public string ProvisioningName { get; set; }

		[JsonProperty("ad_hoc")]
		public bool? AdHoc { get; set; }

		[JsonProperty("app_store")]
		public bool? AppStore { get; set; }

		[JsonProperty("development")]
		public bool? Development { get; set; }

		public static FastlaneSighConfiguration Merge(FastlaneSighConfiguration source, FastlaneSighConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new FastlaneSighConfiguration
			{
				UserName = overwrite.UserName ?? source.UserName,
				TeamName = overwrite.TeamName ?? source.TeamName,
				ProvisioningName = overwrite.ProvisioningName ?? source.ProvisioningName,
				AdHoc = overwrite.AdHoc ?? source.AdHoc,
				AppStore = overwrite.AppStore ?? source.AppStore,
				Development = overwrite.Development ?? source.Development
			};
		}
	}
}