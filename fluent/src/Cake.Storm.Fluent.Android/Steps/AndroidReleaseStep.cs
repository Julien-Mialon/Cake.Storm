using System;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.Steps;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.Android.Steps
{
	[ReleaseStep]
	internal class AndroidReleaseStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			DirectoryPath outputDirectory = configuration.GetArtifactsPath();

			FilePath projectFile = configuration.GetProjectPath();
			configuration.FileExistsOrThrow(projectFile);

			//Create apk package
			configuration.Context.CakeContext.MSBuild(projectFile, settings =>
			{
				settings.SetConfiguration("Release");
				settings.WithTarget("PackageForAndroid");
				
				configuration.ApplyBuildParameters(settings);
			});
			
			//find apk package
			string searchPattern = Path.Combine(projectFile.GetDirectory().FullPath, "**", "*.apk"); 
			FilePath apkPath =  configuration.Context.CakeContext.Globber
				.GetFiles(searchPattern)
				.OrderBy(f => new FileInfo(f.FullPath).LastWriteTimeUtc)
				.FirstOrDefault();
			
			if (apkPath == null)
			{
				configuration.Context.CakeContext.LogAndThrow("Can not find generated apk file");
				throw new Exception(); //static analyzer
			}

			string artifactsApkPath = Path.Combine(outputDirectory.FullPath, apkPath.GetFilename().ToString());
			configuration.Context.CakeContext.CopyFile(apkPath, artifactsApkPath);
			
			configuration.Add(AndroidConstants.GENERATED_ANDROID_PACKAGE_PATH_KEY, new SimpleConfigurationItem<string>(artifactsApkPath));
		}
	}
}