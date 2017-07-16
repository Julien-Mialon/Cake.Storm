using System.Collections.Generic;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.DefaultTooling;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Extensions
{
    public static class ConfigurationExtensions
    {
	    public static TConfiguration WithSolution<TConfiguration>(this TConfiguration configuration, string solutionPath)
			where TConfiguration : IConfiguration
	    {
			configuration.Add(ConfigurationConstants.SOLUTION_KEY, new SimpleConfigurationItem<string>(solutionPath));
		    return configuration;
	    }

	    public static TConfiguration WithBuildParameter<TConfiguration>(this TConfiguration configuration, string key, string value)
		    where TConfiguration : IConfiguration
		{
		    if (configuration.TryGet<DictionaryOfListConfigurationItem<string, string>>(ConfigurationConstants.BUILD_PARAMETERS_KEY, out var configurationItem))
		    {
			    if(configurationItem.Values.TryGetValue(key, out var values))
			    {
				    values.Add(value);
			    }
			    else
			    {
				    configurationItem.Values.Add(key, new List<string>
				    {
					    value
				    });
			    }
		    }
		    else
		    {
			    configuration.Add(ConfigurationConstants.BUILD_PARAMETERS_KEY, new DictionaryOfListConfigurationItem<string, string>(key, value));
		    }
		    return configuration;
	    }

	    public static TConfiguration WithProject<TConfiguration>(this TConfiguration configuration, string projectPath)
		    where TConfiguration : IConfiguration
		{
			configuration.Add(ConfigurationConstants.PROJECT_KEY, new SimpleConfigurationItem<string>(projectPath));
		    return configuration;
	    }

	    public static TConfiguration WithVersion<TConfiguration>(this TConfiguration configuration, string version)
		    where TConfiguration : IConfiguration
	    {
		    configuration.Add(ConfigurationConstants.VERSION_KEY, new SimpleConfigurationItem<string>(version));
		    return configuration;
	    }

		public static TConfiguration UseDefaultTooling<TConfiguration>(this TConfiguration configuration) where TConfiguration : IConfiguration
	    {
		    configuration.AddStep(new CleanStep());
			configuration.AddStep(new CreateBuildDirectoryStep());
			configuration.AddStep(new CreateArtifactsDirectoryStep());
		    return configuration;
	    }
    }
}
