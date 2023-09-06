using System.Linq;
using Cake.Common.Tools.MSBuild;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	[BuildStep]
	public class MSBuildSolutionStep : ICacheableStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			string solutionPath = configuration.GetSolutionPath();
			configuration.FileExistsOrThrow(solutionPath);

			configuration.Context.CakeContext.MSBuild(solutionPath, configuration.ApplyBuildParameters);
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			string solutionPath = configuration.GetSolutionPath();

			MSBuildSettings settings = new MSBuildSettings();
			configuration.ApplyBuildParameters(settings);

			return $"{solutionPath}_{settings.Configuration}_{settings.MSBuildPlatform}_{settings.PlatformTarget}_{settings.ToolPath?.FullPath}_{settings.WorkingDirectory?.FullPath}"
			       + $"{settings.ToolVersion}_{string.Join(";", settings.Targets)}_{string.Join(";", settings.Properties.Select(x => $"{x.Key}={string.Join(",", x.Value)}"))}";
		}
	}
}