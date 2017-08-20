using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Core;
using Cake.Storm.Fluent.DotNetCore.InternalExtensions;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.DotNetCore.Steps
{
	[BuildStep]
    internal class DotNetBuildStep : IStep
    {
	    public void Execute(IConfiguration configuration)
	    {
		    string solutionPath = configuration.GetSolutionPath();
		    if (!configuration.Context.CakeContext.FileExists(solutionPath))
		    {
			    configuration.Context.CakeContext.LogAndThrow($"Solution file {solutionPath} does not exists");
		    }
		    
		    configuration.RunOnConfiguredTargetFramework(framework =>
		    {
			    DotNetCoreBuildSettings settings = new DotNetCoreBuildSettings
			    {
				    Framework = framework
			    };
			    configuration.ApplyBuildParameters(solutionPath, settings);
			    configuration.Context.CakeContext.DotNetCoreBuild(solutionPath, settings);
		    });
	    }
    }
}
