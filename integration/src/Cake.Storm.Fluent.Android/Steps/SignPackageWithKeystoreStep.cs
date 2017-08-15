using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Commands;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PostReleaseStep]
	internal class SignPackageWithKeystoreStep : IStep
	{
		private readonly string _keyStoreFile;
		private readonly IKeystoreAction _keyStore;

		public SignPackageWithKeystoreStep(string keyStoreFile, IKeystoreAction keyStore)
		{
			_keyStoreFile = keyStoreFile;
			_keyStore = keyStore;
		}

		public void Execute(IConfiguration configuration)
		{
			FilePath apkPath = configuration.GetSimple<string>(AndroidConstants.GENERATED_ANDROID_PACKAGE_PATH_KEY);

			string buildPath = configuration.GetBuildPath().FullPath;
			string artifactsPath = configuration.GetArtifactsPath().FullPath;

			string apkName = apkPath.GetFilename().ToString();
			string apkNameWithoutExtension = apkPath.GetFilenameWithoutExtension().ToString();
			string apkExtension = apkPath.GetExtension();

			string sourceApkPath = Path.Combine(buildPath, apkName);
			configuration.Context.CakeContext.CopyFile(apkPath, sourceApkPath);

			string signedApkPath = Path.Combine(buildPath, $"{apkNameWithoutExtension}-Signed{apkExtension}");
			_keyStore.Sign(configuration.AddRootDirectory(_keyStoreFile), configuration, sourceApkPath, signedApkPath);
			JarsignerCommand jarSignerCommand = new JarsignerCommand(configuration.Context.CakeContext);
			jarSignerCommand.VerifyApk(signedApkPath);

			string alignedApkPath = Path.Combine(buildPath, $"{apkNameWithoutExtension}-Aligned{apkExtension}");
			ZipAlignCommand zipAlignCommand = new ZipAlignCommand(configuration.Context.CakeContext);
			zipAlignCommand.Align(signedApkPath, alignedApkPath);

			string resultApkPath = Path.Combine(artifactsPath, apkName);
			if (configuration.Context.CakeContext.FileExists(resultApkPath))
			{
				configuration.Context.CakeContext.DeleteFile(resultApkPath);
			}
			
			configuration.Context.CakeContext.CopyFile(alignedApkPath, resultApkPath);
		}
	}
}