using System.Threading.Tasks;
using Cake.Core;
using Cake.Storm.Fluent.AppCenter.Commands;
using Cake.Storm.Fluent.AppCenter.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.AppCenter.Steps
{
	public abstract class BaseDeployPackageStep : BaseAsyncStep
	{
		protected AppCenterUploader CreateUploader(IConfiguration configuration)
		{
			if (!configuration.TryGetSimple(AppCenterConstants.APPCENTER_TOKEN, out string token))
			{
				configuration.LogAndThrow($"[AppCenter] Missing appcenter token");
			}

			if (!configuration.TryGetSimple(AppCenterConstants.APPCENTER_OWNER_NAME, out string ownerName))
			{
				configuration.LogAndThrow($"[AppCenter] Missing owner name");
			}

			if (!configuration.TryGetSimple(AppCenterConstants.APPCENTER_APPLICATION_NAME, out string applicationName))
			{
				configuration.LogAndThrow($"[AppCenter] Missing application name");
			}

			if (!configuration.TryGetSimple(AppCenterConstants.APPCENTER_DISTRIBUTION_GROUP_NAME, out string distributionGroupName))
			{
				configuration.LogAndThrow($"[AppCenter] Missing distribution group name");
			}

			return new AppCenterUploader(configuration, ownerName, applicationName, token, distributionGroupName);
		}
	}
}