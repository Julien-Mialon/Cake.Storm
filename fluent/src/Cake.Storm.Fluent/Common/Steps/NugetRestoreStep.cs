using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	[PreBuildStep]
	public class NugetRestoreStep : ICacheableStep
	{
		private readonly string _solutionOrProjectPath;

		public NugetRestoreStep(string solutionOrProjectPath)
		{
			_solutionOrProjectPath = solutionOrProjectPath;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (configuration.DisableNugetRestore())
			{
				return;
			}

			if (!configuration.Context.CakeContext.FileExists(_solutionOrProjectPath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution or project file {_solutionOrProjectPath} does not exists");
			}

			configuration.Context.CakeContext.NuGetRestore(_solutionOrProjectPath);
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			return _solutionOrProjectPath;
		}
	}
}
