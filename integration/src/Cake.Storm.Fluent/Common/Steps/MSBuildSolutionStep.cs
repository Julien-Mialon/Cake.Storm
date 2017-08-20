using Cake.Common.Tools.MSBuild;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Common.Steps
{
	[BuildStep]
	public class MSBuildSolutionStep : IStep
	{
		public void Execute(IConfiguration configuration)
		{
			string solutionPath = configuration.GetSolutionPath();
			configuration.FileExistsOrThrow(solutionPath);

			configuration.Context.CakeContext.MSBuild(solutionPath, configuration.ApplyBuildParameters);
		}
	}
}