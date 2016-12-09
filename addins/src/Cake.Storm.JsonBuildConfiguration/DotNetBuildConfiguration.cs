using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{

	public class DotNetBuildConfiguration : Configuration
	{
		internal DotNetBuildConfiguration(BuildConfiguration build, PlatformConfiguration platform, TargetConfiguration target, AppConfiguration app)
			: base(build, platform, target, app)
		{

		}
	}

}
