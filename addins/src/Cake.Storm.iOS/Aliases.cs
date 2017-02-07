using System;
using System.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Common.Tools;

namespace Cake.Storm.iOS
{
	[CakeAliasCategory("Cake.Storm.iOS")]
	public static class Aliases
	{
		[CakeMethodAlias]
		public static string FastlaneGetCertificate(this ICakeContext context, string bundleId, string userName, string teamName, CertificateType certificateType)
		{
			FastlaneSighCommand command = new FastlaneSighCommand(context);
			string certificateFile = $"{Guid.NewGuid()}.mobileprovision";
			if(!command.GetCertificate(userName, teamName, bundleId, certificateType, certificateFile))
			{
				throw new CakeException("Unable to retrieve provisioning profile using fastlane sigh");
			}

			string provisioningContent = File.ReadAllText(certificateFile);

			int keyIndex = provisioningContent.IndexOf("<key>UUID</key>");
			int valueStartIndex = provisioningContent.IndexOf("<string>", keyIndex);
			int valueEndIndex = provisioningContent.IndexOf("</string>", keyIndex) + "</string>".Length;

			string uuid = provisioningContent.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

			context.Log.Information($"Got certificate uuid {uuid} for bundle {bundleId}");
			return uuid;
		}

		[CakeMethodAlias]
		public static PList LoadPListFile(this ICakeContext context, FilePath file)
		{
			if (!context.FileSystem.Exist(file))
			{
				context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"PList file {file} does not exists");
				throw new CakeException($"PList file {file} does not exists");
			}

			return new PList(context, file.MakeAbsolute(context.Environment).FullPath);
		}

		[CakeMethodAlias]
		public static void CreateIpaFile(this ICakeContext context, FilePath projectFile, DirectoryPath outputDirectory, Action<DotNetBuildSettings> configurator = null)
		{
			context.DotNetBuild(projectFile.MakeAbsolute(context.Environment), configuration =>
			{
				configuration.Configuration = "Release";
				configuration.Targets.Add("Build");

				configuration.WithProperty("BuildIpa", "true")
							 .WithProperty("IpaIncludeArtwork", "false")
							 .WithProperty("IpaPackageDir", outputDirectory.MakeAbsolute(context.Environment).FullPath)
				             .WithProperty("Platform", "iPhone");

				configurator?.Invoke(configuration);
			});
		}

		[CakeMethodAlias]
		public static void CreateIpaFileWithSignature(this ICakeContext context, FilePath projectFile, DirectoryPath outputDirectory, string codeSignKey, string codeSignProvision, Action<DotNetBuildSettings> configurator = null)
		{
			context.DotNetBuild(projectFile.MakeAbsolute(context.Environment), configuration =>
			{
				configuration.Configuration = "Release";
				configuration.Targets.Add("Build");

				configuration.WithProperty("BuildIpa", "true")
							 .WithProperty("IpaIncludeArtwork", "false")
							 .WithProperty("IpaPackageDir", outputDirectory.MakeAbsolute(context.Environment).FullPath)
							 .WithProperty("Platform", "iPhone");

				if (!string.IsNullOrEmpty(codeSignKey))
				{
					configuration.WithProperty("CodesignKey", codeSignKey);
				}
				if (!string.IsNullOrEmpty(codeSignProvision))
				{
					configuration.WithProperty("CodesignProvision", codeSignProvision);
				}

				configurator?.Invoke(configuration);
			});
		}
	}
}
