using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Internals
{
	public class BuilderParameters
    {
	    public BuilderParameters(DirectoryPath rootPath, DirectoryPath buildPath, DirectoryPath artifactsPath, IFluentContext context, IConfiguration rootConfiguration, IReadOnlyDictionary<string, IPlatformConfiguration> platformsConfiguration, IReadOnlyDictionary<string, ITargetConfiguration> targetsConfiguration, IReadOnlyDictionary<string, IApplicationConfiguration> applicationsConfiguration)
	    {
		    RootPath = rootPath;
		    BuildPath = buildPath;
		    ArtifactsPath = artifactsPath;
		    Context = context;
		    RootConfiguration = rootConfiguration;
		    PlatformsConfiguration = platformsConfiguration;
		    TargetsConfiguration = targetsConfiguration;
		    ApplicationsConfiguration = applicationsConfiguration;
	    }

	    public DirectoryPath RootPath { get; }

		public DirectoryPath BuildPath { get; }

		public DirectoryPath ArtifactsPath { get; }

		public IFluentContext Context { get; }

		public IConfiguration RootConfiguration { get; }

		public IReadOnlyDictionary<string, IPlatformConfiguration> PlatformsConfiguration { get; }

		public IReadOnlyDictionary<string, ITargetConfiguration> TargetsConfiguration { get; }

		public IReadOnlyDictionary<string, IApplicationConfiguration> ApplicationsConfiguration { get; }
    }
}
