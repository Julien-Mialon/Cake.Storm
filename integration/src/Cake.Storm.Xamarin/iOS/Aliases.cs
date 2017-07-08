using System;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Storm.Xamarin.iOS.Commands;
using Cake.Storm.Xamarin.iOS.Models;

namespace Cake.Storm.Xamarin.iOS
{
	[CakeAliasCategory("Cake.Storm.Xamarin.iOS")]
	[CakeNamespaceImport("Cake.Storm.Xamarin.iOS.Models")]
	public static class Aliases
	{
		[CakeMethodAlias]
		public static string FastlaneGetCertificate(this ICakeContext context, string bundleId, string userName, string teamName, CertificateType certificateType)
		{
			FastlaneCommand command = new FastlaneCommand(context);
			string certificateFile = $"{Guid.NewGuid()}.mobileprovision";
			if (!command.SynchronizeProvisionningProfile(userName, teamName, bundleId, certificateType, certificateFile))
			{
				context.LogAndThrow("Unable to retrieve provisioning profile using fastlane sigh");
			}

			string provisioningContent = File.ReadAllText(certificateFile);

			int keyIndex = provisioningContent.IndexOf("<key>UUID</key>", StringComparison.Ordinal);
			int valueStartIndex = provisioningContent.IndexOf("<string>", keyIndex, StringComparison.Ordinal) + "<string>".Length;
			int valueEndIndex = provisioningContent.IndexOf("</string>", keyIndex, StringComparison.Ordinal);

			string uuid = provisioningContent.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

			context.Log.Information($"Got certificate uuid {uuid} for bundle {bundleId}");
			File.Delete(certificateFile);

			return uuid;
		}

		[CakeMethodAlias]
		public static ApplicationPList LoadApplicationPList(this ICakeContext context, FilePath file)
		{
			if (!context.FileSystem.Exist(file))
			{
				context.LogAndThrow($"PList file {file} does not exists");
			}

			return new ApplicationPList(context, file.MakeAbsolute(context.Environment).FullPath);
		}

		[CakeMethodAlias]
		public static void CreateIpaFile(this ICakeContext context, FilePath projectFile, DirectoryPath outputDirectory, Action<MSBuildSettings> configurator = null)
		{
			context.MSBuild(projectFile.MakeAbsolute(context.Environment), configuration =>
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
		public static void CreateIpaFileWithSignature(this ICakeContext context, FilePath projectFile, DirectoryPath outputDirectory, string codeSignKey, string codeSignProvision, Action<MSBuildSettings> configurator = null)
		{
			context.MSBuild(projectFile.MakeAbsolute(context.Environment), configuration =>
			{
				configuration.Configuration = "Release";
				configuration.Targets.Add("Build");

				configuration.WithProperty("BuildIpa", "true")
					.WithProperty("IpaIncludeArtwork", "false")
					.WithProperty("IpaPackageDir", outputDirectory.MakeAbsolute(context.Environment).FullPath)
					.WithProperty("Platform", "iPhone");

				if (!string.IsNullOrEmpty(codeSignKey))
				{
					configuration.WithProperty("CodesignKey", codeSignKey.Replace(",", "%2c"));
				}
				if (!string.IsNullOrEmpty(codeSignProvision))
				{
					configuration.WithProperty("CodesignProvision", codeSignProvision);
				}

				configurator?.Invoke(configuration);
			});
		}

		[CakeMethodAlias]
		public static void CopyDSymToOutputDirectory(this ICakeContext context, FilePath solutionFile, DirectoryPath outputDirectory)
		{
			string searchPattern = solutionFile.GetDirectory() + "/**/*.dSYM";
			// Use the globber to find any dSYM directory within the tree
			string symDirectory = context.Globber
				.GetDirectories(searchPattern)
				.OrderBy(d => new DirectoryInfo(d.FullPath).LastWriteTimeUtc)
				.FirstOrDefault()?.FullPath;

			if (string.IsNullOrEmpty(symDirectory))
			{
				context.LogAndThrow("Can not find dSYM file");
			}

			context.Zip(symDirectory, $"{outputDirectory}/{System.IO.Path.GetFileName(symDirectory)}.zip");
		}
	}
}
