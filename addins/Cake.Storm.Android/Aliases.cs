using System.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Storm.Android
{
	[CakeAliasCategory("Cake.Storm.Android")]
	public static class Aliases
	{
		[CakeMethodAlias]
		public static AndroidManifest LoadAndroidManifest(this ICakeContext context, FilePath manifestFile)
		{
			if (!File.Exists(manifestFile.FullPath))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Manifest file not found {manifestFile.FullPath}");
				throw new CakeException($"Manifest file not found {manifestFile.FullPath}");
			}

			return new AndroidManifest(context, manifestFile.FullPath);
		}
	}
}
