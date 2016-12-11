using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{
	public class DotNetBuildConfiguration : Configuration
	{
		public string OutputPath { get; internal set; }

		internal DotNetBuildConfiguration(BuildConfiguration build, PlatformConfiguration platform, TargetConfiguration target, AppConfiguration app)
			: base(build, platform, target, app)
		{
			OutputPath = target.DotNet.OutputPath;
		}
	}
}
