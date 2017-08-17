using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.NuGet.Common;

namespace Cake.Storm.Fluent.NuGet.Extensions
{
	public static class ConfigurationExtensions
	{
		public static TConfiguration UseNuGetTooling<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
		{
			return configuration;
		}

		public static TConfiguration WithNuspec<TConfiguration>(this TConfiguration configuration, string nuspecFile)
			where TConfiguration : IConfiguration
		{
			configuration.AddSimple(NuGetConstants.NUSPEC_FILE_KEY, nuspecFile);
			return configuration;
		}

		public static TConfiguration WithNugetPackageId<TConfiguration>(this TConfiguration configuration, string packageId)
			where TConfiguration : IConfiguration
		{
			configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, packageId);
			return configuration;
		}
		
		public static TConfiguration WithNugetPackageAuthor<TConfiguration>(this TConfiguration configuration, string author)
			where TConfiguration : IConfiguration
		{
			configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_AUTHOR_KEY, author);
			return configuration;
		}
		
		public static TConfiguration WithNugetPackageReleaseNotes<TConfiguration>(this TConfiguration configuration, string releaseNoteFile)
			where TConfiguration : IConfiguration
		{
			configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_RELEASE_NOTES_FILE_KEY, releaseNoteFile);
			return configuration;
		}
		
		public static TConfiguration WithNugetPackageVersion<TConfiguration>(this TConfiguration configuration, string version)
			where TConfiguration : IConfiguration
		{
			configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, version);
			return configuration;
		}

		public static TConfiguration WithFileInNuget<TConfiguration>(this TConfiguration configuration, string filePath, string nugetRelativePath)
			where TConfiguration : IConfiguration
		{
			if (configuration.TryGet(NuGetConstants.NUGET_ADDITIONAL_FILES_KEY, out ListConfigurationItem<(string filePath, string nugetRelativePath)> list))
			{
				list.Values.Add((filePath, nugetRelativePath));
			}
			else
			{
				configuration.Add(NuGetConstants.NUGET_ADDITIONAL_FILES_KEY, new ListConfigurationItem<(string filePath, string nugetRelativePath)>((filePath, nugetRelativePath)));
			}
			return configuration;
		}
	}
}