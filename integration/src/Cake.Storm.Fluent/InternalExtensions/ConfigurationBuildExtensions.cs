using System.Collections.Generic;
using Cake.Common.Tools.MSBuild;
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
				    settings.SetConfiguration(MSBuildHelper.PropertyValue(buildParameter.Value));
			    }
			    else
			    {
				    settings.WithProperty(buildParameter.Key, MSBuildHelper.PropertyValue(buildParameter.Value));
			    }
		    }
		}
    }
}
