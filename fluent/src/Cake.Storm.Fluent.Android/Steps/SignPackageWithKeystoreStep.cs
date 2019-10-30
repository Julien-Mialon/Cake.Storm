using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Commands;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PostReleaseStep]
	internal class SignPackageWithKeystoreStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
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

			JarsignerCommand jarSignerCommand = new JarsignerCommand(configuration.Context.CakeContext);
			Sign(jarSignerCommand, configuration, sourceApkPath, signedApkPath);
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
			configuration.AddSimple(AndroidConstants.ANDROID_ARTIFACT_FILEPATH, resultApkPath);
		}

		private void Sign(JarsignerCommand command, IConfiguration configuration, string sourceApk, string destinationApk)
		{
			string keyStoreFile = configuration.AddRootDirectory(configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_FILE));
			string password = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_PASSWORD);
			string keyAlias = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYALIAS);
			string keyPassword = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYPASSWORD);

			command.SignApk(sourceApk, destinationApk, keyStoreFile, password, keyAlias, keyPassword);
		}
	}
}