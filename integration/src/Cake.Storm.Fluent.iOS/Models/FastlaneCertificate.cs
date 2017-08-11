using System;
using System.IO;
using Cake.Common.IO;
using Cake.Core;
using Cake.Storm.Fluent.iOS.Commands;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.iOS.Models
{
	internal class FastlaneCertificate : IFastlaneCertificate
	{
		public string UserName { get; private set; }
		public string TeamName { get; private set; }
		public string ProvisioningProfileName { get; private set; }
		public CertificateType CertificateType { get; private set; }

		public IFastlaneCertificate WithUserName(string userName)
		{
			UserName = userName;
			return this;
		}

		public IFastlaneCertificate WithTeamName(string teamName)
		{
			TeamName = teamName;
			return this;
		}

		public IFastlaneCertificate WithProvisioningProfileName(string provisioningProfileName)
		{
			ProvisioningProfileName = provisioningProfileName;
			return this;
		}

		public IFastlaneCertificate WithCertificateType(CertificateType certificateType)
		{
			CertificateType = certificateType;
			return this;
		}

		public void Execute(IConfiguration configuration)
		{
			FastlaneCommand command = new FastlaneCommand(configuration.Context.CakeContext);

			if (string.IsNullOrEmpty(UserName))
			{
				configuration.Context.CakeContext.LogAndThrow($"Fastlane: {nameof(UserName)} is a required property");
			}
			if (string.IsNullOrEmpty(TeamName))
			{
				configuration.Context.CakeContext.LogAndThrow($"Fastlane: {nameof(TeamName)} is a required property");
			}
			if (string.IsNullOrEmpty(ProvisioningProfileName))
			{
				configuration.Context.CakeContext.LogAndThrow($"Fastlane: {nameof(ProvisioningProfileName)} is a required property");
			}

			string bundleId = configuration.GetSimple<string>(iOSConstants.BUNDLE_ID_KEY);
			if (string.IsNullOrEmpty(bundleId))
			{
				configuration.Context.CakeContext.LogAndThrow($"Fastlane: {nameof(bundleId)} is a required property");
			}

			string certificateOutputFile = configuration.AddRootDirectory($"{Guid.NewGuid():N}.mobileprovision");
			
			if (!command.SynchronizeProvisionningProfile(UserName, TeamName, bundleId, CertificateType, certificateOutputFile) 
				|| !configuration.Context.CakeContext.FileExists(certificateOutputFile))
			{
				configuration.Context.CakeContext.LogAndThrow("Error happened with fastlane, unable to read downloaded provisioning profile");
			}

			string provisioningContent = File.ReadAllText(certificateOutputFile);

			int keyIndex = provisioningContent.IndexOf("<key>UUID</key>", StringComparison.Ordinal);
			int valueStartIndex = provisioningContent.IndexOf("<string>", keyIndex, StringComparison.Ordinal) + "<string>".Length;
			int valueEndIndex = provisioningContent.IndexOf("</string>", keyIndex, StringComparison.Ordinal);

			string uuid = provisioningContent.Substring(valueStartIndex, valueEndIndex - valueStartIndex);
			configuration.Context.CakeContext.DeleteFile(certificateOutputFile);

			configuration.Add(iOSConstants.IOS_SIGN_PROVISION, new SimpleConfigurationItem<string>(uuid));
		}
	}
}
