using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.DotNetCore.Steps
{
	[PreBuildStep]
	internal class DotNetRestoreStep : ICacheableStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (configuration.DisableNugetRestore())
			{
				return;
			}

			string solutionPath = configuration.GetSolutionPath();
			if (!configuration.Context.CakeContext.FileExists(solutionPath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution file {solutionPath} does not exists");
			}

			configuration.Context.CakeContext.DotNetCoreRestore(solutionPath);
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			return configuration.GetSolutionPath();
		}
	}
}
