using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Storm.Fluent.DotNetCore.Common;
using Cake.Storm.Fluent.DotNetCore.Models;
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
			DotNetRestoreStrategy restoreStrategy = RestoreStrategy(configuration);

			switch (restoreStrategy)
			{
				case DotNetRestoreStrategy.Solution:
					configuration.Context.CakeContext.DotNetCoreRestore(GetSolutionPath(configuration));
					break;
				case DotNetRestoreStrategy.AllSolutions:
					configuration.Context.CakeContext.DotNetCoreRestore(configuration.RootDirectory());
					break;
				case DotNetRestoreStrategy.None:
				default:
					break;
			}
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			return RestoreStrategy(configuration) switch
			{
				DotNetRestoreStrategy.None => "<none>",
				DotNetRestoreStrategy.Solution => configuration.GetSolutionPath(),
				DotNetRestoreStrategy.AllSolutions => "<all>",
				_ => "<undefined>"
			};
		}

		private DotNetRestoreStrategy RestoreStrategy(IConfiguration configuration)
		{
			if (configuration.TryGetSimple(DotNetCoreConstants.DOTNETCORE_RESTORE_STRATEGY_KEY, out DotNetRestoreStrategy target))
			{
				return target;
			}

			return DotNetRestoreStrategy.AllSolutions;
		}

		private string GetSolutionPath(IConfiguration configuration)
		{
			string toRestorePath = configuration.GetSolutionPath();

			if (!configuration.Context.CakeContext.FileExists(toRestorePath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution or project file {toRestorePath} does not exists");
			}

			return toRestorePath;
		}
	}
}
