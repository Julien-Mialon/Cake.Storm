using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	[PreBuildStep]
	public class NugetRestoreStep : IStep
	{
		private readonly string _solutionOrProjectPath;

		public NugetRestoreStep(string solutionOrProjectPath)
		{
			_solutionOrProjectPath = solutionOrProjectPath;
		}

		public void Execute(IConfiguration configuration)
		{
			if (!configuration.Context.CakeContext.FileExists(_solutionOrProjectPath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Solution or project file {_solutionOrProjectPath} does not exists");
			}

			configuration.Context.CakeContext.NuGetRestore(_solutionOrProjectPath);
		}
	}
}
