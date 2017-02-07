using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{

	public class iOSBuildConfiguration : Configuration
	{
		public string Bundle { get; internal set; }

		public string Version { get; internal set; }

		public string BuildVersion { get; internal set; }

		public string PListFile { get; internal set; }

		public string CodesignKey { get; set; }

		public string CodesignProvision { get; set; }

		public FastlaneSighBuildConfiguration Fastlane {get;set;}

		internal iOSBuildConfiguration(BuildConfiguration build, PlatformConfiguration platform, TargetConfiguration target, AppConfiguration app)
			: base(build, platform, target, app)
		{
			iOSConfiguration iOS = target.iOS;

			if (iOS == null)
			{
				return;
			}

			Bundle = iOS.Bundle;
			Version = iOS.Version;
			BuildVersion = iOS.BuildVersion;
			CodesignKey = iOS.CodesignKey;
			CodesignProvision = iOS.CodesignProvision;
			PListFile = iOS.PListFile;
			Fastlane = new FastlaneSighBuildConfiguration(iOS.Fastlane);
		}
	}

	public class FastlaneSighBuildConfiguration
	{
		public bool IsEnabled { get; set; }

		public string UserName { get; set; }
		
		public string TeamName { get; set; }

		public string ProvisioningName { get; set; }

		public bool AdHoc { get; set; }

		public bool AppStore { get; set; }

		public bool Development { get; set; }


		public FastlaneSighBuildConfiguration(FastlaneSighConfiguration fastlane)
		{
			IsEnabled = fastlane != null;
			if(!IsEnabled)
			{
				return;
			}

			UserName = fastlane.UserName;
			TeamName = fastlane.TeamName;
			ProvisioningName = fastlane.ProvisioningName;
			AdHoc = fastlane?.AdHoc ?? false;
			AppStore = fastlane?.AppStore ?? false;
			Development = fastlane?.Development ?? false;
		}
	}
}
