using System.Collections.Generic;
using System.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	[PreBuildStep]
	public class NugetRestoreStep : ICacheableStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			NugetRestoreStrategy restoreStrategy = RestoreStrategy(configuration);

			switch (restoreStrategy)
			{
				case NugetRestoreStrategy.Solution:
					configuration.Context.CakeContext.NuGetRestore(GetSolutionPath(configuration));
					break;
				case NugetRestoreStrategy.AllSolutions:
					configuration.Context.CakeContext.NuGetRestore(GetSolutionsPaths(configuration));
					break;
				case NugetRestoreStrategy.None:
				default:
					break;
			}
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			return RestoreStrategy(configuration) switch
			{
				NugetRestoreStrategy.None => "<none>",
				NugetRestoreStrategy.Solution => configuration.GetSolutionPath(),
				NugetRestoreStrategy.AllSolutions => "<all>",
				_ => "<undefined>"
			};
		}

		private NugetRestoreStrategy RestoreStrategy(IConfiguration configuration)
		{
			if (configuration.TryGetSimple(ConfigurationConstants.NUGET_RESTORE_STRATEGY_KEY, out NugetRestoreStrategy target))
			{
				return target;
			}

			return NugetRestoreStrategy.AllSolutions;
		}

		private FilePath GetSolutionPath(IConfiguration configuration)
		{
			string toRestorePath = configuration.GetSolutionPath();

			if (!configuration.Context.CakeContext.FileExists(toRestorePath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution or project file {toRestorePath} does not exists");
			}

			return toRestorePath;
		}

		private List<FilePath> GetSolutionsPaths(IConfiguration configuration)
		{
			string directory = configuration.AddRootDirectory(".");

			if (!configuration.Context.CakeContext.DirectoryExists(directory))
			{
				configuration.Context.CakeContext.LogAndThrow($"Directory {directory} does not exists");
			}

			List<FilePath> solutions = configuration.Context.CakeContext.Globber.GetFiles(System.IO.Path.Combine(directory, "**", "*.sln")).ToList();

			if (solutions.Count == 0)
			{
				configuration.Context.CakeContext.LogAndThrow($"No solution file found in directory {directory}");
			}

			return solutions;
		}
	}
}