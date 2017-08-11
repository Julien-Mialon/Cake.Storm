using System;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Restore;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Steps
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
