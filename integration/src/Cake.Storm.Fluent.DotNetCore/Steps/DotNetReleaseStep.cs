using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.DotNetCore.Steps
{
	[ReleaseStep]
	internal class DotNetReleaseStep : IStep
	{
		public void Execute(IConfiguration configuration)
		{
			string projectPath = configuration.GetProjectPath();
			if (!configuration.Context.CakeContext.FileExists(projectPath))
			{
				configuration.Context.CakeContext.LogAndThrow($"Project file {projectPath} does not exists");
			}
			
			configuration.Context.CakeContext.DotNetCorePublish(projectPath, new DotNetCorePublishSettings
			{
				OutputDirectory = configuration.GetArtifactsPath(),
				
			});
		}
	}
}