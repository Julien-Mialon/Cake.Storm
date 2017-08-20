using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.NuGet.Interfaces;
using Cake.Storm.Fluent.NuGet.Models;
using Cake.Storm.Fluent.NuGet.Steps;

namespace Cake.Storm.Fluent.NuGet.Extensions
{
	public static class ConfigurationExtensions
	{
		public static TConfiguration UseNugetPack<TConfiguration>(this TConfiguration configuration, Action<INugetPackConfiguration> configurator)
			where TConfiguration : class, IConfiguration
		{
			INugetPackConfiguration packConfiguration = new NugetPackConfiguration(configuration);
			configurator.Invoke(packConfiguration);
			
			configuration.AddStep(new NugetPackStep());
			return configuration;
		}
		
		public static TConfiguration UseNugetPush<TConfiguration>(this TConfiguration configuration, Action<INugetPushConfiguration> configurator = null)
			where TConfiguration : IConfiguration
		{
			INugetPushConfiguration pushConfiguration = new NugetPushConfiguration(configuration);
			configurator?.Invoke(pushConfiguration);
			
			configuration.AddStep(new NugetPushStep());
			return configuration;
		}
	}
}