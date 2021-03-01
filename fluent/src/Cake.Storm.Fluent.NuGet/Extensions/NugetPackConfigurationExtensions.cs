using System.Collections.Generic;
using System.IO;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.NuGet.Interfaces;
using Cake.Storm.Fluent.NuGet.Models;
using Cake.Storm.Fluent.NuGet.Resolvers;
using Cake.Storm.Fluent.Resolvers;

namespace Cake.Storm.Fluent.NuGet.Extensions
{
	public static class NugetPackConfigurationExtensions
	{
		public static INugetPackConfiguration WithNuspec(this INugetPackConfiguration configuration, string nuspecFile)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_NUSPEC_FILE_KEY, nuspecFile);
			return configuration;
		}

		public static INugetPackConfiguration WithPackageId(this INugetPackConfiguration configuration, string packageId)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, packageId);
			return configuration;
		}

		public static INugetPackConfiguration WithAuthor(this INugetPackConfiguration configuration, string author)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_AUTHOR_KEY, author);
			return configuration;
		}

		public static INugetPackConfiguration WithReleaseNotesFile(this INugetPackConfiguration configuration, string releaseNoteFile)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_RELEASE_NOTES_FILE_KEY, releaseNoteFile);
			return configuration;
		}

		public static INugetPackConfiguration WithVersion(this INugetPackConfiguration configuration, string version)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, version);
			return configuration;
		}

		public static INugetPackConfiguration WithDependenciesFromProjectFile(this INugetPackConfiguration configuration, string csprojFile)
		{
			if (configuration.Configuration.TryGet(NuGetConstants.NUGET_DEPENDENCIES_KEY, out ListConfigurationItem<string> files))
			{
				files.Values.Add(csprojFile);
			}
			else
			{
				configuration.Configuration.Add(NuGetConstants.NUGET_DEPENDENCIES_KEY,
					new ListConfigurationItem<string>(csprojFile));
			}
			return configuration;
		}

		public static INugetPackConfiguration WithDependenciesFromProject(this INugetPackConfiguration configuration)
		{
			return configuration.WithDependenciesFromProjectFile(NuGetConstants.NUGET_DEPENDENCIES_FROM_PROJECT_VALUE);
		}

		public static INugetPackConfiguration WithDependenciesFromProjectFiles(this INugetPackConfiguration configuration, params string[] csprojFiles)
		{
			foreach (string csprojFile in csprojFiles)
			{
				configuration = configuration.WithDependenciesFromProjectFile(csprojFile);
			}

			return configuration;
		}

		public static INugetPackConfiguration AddAllFilesFromArtifacts(this INugetPackConfiguration configuration, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryNugetFileResolver(null, nugetRelativePath, (dir, c) => c.GetArtifactsPath().FullPath));
		}

		public static INugetPackConfiguration AddFilesFromArtifacts(this INugetPackConfiguration configuration, string filePattern, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryWithPatternNugetFileResolver(null, filePattern, nugetRelativePath, (dir, c) => c.GetArtifactsPath().FullPath));
		}

		public static INugetPackConfiguration AddFileFromArtifacts(this INugetPackConfiguration configuration, string artifactsRelativeFilePath, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new SimpleFileNugetFileResolver(artifactsRelativeFilePath, nugetRelativePath, (file, c) => Path.Combine(c.GetArtifactsPath().FullPath, file)));
		}

		public static INugetPackConfiguration AddAllFilesFromBuild(this INugetPackConfiguration configuration, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryNugetFileResolver(null, nugetRelativePath, (dir, c) => c.GetBuildPath().FullPath));
		}

		public static INugetPackConfiguration AddFilesFromBuild(this INugetPackConfiguration configuration, string filePattern, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryWithPatternNugetFileResolver(null, filePattern, nugetRelativePath, (dir, c) => c.GetBuildPath().FullPath));
		}

		public static INugetPackConfiguration AddFileFromBuild(this INugetPackConfiguration configuration, string buildRelativeFilePath, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new SimpleFileNugetFileResolver(buildRelativeFilePath, nugetRelativePath, (file, c) =>  Path.Combine(c.GetBuildPath().FullPath, file)));
		}

		public static INugetPackConfiguration AddAllFiles(this INugetPackConfiguration configuration, string directory, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryNugetFileResolver(directory, nugetRelativePath, (dir, c) => c.AddRootDirectory(dir)));
		}

		public static INugetPackConfiguration AddFiles(this INugetPackConfiguration configuration, string directory, string filePattern, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new AllFilesFromDirectoryWithPatternNugetFileResolver(directory, filePattern, nugetRelativePath, (dir, c) => c.AddRootDirectory(dir)));
		}

		public static INugetPackConfiguration AddFile(this INugetPackConfiguration configuration, string filePath, string nugetRelativePath = null)
		{
			return configuration.AddFileResolver(new SimpleFileNugetFileResolver(filePath, nugetRelativePath, (file, c) => c.AddRootDirectory(file)));
		}

		private static INugetPackConfiguration AddFileResolver(this INugetPackConfiguration configuration, IValueResolver<IEnumerable<NugetFile>> resolver)
		{
			if (configuration.Configuration.TryGet(NuGetConstants.NUGET_FILES_KEY, out ListConfigurationItem<IValueResolver<IEnumerable<NugetFile>>> list))
			{
				list.Values.Add(resolver);
			}
			else
			{
				configuration.Configuration.Add(NuGetConstants.NUGET_FILES_KEY,
					new ListConfigurationItem<IValueResolver<IEnumerable<NugetFile>>>(resolver));
			}
			return configuration;
		}
	}
}