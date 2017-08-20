using System;
using System.IO;
using Cake.Common.IO;
using Cake.Core;
using Cake.Storm.Fluent.iOS.Commands;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.iOS.Extensions;
using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.iOS.Steps
{
	[PreReleaseStep]
	internal class FastlaneCertificateStep : IStep
	{
		public void Execute(IConfiguration configuration)
		{
			FastlaneCommand command = new FastlaneCommand(configuration.Context.CakeContext);

			if (!configuration.TryGetSimple(iOSConstants.FASTLANE_APPLE_USERNAME, out string userName))
			{
				configuration.LogAndThrow("Fastlane: missing user name");
			}
			
			if (!configuration.TryGetSimple(iOSConstants.FASTLANE_APPLE_TEAMNAME, out string teamName))
			{
				configuration.LogAndThrow("Fastlane: missing team name");
			}
			
			if (!configuration.TryGetSimple(iOSConstants.IOS_BUNDLE_ID_KEY, out string bundleId))
			{
				configuration.LogAndThrow("Fastlane: missing bundle id");
			}

			if (!configuration.TryGetSimple(iOSConstants.FASTLANE_SIGH_CERTIFICATE_TYPE, out CertificateType certificateType))
			{
				certificateType = CertificateType.Development;
			}
			
			string certificateOutputFile = configuration.AddRootDirectory($"{Guid.NewGuid():N}.mobileprovision");
			
			if (!command.SynchronizeProvisionningProfile(userName, teamName, bundleId, certificateType, certificateOutputFile) 
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

			configuration.WithSignProvision(uuid);
		}
	}
}