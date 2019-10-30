using System;
using Cake.Storm.Fluent.AppCenter.Interfaces;
using Cake.Storm.Fluent.AppCenter.Models;
using Cake.Storm.Fluent.AppCenter.Steps;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.AppCenter.Extensions
{
	public static class ConfigurationExtensions
	{
		public static TConfiguration ConfigureAppCenter<TConfiguration>(this TConfiguration configuration, Action<IAppCenterConfiguration> configurator)
			where TConfiguration : IConfiguration
		{
			IAppCenterConfiguration appCenterConfiguration = new AppCenterConfiguration(configuration);
			configurator(appCenterConfiguration);

			return configuration;
		}

		public static TConfiguration DeployAndroidPackageToAppCenter<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			configuration.AddStep(new DeployAndroidPackageStep());

			return configuration;
		}

		public static TConfiguration DeployiOSPackageToAppCenter<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			configuration.AddStep(new DeployiOSPackageStep());

			return configuration;
		}
	}
}