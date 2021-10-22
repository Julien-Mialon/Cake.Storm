using Cake.Common.IO;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Commands;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Android.Extensions;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.Android.Steps
{
	internal class BaseSignWithKeystoreStep
	{
		protected string SignPackage(IConfiguration configuration, FilePath packagePath, bool align)
		{
			string buildPath = configuration.GetBuildPath().FullPath;
			string artifactsPath = configuration.GetArtifactsPath().FullPath;

			string packageName = packagePath.GetFilename().ToString();
			string packageNameWithoutExtension = packagePath.GetFilenameWithoutExtension().ToString();
			string packageExtension = packagePath.GetExtension();

			string sourcePackagePath = Path.Combine(buildPath, packageName);
			configuration.Context.CakeContext.CopyFile(packagePath, sourcePackagePath);

			bool forceUseJarsigner = configuration.IsJarsignerForced();
			string buildOutputPackagePath = forceUseJarsigner
				? SignWithJarSigner(configuration, align, sourcePackagePath, buildPath, packageNameWithoutExtension, packageExtension)
				: SignWithApkSigner(configuration, align, sourcePackagePath, buildPath, packageNameWithoutExtension, packageExtension);

			string resultPackagePath = Path.Combine(artifactsPath, packageName);
			if (configuration.Context.CakeContext.FileExists(resultPackagePath))
			{
				configuration.Context.CakeContext.DeleteFile(resultPackagePath);
			}

			configuration.Context.CakeContext.CopyFile(buildOutputPackagePath, resultPackagePath);
			return resultPackagePath;
		}

		private static string SignWithJarSigner(IConfiguration configuration, bool align, string sourcePackagePath, string buildPath, string packageNameWithoutExtension, string packageExtension)
		{
			string signedPackagePath = Path.Combine(buildPath, $"{packageNameWithoutExtension}-Signed{packageExtension}");

			JarsignerCommand jarSignerCommand = new JarsignerCommand(configuration.Context.CakeContext);
			Sign(jarSignerCommand, configuration, sourcePackagePath, signedPackagePath);
			jarSignerCommand.VerifyApk(signedPackagePath);

			string buildOutputPackagePath = signedPackagePath;
			if (align)
			{
				string alignedPackagePath = Path.Combine(buildPath, $"{packageNameWithoutExtension}-Aligned{packageExtension}");
				ZipAlignCommand zipAlignCommand = new ZipAlignCommand(configuration.Context.CakeContext);
				zipAlignCommand.Align(signedPackagePath, alignedPackagePath);

				buildOutputPackagePath = alignedPackagePath;
			}

			return buildOutputPackagePath;

			static void Sign(JarsignerCommand command, IConfiguration configuration, string sourceApk, string destinationApk)
			{
				string keyStoreFile = configuration.AddRootDirectory(configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_FILE));
				string password = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_PASSWORD);
				string keyAlias = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYALIAS);
				string keyPassword = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYPASSWORD);

				command.SignApk(sourceApk, destinationApk, keyStoreFile, password, keyAlias, keyPassword);
			}
		}

		private static string SignWithApkSigner(IConfiguration configuration, bool align, string sourcePackagePath, string buildPath, string packageNameWithoutExtension, string packageExtension)
		{
			string buildOutputPackagePath = sourcePackagePath;
			if (align)
			{
				string alignedPackagePath = Path.Combine(buildPath, $"{packageNameWithoutExtension}-Aligned{packageExtension}");
				ZipAlignCommand zipAlignCommand = new ZipAlignCommand(configuration.Context.CakeContext);
				zipAlignCommand.Align(sourcePackagePath, alignedPackagePath);

				buildOutputPackagePath = alignedPackagePath;
			}

			string signedPackagePath = Path.Combine(buildPath, $"{packageNameWithoutExtension}-Signed{packageExtension}");

			ApksignerCommand apkSignerCommand = new ApksignerCommand(configuration.Context.CakeContext);
			Sign(apkSignerCommand, configuration, buildOutputPackagePath, signedPackagePath);
			apkSignerCommand.VerifyApk(signedPackagePath);

			buildOutputPackagePath = signedPackagePath;
			return buildOutputPackagePath;

			static void Sign(ApksignerCommand command, IConfiguration configuration, string sourceApk, string destinationApk)
			{
				string keyStoreFile = configuration.AddRootDirectory(configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_FILE));
				string password = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_PASSWORD);
				string keyAlias = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYALIAS);
				string keyPassword = configuration.GetSimple<string>(AndroidConstants.ANDROID_KEYSTORE_KEYPASSWORD);

				command.SignApk(sourceApk, destinationApk, keyStoreFile, password, keyAlias, keyPassword);
			}
		}
	}

	[PostReleaseStep]
	internal class SignApkWithKeystoreStep : BaseSignWithKeystoreStep, IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (!configuration.IsAndroidApkEnabled())
			{
				return;
			}

			FilePath apkPath = configuration.GetSimple<string>(AndroidConstants.GENERATED_ANDROID_PACKAGE_PATH_KEY);

			string resultApkPath = SignPackage(configuration, apkPath, align: true);
			configuration.AddSimple(AndroidConstants.GENERATED_ANDROID_PACKAGE_ARTIFACT_FILEPATH, resultApkPath);
		}
	}

	[PostReleaseStep]
	internal class SignAabWithKeystoreStep : BaseSignWithKeystoreStep, IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (!configuration.IsAndroidAppBundleEnabled())
			{
				return;
			}

			FilePath packagePath = configuration.GetSimple<string>(AndroidConstants.GENERATED_ANDROID_APP_BUNDLE_PATH_KEY);

			string resultPath = SignPackage(configuration, packagePath, align: false);
			configuration.AddSimple(AndroidConstants.GENERATED_ANDROID_APP_BUNDLE_ARTIFACT_FILEPATH, resultPath);
		}
	}
}