using Cake.Storm.Fluent.DotNetCore.Steps;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.DotNetCore.Extensions
{
    public static class DotNetExtensions
    {
	    public static TConfiguration UseDotNetCoreTooling<TConfiguration>(this TConfiguration configuration)
		    where TConfiguration : IPlatformConfiguration
	    {
		    configuration.AddStep(new DotNetRestoreStep());
			configuration.AddStep(new DotNetBuildStep());
			configuration.AddStep(new DotNetReleaseStep());
		    return configuration;
	    }
    }
}
