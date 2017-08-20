using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Internals
{
    public class Builder
    {
	    private readonly BuilderParameters _parameters;

	    public Builder(BuilderParameters parameters)
	    {
		    _parameters = parameters;
	    }

	    public void Build()
	    {
			//set path to rootConfiguration
			_parameters.RootConfiguration.Add(ConfigurationConstants.ROOT_PATH_KEY, new SimpleConfigurationItem<DirectoryPath>(_parameters.RootPath));
		    _parameters.RootConfiguration.Add(ConfigurationConstants.BUILD_PATH_KEY, new SimpleConfigurationItem<DirectoryPath>(_parameters.BuildPath));
		    _parameters.RootConfiguration.Add(ConfigurationConstants.ARTIFACTS_PATH_KEY, new SimpleConfigurationItem<DirectoryPath>(_parameters.ArtifactsPath));
			
			List<IBuilder> builders = new List<IBuilder>
			{
				new CleanTaskBuilder(_parameters),
				new BuildTaskBuilder(_parameters),
			};

		    foreach (var builder in builders)
		    {
			    builder.Build();
		    }

		    _parameters.Context.Task("help").Does(() =>
		    {
			    _parameters.Context.CakeContext.Log.Information("");
			    _parameters.Context.CakeContext.Log.Information("List of all available targets:");
			    _parameters.Context.CakeContext.Log.Information("");
				foreach (var builder in builders)
			    {
				    builder.Help();
			    }
		    });
		    _parameters.Context.Task("default").IsDependentOn("help").Does(() => { });
	    }
    }
}
