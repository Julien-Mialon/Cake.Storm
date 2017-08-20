using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.NuGet.Interfaces;
using Cake.Storm.Fluent.NuGet.Models;

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

		public static INugetPackConfiguration AddFile(this INugetPackConfiguration configuration, string filePath, string nugetRelativePath = null)
		{
			if (configuration.Configuration.TryGet(NuGetConstants.NUGET_ADDITIONAL_FILES_KEY, out ListConfigurationItem<NugetFile> list))
			{
				list.Values.Add(new NugetFile(filePath, nugetRelativePath));
			}
			else
			{
				configuration.Configuration.Add(NuGetConstants.NUGET_ADDITIONAL_FILES_KEY, new ListConfigurationItem<NugetFile>(new NugetFile(filePath, nugetRelativePath)));
			}
			return configuration;
		}
	}
}