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

		public static TConfiguration UseKeystore<TConfiguration>(this TConfiguration configuration, string keyStoreFile, Action<IKeystore> configurator)
			where TConfiguration : IConfiguration
		{
			IKeystoreAction keystore = new Keystore();
			configurator(keystore);

			configuration.AddStep(new KeystoreValidationStep(keyStoreFile, keystore));
			configuration.AddStep(new SignPackageWithKeystoreStep(keyStoreFile, keystore));

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
			configuration.AddStep(new NugetRestoreAllStep());
			configuration.AddStep(new MSBuildSolutionStep());
			configuration.AddStep(new AndroidReleaseStep());
			
			return configuration;
		}
	}
}