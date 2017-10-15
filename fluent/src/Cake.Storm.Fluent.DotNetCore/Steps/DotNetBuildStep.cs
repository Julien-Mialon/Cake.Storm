using System.Linq;
using System.Text;
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
    internal class DotNetBuildStep : ICacheableStep
    {
	    public void Execute(IConfiguration configuration, StepType currentStep)
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

	    public string GetCacheId(IConfiguration configuration, StepType currentStep)
	    {
		    string solutionPath = configuration.GetSolutionPath();
		    StringBuilder builder = new StringBuilder();

		    builder.Append($"{solutionPath} ");
		    
		    configuration.RunOnConfiguredTargetFramework(framework =>
		    {
			    builder.Append($"{framework}=(");

			    DotNetCoreBuildSettings settings = new DotNetCoreBuildSettings();
			    configuration.ApplyBuildParameters(solutionPath, settings);
			    
			    builder.Append($"{settings.Configuration}_{settings.Runtime}_{settings.OutputDirectory?.FullPath}_{settings.ToolPath?.FullPath}_{settings.WorkingDirectory?.FullPath}"
			           + $"{settings.MSBuildSettings.ToolVersion}_{string.Join(";", settings.MSBuildSettings.Targets)}_{string.Join(";", settings.MSBuildSettings.Properties.Select(x => $"{x.Key}={string.Join(",", x.Value)}"))}");
			    
			    builder.Append(")");
		    });

		    return builder.ToString();
	    }
    }
}
