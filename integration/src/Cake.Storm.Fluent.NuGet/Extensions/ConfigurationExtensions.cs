using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.NuGet.Extensions
{
	public static class ConfigurationExtensions
	{
		public static TConfiguration UseNuGetTooling<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			return configuration;
		}

		public static TConfiguration EnableNuGetPush<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			return configuration;
		}
	}
}