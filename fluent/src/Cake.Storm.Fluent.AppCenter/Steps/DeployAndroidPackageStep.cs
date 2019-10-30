using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.AppCenter.Commands;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.AppCenter.Steps
{
	[DeployStep]
	public class DeployAndroidPackageStep : BaseDeployPackageStep
	{
		protected override async Task ExecuteAsync(IConfiguration configuration, StepType currentStep)
		{
			AppCenterUploader uploader = CreateUploader(configuration);

			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_ARTIFACT_FILEPATH, out string apkPath))
			{
				configuration.LogAndThrow($"Missing path for apk from signing task");
			}

			if (!await uploader.UploadAndroidPackage(apkPath))
			{
				configuration.LogAndThrow($"[AppCenter] can not upload android package {apkPath}");
			}

			configuration.Context.CakeContext.Log.Information($"[AppCenter] Android package {apkPath} has been deployed to AppCenter");
		}
	}
}