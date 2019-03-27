using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Helpers;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.iOS.Steps
{
	[ReleaseStep]
	// ReSharper disable once InconsistentNaming
	internal class iOSReleaseStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			DirectoryPath outputDirectory = configuration.GetArtifactsPath();

			string solutionFile = configuration.GetSolutionPath();

			string[] projectsPath = configuration.GetProjectsPath();

			if (projectsPath.Length > 1)
			{
				configuration.Context.CakeContext.LogAndThrow($"Only one project supported");
			}

			string projectFile = projectsPath[0];
			configuration.FileExistsOrThrow(solutionFile);

			//Create iPA package
			configuration.Context.CakeContext.MSBuild(solutionFile, settings =>
			{
				settings.SetConfiguration("Release");
				settings.WithProperty("BuildIpa", "true")
					.WithProperty("IpaIncludeArtwork", "false")
					.WithProperty("IpaPackageDir", MSBuildHelper.PropertyValue(outputDirectory.MakeAbsolute(configuration.Context.CakeContext.Environment).FullPath))
					.WithProperty("Platform", "iPhone");

				string codeSignKey = configuration.Has(iOSConstants.IOS_CODESIGN_KEY) ? configuration.GetSimple<string>(iOSConstants.IOS_CODESIGN_KEY) : null;
				if (string.IsNullOrEmpty(codeSignKey))
				{
					configuration.Context.CakeContext.LogAndThrow("No codesignkey for iOS Release");
				}

				string codeSignProvision = configuration.Has(iOSConstants.IOS_CODESIGN_PROVISION) ? configuration.GetSimple<string>(iOSConstants.IOS_CODESIGN_PROVISION) : null;
				if (string.IsNullOrEmpty(codeSignProvision))
				{
					configuration.Context.CakeContext.LogAndThrow("No codesignprovision for iOS Release");
				}

				settings.WithProperty("CodesignKey", MSBuildHelper.PropertyValue(codeSignKey))
					.WithProperty("CodesignProvision", MSBuildHelper.PropertyValue(codeSignProvision));

				configuration.ApplyBuildParameters(settings);
			});

			//Copy dSYM to output
			string searchPattern = new FilePath(projectFile).GetDirectory() + "/**/*.dSYM";
			string symDirectory = configuration.Context.CakeContext.Globber
				.GetDirectories(searchPattern)
				.OrderBy(d => new DirectoryInfo(d.FullPath).LastWriteTimeUtc)
				.FirstOrDefault()?.FullPath;

			if (string.IsNullOrEmpty(symDirectory))
			{
				configuration.Context.CakeContext.LogAndThrow("Can not find dSYM file");
			}

			configuration.Context.CakeContext.Zip(symDirectory, $"{outputDirectory}/{System.IO.Path.GetFileName(symDirectory)}.zip");
		}
	}
}