using System;
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
			if (!context.FileSystem.Exist(manifestFile))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Manifest file not found {manifestFile.FullPath}");
				throw new CakeException($"Manifest file not found {manifestFile.FullPath}");
			}

			return new AndroidManifest(context, manifestFile.FullPath);
		}

		[CakeMethodAlias]
		public static Keystore LoadKeystore(this ICakeContext context, FilePath keystoreFile)
		{
			if (!context.FileSystem.Exist(keystoreFile))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Keystore file not found {keystoreFile.FullPath}");
				throw new CakeException($"Keystore file not found {keystoreFile.FullPath}");
			}

			return new Keystore(context, keystoreFile);
		}

		[CakeMethodAlias]
		public static Keystore CreateKeystore(this ICakeContext context, FilePath keystoreFile, string keystorePassword, string keyAlias, string keyPassword, string authority)
		{
			if (context.FileSystem.Exist(keystoreFile))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"Keystore file already exists {keystoreFile.FullPath}");
				throw new CakeException($"Keystore file already exists {keystoreFile.FullPath}");
			}

			Keystore keystore = new Keystore(context, keystoreFile);

			keystore.CreateAlias(keystorePassword, keyAlias, keyPassword, authority);

			return keystore;
		}
	}
}
