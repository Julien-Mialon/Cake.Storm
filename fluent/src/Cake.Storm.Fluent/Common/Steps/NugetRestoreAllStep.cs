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
	public class NugetRestoreAllStep : ICacheableStep
	{
		private readonly string _directoryPath;

		public NugetRestoreAllStep(string directoryPath = null)
		{
			_directoryPath = directoryPath;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (configuration.DisableNugetRestore())
			{
				return;
			}

			List<FilePath> solutions = GetSolutionsPaths(configuration);

			configuration.Context.CakeContext.NuGetRestore(solutions);
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			List<FilePath> solutions = GetSolutionsPaths(configuration);

			return string.Join(";", solutions.Select(x => x.FullPath));
		}

		private List<FilePath> GetSolutionsPaths(IConfiguration configuration)
		{
			string directory = _directoryPath ?? configuration.AddRootDirectory(".");

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