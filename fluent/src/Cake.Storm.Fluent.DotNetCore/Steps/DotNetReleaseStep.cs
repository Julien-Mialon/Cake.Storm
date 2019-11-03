using System;
using System.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.DotNetCore.Common;
using Cake.Storm.Fluent.DotNetCore.InternalExtensions;
using Cake.Storm.Fluent.DotNetCore.Models;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.DotNetCore.Steps
{
	[ReleaseStep]
	internal class DotNetReleaseStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			//default to Copy
			OutputType outputType = OutputType.Copy;

			if (configuration.Has(DotNetCoreConstants.DOTNETCORE_OUTPUT_TYPE_KEY))
			{
				outputType = configuration.GetSimple<OutputType>(DotNetCoreConstants.DOTNETCORE_OUTPUT_TYPE_KEY);
			}

			switch (outputType)
			{
				case OutputType.Publish:
					ExecutePublish(configuration);
					break;
				case OutputType.Pack:
					ExecutePack(configuration);
					break;
				case OutputType.Copy:
					ExecuteCopy(configuration);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(outputType), outputType, "Invalid value for DotNetCore output type");
			}
		}

		private void ExecutePublish(IConfiguration configuration)
		{
			foreach (string project in configuration.GetProjectsPath())
			{
				Run(project);
			}

			void Run(string projectPath)
			{
				configuration.FileExistsOrThrow(projectPath);

				configuration.RunOnConfiguredTargetFramework(framework =>
				{
					DotNetCorePublishSettings settings = new DotNetCorePublishSettings
					{
						OutputDirectory = ArtifactsPath(configuration, framework),
						Framework = framework
					};
					configuration.ApplyBuildParameters(projectPath, settings);
					configuration.Context.CakeContext.DotNetCorePublish(projectPath, settings);
				});
			}
		}

		private void ExecutePack(IConfiguration configuration)
		{
			foreach (string project in configuration.GetProjectsPath())
			{
				Run(project);
			}

			void Run(string projectPath)
			{
				configuration.FileExistsOrThrow(projectPath);

				configuration.RunOnConfiguredTargetFramework(framework =>
				{
					DotNetCorePackSettings settings = new DotNetCorePackSettings
					{
						OutputDirectory = ArtifactsPath(configuration, framework)
					};
					configuration.ApplyBuildParameters(projectPath, settings);
					configuration.Context.CakeContext.DotNetCorePack(projectPath, settings);
				});
			}
		}

		private void ExecuteCopy(IConfiguration configuration)
		{
			foreach (string project in configuration.GetProjectsPath())
			{
				Run(project);
			}

			void Run(string projectPath)
			{
				configuration.FileExistsOrThrow(projectPath);

				var buildParameters =
					configuration.Get<DictionaryOfListConfigurationItem<string, string>>(
						ConfigurationConstants.BUILD_PARAMETERS_KEY);
				string buildConfiguration = buildParameters.Values.Where(x => x.Key.ToLowerInvariant() == "configuration")
					                            .Select(x => x.Value).FirstOrDefault()?.Single() ?? "Debug";

				FilePath projectFilePath = projectPath;
				DirectoryPath projectOutputPath = projectFilePath.GetDirectory().Combine("bin").Combine(buildConfiguration);

				string copyFileName = projectFilePath.GetFilenameWithoutExtension().ToString();
				string copyPattern = copyFileName + ".*";

				configuration.RunOnConfiguredTargetFramework(framework =>
				{
					DirectoryPath outputPathForFramework;
					if (framework == null)
					{
						outputPathForFramework = configuration.Context.CakeContext.FileSystem.GetDirectory(projectOutputPath)
							.GetDirectories("*", SearchScope.Current).FirstOrDefault()?.Path;
						if (outputPathForFramework == null)
						{
							configuration.Context.CakeContext.LogAndThrow($"Cannot determine framework to use for project {projectPath}");
							throw new Exception();
						}
					}
					else
					{
						outputPathForFramework = projectOutputPath.Combine(framework);
					}

					configuration.Context.CakeContext.CopyFiles(
						configuration.Context.CakeContext.FileSystem
							.GetDirectory(outputPathForFramework)
							.GetFiles(copyPattern, SearchScope.Recursive)
							.Select(x => x.Path)
							.Where(x => x.GetFilenameWithoutExtension().ToString() == copyFileName && (x.GetExtension() == ".dll" || x.GetExtension() == ".pdb" || x.GetExtension() == ".xml")),
						ArtifactsPath(configuration, framework ?? outputPathForFramework.GetDirectoryName()),
						preserveFolderStructure: true);
				});
			}
		}

		private DirectoryPath ArtifactsPath(IConfiguration configuration, string framework)
		{
			if (framework == null)
			{
				return configuration.GetArtifactsPath();
			}

			DirectoryPath resultPath = configuration.GetArtifactsPath().Combine(framework);
			configuration.Context.CakeContext.EnsureDirectoryExists(resultPath);
			return resultPath;
		}
	}
}