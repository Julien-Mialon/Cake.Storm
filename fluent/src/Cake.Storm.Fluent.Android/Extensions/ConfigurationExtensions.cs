using System;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Android.Models;
using Cake.Storm.Fluent.Android.Steps;
using Cake.Storm.Fluent.Common.Steps;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Android.Extensions
{
	public static class ConfigurationExtensions
	{
		public static TConfiguration UseAndroidManifestTransformation<TConfiguration>(this TConfiguration configuration, string manifestFile, Action<IAndroidManifestTransformation> transformerAction = null)
			where TConfiguration : IConfiguration
		{
			IAndroidManifestTransformationAction transformation = new AndroidManifestTransformation();
			transformerAction?.Invoke(transformation);

			configuration.AddStep(new AndroidManifestTransformationStep(manifestFile, transformation));
			return configuration;
		}

		public static TConfiguration UseKeystore<TConfiguration>(this TConfiguration configuration, string keyStoreFile, Action<IKeystoreConfiguration> configurator)
			where TConfiguration : IConfiguration
		{
			return configuration.UseKeystore(keystoreConfiguration =>
			{
				keystoreConfiguration.WithFile(keyStoreFile);
				configurator?.Invoke(keystoreConfiguration);
			});
		}

		public static TConfiguration UseKeystore<TConfiguration>(this TConfiguration configuration, Action<IKeystoreConfiguration> configurator)
			where TConfiguration : IConfiguration
		{
			IKeystoreConfiguration keystoreConfiguration = new KeystoreConfiguration(configuration);
			configurator(keystoreConfiguration);

			configuration.AddStep(new KeystoreValidationStep());
			configuration.AddStep(new SignApkWithKeystoreStep());
			configuration.AddStep(new SignAabWithKeystoreStep());

			return configuration;
		}

		public static TConfiguration WithAndroidPackage<TConfiguration>(this TConfiguration configuration, string package)
			where TConfiguration : IConfiguration
		{
			configuration.Add(AndroidConstants.ANDROID_PACKAGE_KEY, new SimpleConfigurationItem<string>(package));
			return configuration;
		}

		public static TConfiguration UseAndroidTooling<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			configuration.AddStep(new NugetRestoreStep());
			configuration.AddStep(new MSBuildSolutionStep());
			configuration.AddStep(new AndroidReleaseStep());

			return configuration;
		}

		public static TConfiguration UseAppBundle<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			configuration.Add(AndroidConstants.ANDROID_USE_AAB, new SimpleConfigurationItem<bool>(true));
			return configuration;
		}

		public static TConfiguration UseApk<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			configuration.Add(AndroidConstants.ANDROID_USE_APK, new SimpleConfigurationItem<bool>(true));
			return configuration;
		}
	}
}