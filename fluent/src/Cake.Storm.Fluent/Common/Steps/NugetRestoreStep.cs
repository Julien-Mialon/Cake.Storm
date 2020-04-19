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

		public NugetRestoreStep(string solutionOrProjectPath = null)
		{
			_solutionOrProjectPath = solutionOrProjectPath;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			string toRestorePath = _solutionOrProjectPath;

			if (string.IsNullOrEmpty(toRestorePath))
			{
				toRestorePath = configuration.GetSolutionPath();
			}

			if (!configuration.Context.CakeContext.FileExists(toRestorePath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution or project file {toRestorePath} does not exists");
			}

			configuration.Context.CakeContext.NuGetRestore(toRestorePath);
		}

		public string GetCacheId(IConfiguration configuration, StepType currentStep)
		{
			return _solutionOrProjectPath;
		}
	}
}