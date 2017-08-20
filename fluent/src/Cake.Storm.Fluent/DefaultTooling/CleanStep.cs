using System;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.DefaultTooling
{
	[CleanStep]
    public class CleanStep : IStep
    {
		[Flags]
	    private enum CleanLevel
	    {
		    BinObj = 1,
			Build = 2,
			Artifacts = 4,
			All = 7
	    }

	    public void Execute(IConfiguration configuration)
	    {
		    ICakeContext cake = configuration.Context.CakeContext;
			CleanLevel cleanLevel = CleanLevel.All;
		    if (cake.HasArgument("no-clean"))
		    {
			    string noCleanArgument = cake.Argument<string>("no-clean");
			    string[] splitted = noCleanArgument.ToLowerInvariant().Split('#');
			    foreach (string s in splitted)
			    {
				    switch (s)
				    {
						case "binobj":
							cleanLevel &= ~CleanLevel.BinObj;
							break;
						case "build":
							cleanLevel &= ~CleanLevel.Build;
							break;
						case "artifacts":
							cleanLevel &= ~CleanLevel.Artifacts;
							break;
						case "all":
							cleanLevel &= ~CleanLevel.All;
							break;
						default:
							configuration.Context.CakeContext.LogAndThrow($"no-clean option {s} is not supported, only binobj, build, artifacts and all are valid arguments");
							break;
				    }
			    }
		    }

		    if (cleanLevel.HasFlag(CleanLevel.Artifacts))
		    {
			    DirectoryPath directoryPath = configuration.GetSimple<DirectoryPath>(ConfigurationConstants.ARTIFACTS_PATH_KEY);
				cake.Log.Information($"Clean artifacts {directoryPath.FullPath}");
			    if (cake.DirectoryExists(directoryPath))
			    {
				    IDirectory directory = cake.FileSystem.GetDirectory(directoryPath);
					directory.Delete(true);
			    }
		    }
		    if (cleanLevel.HasFlag(CleanLevel.Build))
		    {
				DirectoryPath directoryPath = configuration.GetSimple<DirectoryPath>(ConfigurationConstants.BUILD_PATH_KEY);
			    cake.Log.Information($"Clean build {directoryPath.FullPath}");
				if (cake.DirectoryExists(directoryPath))
				{
					IDirectory directory = cake.FileSystem.GetDirectory(directoryPath);
					directory.Delete(true);
				}
			}
		    if (cleanLevel.HasFlag(CleanLevel.BinObj))
		    {
				DirectoryPath rootPath = configuration.GetSimple<DirectoryPath>(ConfigurationConstants.ROOT_PATH_KEY);
			    cake.Log.Information($"Clean bin and obj in {rootPath.FullPath}");
				cake.DeleteDirectories(cake.GetDirectories(Path.Combine(rootPath.FullPath, "**/bin")), true);
			    cake.DeleteDirectories(cake.GetDirectories(Path.Combine(rootPath.FullPath, "**/obj")), true);
			}
	    }
    }
}
