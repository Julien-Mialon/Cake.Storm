using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.iOS.Steps;

[PreBuildStep]
internal class DotNetiOSRestoreProject : IStep
{
	public void Execute(IConfiguration configuration, StepType currentStep)
	{
		if (configuration.TryGetSimple(iOSConstants.DISABLE_NET_IOS_DOTNET_RESTORE, out bool disableRestore) && disableRestore)
		{
			return;
		}

		string[] projectsPath = configuration.GetProjectsPath();

		if (projectsPath.Length > 1)
		{
			configuration.Context.CakeContext.LogAndThrow($"Only one project supported");
		}

		string projectFile = projectsPath[0];
		configuration.FileExistsOrThrow(projectFile);

		configuration.Context.CakeContext.DotNetCoreRestore(projectFile, new()
		{
			Runtime = "ios-arm64"
		});
	}
}