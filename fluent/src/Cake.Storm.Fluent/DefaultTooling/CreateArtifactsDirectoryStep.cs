using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.DefaultTooling
{
	[PreReleaseStep]
    public class CreateArtifactsDirectoryStep : IStep
    {
	    public void Execute(IConfiguration configuration)
	    {
		    DirectoryPath directory = configuration.GetSimple<DirectoryPath>(ConfigurationConstants.ARTIFACTS_PATH_KEY);
		    configuration.Context.CakeContext.EnsureDirectoryExists(directory);
	    }
    }
}
