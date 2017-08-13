using System.Collections.Generic;
using System.Linq;
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
				else
				{
					settings.WithProperty(buildParameter.Key, MSBuildHelper.PropertyValue(buildParameter.Value));
				}
			}
		}
	}
}