using Cake.Storm.Fluent.AppCenter.Common;
using Cake.Storm.Fluent.AppCenter.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.AppCenter.Extensions
{
	public static class AppCenterConfigurationExtensions
	{
		public static IAppCenterConfiguration WithToken(this IAppCenterConfiguration configuration, string token)
		{
			configuration.Configuration.AddSimple(AppCenterConstants.APPCENTER_TOKEN, token);
			return configuration;
		}

		public static IAppCenterConfiguration WithOwner(this IAppCenterConfiguration configuration, string ownerName)
		{
			configuration.Configuration.AddSimple(AppCenterConstants.APPCENTER_OWNER_NAME, ownerName);
			return configuration;
		}

		public static IAppCenterConfiguration WithApplication(this IAppCenterConfiguration configuration, string applicationName)
		{
			configuration.Configuration.AddSimple(AppCenterConstants.APPCENTER_APPLICATION_NAME, applicationName);
			return configuration;
		}

		public static IAppCenterConfiguration WithDistributionGroup(this IAppCenterConfiguration configuration, string distributionGroupName)
		{
			configuration.Configuration.AddSimple(AppCenterConstants.APPCENTER_DISTRIBUTION_GROUP_NAME, distributionGroupName);
			return configuration;
		}
	}
}