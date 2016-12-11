using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{

	public class iOSBuildConfiguration : Configuration
	{
		public string Bundle { get; internal set; }

		public string Version { get; internal set; }

		public string BuildVersion { get; internal set; }

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
		}
	}

}
