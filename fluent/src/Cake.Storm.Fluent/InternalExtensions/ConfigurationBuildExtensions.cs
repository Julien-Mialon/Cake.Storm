using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.Helpers;

namespace Cake.Storm.Fluent.InternalExtensions
{
	public static class ConfigurationBuildExtensions
	{
		public static void ApplyBuildParameters(this IConfiguration configuration, MSBuildSettings settings)
		{
			if (!configuration.Has(ConfigurationConstants.BUILD_PARAMETERS_KEY))
			{
				return;
			}

			var buildParameters = configuration.Get<DictionaryOfListConfigurationItem<string, string>>(ConfigurationConstants.BUILD_PARAMETERS_KEY);
			settings.ToolVersion = MSBuildToolVersion.VS2019;
			foreach (KeyValuePair<string, List<string>> buildParameter in buildParameters.Values)
			{
				if (buildParameter.Key.ToLowerInvariant() == "configuration")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Configuration can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					settings.SetConfiguration(MSBuildHelper.PropertyValue(buildParameter.Value.Single()));
				}
				else if (buildParameter.Key.ToLowerInvariant() == "platform")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Platform can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					string platform = buildParameter.Value.Single();
					switch (platform.ToLowerInvariant())
					{
						case "any cpu":
						case "anycpu":
							settings.SetPlatformTarget(PlatformTarget.MSIL);
							break;
						case "x86":
							settings.SetPlatformTarget(PlatformTarget.x86);
							break;
						case "x64":
							settings.SetPlatformTarget(PlatformTarget.x64);
							break;
						case "arm":
							settings.SetPlatformTarget(PlatformTarget.ARM);
							break;
						case "win32":
							settings.SetPlatformTarget(PlatformTarget.Win32);
							break;
						default:
							settings.WithProperty("Platform", MSBuildHelper.PropertyValue(platform));
							break;
					}
				}
				else if (buildParameter.Key.ToLowerInvariant() == "toolversion")
				{
					settings.ToolVersion = (MSBuildToolVersion)Enum.Parse(typeof(MSBuildToolVersion), buildParameter.Value.Single(), true);
				}
				else
				{
					settings.WithProperty(buildParameter.Key, MSBuildHelper.PropertyValue(buildParameter.Value));
				}
			}
		}

		public static void ApplyBuildParameters(this IConfiguration configuration, DotNetCoreMSBuildSettings settings)
		{
			if (!configuration.Has(ConfigurationConstants.BUILD_PARAMETERS_KEY))
			{
				return;
			}

			var buildParameters = configuration.Get<DictionaryOfListConfigurationItem<string, string>>(ConfigurationConstants.BUILD_PARAMETERS_KEY);
			foreach (KeyValuePair<string, List<string>> buildParameter in buildParameters.Values)
			{
				if (buildParameter.Key.ToLowerInvariant() == "configuration")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Configuration can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					settings.SetConfiguration(MSBuildHelper.PropertyValue(buildParameter.Value.Single()));
				}
				else if (buildParameter.Key.ToLowerInvariant() == "platform")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Platform can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					string platform = buildParameter.Value.Single();
					switch (platform.ToLowerInvariant())
					{
						case "any cpu":
						case "anycpu":
							settings.WithProperty("Platform", "Any CPU");
							break;
						case "x86":
							settings.WithProperty("Platform", "x86");
							break;
						case "x64":
							settings.WithProperty("Platform", "x64");
							break;
						case "arm":
							settings.WithProperty("Platform", "ARM");
							break;
						case "win32":
							settings.WithProperty("Platform", "Win32");
							break;
						default:
							settings.WithProperty("Platform", MSBuildHelper.PropertyValue(platform));
							break;
					}
				}
				else
				{
					settings.WithProperty(buildParameter.Key, MSBuildHelper.PropertyValue(buildParameter.Value));
				}
			}
		}

		public static void ApplyBuildParameters(this IConfiguration configuration, string projectOrSolutionFile, DotNetCoreBuildSettings settings)
		{
			configuration.ApplyBuildParametersForDotNetCore(projectOrSolutionFile,
				buildConfiguration => settings.Configuration = buildConfiguration,
				() => settings.MSBuildSettings,
				msBuildSettings => settings.MSBuildSettings = msBuildSettings);
		}

		public static void ApplyBuildParameters(this IConfiguration configuration, string projectOrSolutionFile, DotNetCorePublishSettings settings)
		{
			configuration.ApplyBuildParametersForDotNetCore(projectOrSolutionFile,
				buildConfiguration => settings.Configuration = buildConfiguration,
				() => settings.MSBuildSettings,
				msBuildSettings => settings.MSBuildSettings = msBuildSettings);
		}

		public static void ApplyBuildParameters(this IConfiguration configuration, string projectOrSolutionFile, DotNetCorePackSettings settings)
		{
			configuration.ApplyBuildParametersForDotNetCore(projectOrSolutionFile,
				buildConfiguration => settings.Configuration = buildConfiguration,
				() => settings.MSBuildSettings,
				msBuildSettings => settings.MSBuildSettings = msBuildSettings);
		}

		private static void ApplyBuildParametersForDotNetCore(this IConfiguration configuration, string projectOrSolutionFile, Action<string> setConfiguration, Func<DotNetCoreMSBuildSettings> getSettings, Action<DotNetCoreMSBuildSettings> setSettings)
		{
			if (!configuration.Has(ConfigurationConstants.BUILD_PARAMETERS_KEY))
			{
				return;
			}

			DotNetCoreMSBuildSettings settings = getSettings() ?? new DotNetCoreMSBuildSettings();

			var buildParameters = configuration.Get<DictionaryOfListConfigurationItem<string, string>>(ConfigurationConstants.BUILD_PARAMETERS_KEY);
			foreach (KeyValuePair<string, List<string>> buildParameter in buildParameters.Values)
			{
				if (buildParameter.Key.ToLowerInvariant() == "configuration")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Configuration can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					setConfiguration(MSBuildHelper.PropertyValue(buildParameter.Value.Single()));
				}
				else if (buildParameter.Key.ToLowerInvariant() == "platform")
				{
					if (buildParameter.Value.Count > 1)
					{
						configuration.Context.CakeContext.LogAndThrow($"BuildParameter Platform can only contains one value (currently: {string.Join(" ; ", buildParameter.Value)})");
					}

					string platform = buildParameter.Value.Single();
					switch (platform.ToLowerInvariant())
					{
						case "any cpu":
						case "anycpu":
							if (projectOrSolutionFile.EndsWith("sln"))
							{
								settings.WithProperty("Platform", MSBuildHelper.PropertyValue("Any CPU"));
							}
							else
							{
								settings.WithProperty("Platform", MSBuildHelper.PropertyValue("AnyCPU"));
							}

							break;
						default:
							settings.WithProperty("Platform", MSBuildHelper.PropertyValue(platform));
							break;
					}
				}
				else
				{
					settings.WithProperty(buildParameter.Key, MSBuildHelper.PropertyValue(buildParameter.Value));
				}
			}

			setSettings(settings);
		}
	}
}