using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.AppCenter.Commands;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.AppCenter.Steps
{
	[DeployStep]
	public class DeployiOSPackageStep : BaseDeployPackageStep
	{
		protected override async Task ExecuteAsync(IConfiguration configuration, StepType currentStep)
		{
			AppCenterUploader uploader = CreateUploader(configuration);

			if (!configuration.TryGetSimple(iOSConstants.IOS_ARTIFACT_PACKAGE_FILEPATH, out string ipaPath))
			{
				configuration.LogAndThrow($"Missing path for ipa from signing task");
			}

			if (!configuration.TryGetSimple(iOSConstants.IOS_ARTIFACT_SYMBOLS_FILEPATH, out string symbolsPath))
			{
				configuration.LogAndThrow($"Missing path for symbols from signing task");
			}

			if (!await uploader.UploadiOSPackage(ipaPath, symbolsPath))
			{
				configuration.LogAndThrow($"[AppCenter] can not upload iOS package {ipaPath} with symbols {symbolsPath}");
			}

			configuration.Context.CakeContext.Log.Information($"[AppCenter] iOS package {ipaPath} has been deployed to AppCenter with symbols {symbolsPath}");
		}
	}
}