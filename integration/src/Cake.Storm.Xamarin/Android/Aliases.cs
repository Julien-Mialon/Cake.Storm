using System;
using System.IO;
using System.Linq;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Storm.Xamarin.Android.Commands;
using Cake.Storm.Xamarin.Android.Models;

namespace Cake.Storm.Xamarin.Android
{
	[CakeAliasCategory("Cake.Storm.Xamarin.Android")]
	[CakeNamespaceImport("Cake.Storm.Xamarin.Android.Models")]
	public static class Aliases
    {
	    [CakeMethodAlias]
	    public static AndroidManifest LoadAndroidManifest(this ICakeContext context, FilePath manifestFile)
	    {
		    if (!context.FileSystem.Exist(manifestFile))
		    {
				context.LogAndThrow($"Manifest file not found {manifestFile.FullPath}");
		    }

		    return new AndroidManifest(context, manifestFile.FullPath);
	    }

	    [CakeMethodAlias]
	    public static Keystore LoadKeystore(this ICakeContext context, FilePath keystoreFile)
	    {
		    if (!context.FileSystem.Exist(keystoreFile))
		    {
			    context.LogAndThrow($"Keystore file not found {keystoreFile.FullPath}");
		    }

		    return new Keystore(context, keystoreFile);
	    }

	    [CakeMethodAlias]
	    public static Keystore CreateKeystore(this ICakeContext context, FilePath keystoreFile, string keystorePassword, string keyAlias, string keyPassword, string authority)
	    {
		    if (context.FileSystem.Exist(keystoreFile))
		    {
			    context.LogAndThrow($"Keystore file already exists {keystoreFile.FullPath}");
		    }

		    Keystore keystore = new Keystore(context, keystoreFile);
		    keystore.CreateAlias(keystorePassword, keyAlias, keyPassword, authority);
		    return keystore;
	    }

	    [CakeMethodAlias]
	    public static void SignApk(this ICakeContext context, FilePath inputApk, FilePath outputApk, FilePath keystoreFile, string keystorePassword, string alias, string aliasPassword)
	    {
		    JarsignerCommand command = new JarsignerCommand(context);

		    command.SignApk(inputApk, outputApk, keystoreFile, keystorePassword, alias, aliasPassword);
	    }

	    [CakeMethodAlias]
	    public static void VerifyApk(this ICakeContext context, FilePath apkFile)
	    {
		    JarsignerCommand command = new JarsignerCommand(context);

		    command.VerifyApk(apkFile);
	    }

	    [CakeMethodAlias]
	    public static void AlignApk(this ICakeContext context, FilePath inputApk, FilePath outputApk)
	    {
		    ZipAlignCommand command = new ZipAlignCommand(context);

		    command.Align(inputApk, outputApk);
	    }

	    [CakeMethodAlias]
	    public static FilePath PackageForAndroid(this ICakeContext context, FilePath projectFile, AndroidManifest manifest, Action<MSBuildSettings> configurator = null)
	    {
		    if (!context.FileSystem.Exist(projectFile))
		    {
				context.LogAndThrow($"Project File Not Found: {projectFile.FullPath}");
		    }

		    context.MSBuild(projectFile, configuration =>
		    {
			    configuration.Configuration = "Release";
			    configuration.Targets.Add("PackageForAndroid");

			    configurator?.Invoke(configuration);
		    });

		    string searchPattern = projectFile.GetDirectory() + "/**/" + manifest.Package + ".apk";
		    // Use the globber to find any .apk files within the tree
		    return context.Globber
			    .GetFiles(searchPattern)
			    .OrderBy(f => new FileInfo(f.FullPath).LastWriteTimeUtc)
			    .FirstOrDefault();
	    }
	}
}
