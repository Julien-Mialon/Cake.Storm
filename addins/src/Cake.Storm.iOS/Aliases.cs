using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Common.Tools;

namespace Cake.Storm.iOS
{
	[CakeAliasCategory("Cake.Storm.iOS")]
	public static class Aliases
	{
		[CakeMethodAlias]
		public static PList LoadPListFile(this ICakeContext context, FilePath file)
		{
			if (!context.FileSystem.Exist(file))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"PList file {file} does not exists");
				throw new CakeException($"PList file {file} does not exists");
			}

			return new PList(context, file.MakeAbsolute(context.Environment).FullPath);
		}

		[CakeMethodAlias]
		public static void CreateIpaFile(this ICakeContext context, FilePath projectFile, DirectoryPath outputDirectory, Action<DotNetBuildSettings> configurator = null)
		{
			context.DotNetBuild(projectFile.MakeAbsolute(context.Environment), configuration =>
			{
				configuration.Configuration = "Release";
				configuration.Targets.Add("Build");

				configuration.WithProperty("BuildIpa", "true")
							 .WithProperty("IpaIncludeArtwork", "false")
							 .WithProperty("IpaPackageDir", outputDirectory.MakeAbsolute(context.Environment).FullPath)
				             .WithProperty("Platform", "iPhone");

				configurator?.Invoke(configuration);
			});
		}
	}
}
