using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.iOS.Extensions
{
	public static class FastlaneCertificateConfigurationExtensions
	{
		public static IFastlaneCertificateConfiguration WithUserName(this IFastlaneCertificateConfiguration configuration, string userName)
		{
			configuration.Configuration.AddSimple(iOSConstants.FASTLANE_APPLE_USERNAME, userName);
			return configuration;
		}
		
		public static IFastlaneCertificateConfiguration WithTeamName(this IFastlaneCertificateConfiguration configuration, string teamName)
		{
			configuration.Configuration.AddSimple(iOSConstants.FASTLANE_APPLE_TEAMNAME, teamName);
			return configuration;
		}
		
		public static IFastlaneCertificateConfiguration WithProvisioningProfileName(this IFastlaneCertificateConfiguration configuration, string provisioningProfileName)
		{
			configuration.Configuration.AddSimple(iOSConstants.FASTLANE_SIGH_PROVISIONING_NAME, provisioningProfileName);
			return configuration;
		}
		
		public static IFastlaneCertificateConfiguration WithCertificateType(this IFastlaneCertificateConfiguration configuration, CertificateType certificateType)
		{
			configuration.Configuration.AddSimple(iOSConstants.FASTLANE_SIGH_CERTIFICATE_TYPE, certificateType);
			return configuration;
		}
	}
}