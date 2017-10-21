using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.NuGet.Models;
using Cake.Storm.Fluent.Resolvers;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.NuGet.Steps
{
	[PreDeployStep]
	public class NugetPackStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			//get artifacts output directory
			DirectoryPath artifactsPath = configuration.GetArtifactsPath();

			//find nuspec file
			if (!configuration.TryGetSimple(NuGetConstants.NUGET_NUSPEC_FILE_KEY, out string nuspecPath))
			{
				configuration.Context.CakeContext.LogAndThrow("Missing nuspec file path in configuration");
			}

			// get package id
			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, out string packageId))
			{
				packageId = ReadPackageIdFromNuspec(configuration, configuration.AddRootDirectory(nuspecPath));
				configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, packageId);
			}

			// get nuget package version
			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, out string packageVersion))
			{
				if (!configuration.TryGetSimple(ConfigurationConstants.VERSION_KEY, out packageVersion))
				{
					packageVersion = ReadPackageVersionFromNuspec(configuration, configuration.AddRootDirectory(nuspecPath));
				}
				configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, packageVersion);
			}

			List<NugetFile> nugetFiles = new List<NugetFile>();
			if (configuration.TryGet(NuGetConstants.NUGET_FILES_KEY, out ListConfigurationItem<IValueResolver<IEnumerable<NugetFile>>> fileResolvers))
			{
				nugetFiles = fileResolvers.Values.SelectMany(x => x.Resolve(configuration)).ToList();
			}
			
			IDirectory artifactsDirectory = configuration.Context.CakeContext.FileSystem.GetDirectory(artifactsPath);
			//create nuget output directory
			DirectoryPath nugetContentPath = artifactsPath.Combine(packageId);
			configuration.Context.CakeContext.EnsureDirectoryExists(nugetContentPath);
			
			foreach (NugetFile nugetFile in nugetFiles)
			{
				string file = nugetFile.FilePath;
				configuration.FileExistsOrThrow(file);
				DirectoryPath destinationPath = nugetContentPath;
				if (!string.IsNullOrEmpty(nugetFile.NugetRelativePath))
				{
					destinationPath = destinationPath.Combine(nugetFile.NugetRelativePath);
				}
				configuration.Context.CakeContext.EnsureDirectoryExists(destinationPath);
				configuration.Context.CakeContext.CopyFileToDirectory(file, destinationPath);
			}
			
			//copy nuspec with parameters
			string nuspecOutputPath = Nuspec(configuration, nugetContentPath);

			if (configuration.TryGet(NuGetConstants.NUGET_FILES_KEY, out ListConfigurationItem<NugetFile> list))
			{
				foreach (NugetFile nugetFile in list.Values)
				{
					string file = configuration.AddRootDirectory(nugetFile.FilePath);
					configuration.FileExistsOrThrow(file);
					DirectoryPath destinationPath = nugetContentPath;
					if (!string.IsNullOrEmpty(nugetFile.NugetRelativePath))
					{
						destinationPath = destinationPath.Combine(nugetFile.NugetRelativePath);
					}
					configuration.Context.CakeContext.EnsureDirectoryExists(destinationPath);
					configuration.Context.CakeContext.CopyFileToDirectory(file, destinationPath);
				}
			}

			configuration.Context.CakeContext.NuGetPack(nuspecOutputPath, new NuGetPackSettings
			{
				OutputDirectory = artifactsPath,
			});

			string nugetPackagePath = artifactsPath.CombineWithFilePath($"{packageId}.{packageVersion}.nupkg").FullPath;
			configuration.AddSimple(NuGetConstants.NUGET_PACK_OUTPUT_FILE_KEY, nugetPackagePath);
		}

		private string ReadPackageIdFromNuspec(IConfiguration configuration, string nuspecPath)
		{
			configuration.FileExistsOrThrow(nuspecPath);
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecPath).OpenRead())
			{
				XDocument document = XDocument.Load(inputStream);
				XElement metadataRoot = GetMetadataRootFromNuspec(configuration, document);

				XElement idElement = metadataRoot.Descendants(XName.Get("id")).FirstOrDefault();
				if (idElement == null)
				{
					configuration.LogAndThrow($"Invalid nuspec file, missing id tag {nuspecPath}");
					throw new Exception();
				}

				return idElement.Value;
			}
		}
		
		private string ReadPackageVersionFromNuspec(IConfiguration configuration, string nuspecPath)
		{
			configuration.FileExistsOrThrow(nuspecPath);
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecPath).OpenRead())
			{
				XDocument document = XDocument.Load(inputStream);
				XElement metadataRoot = GetMetadataRootFromNuspec(configuration, document);

				XElement versionElement = metadataRoot.Descendants(XName.Get("version")).FirstOrDefault();
				if (versionElement == null)
				{
					configuration.LogAndThrow($"Invalid nuspec file, missing id tag {nuspecPath}");
					throw new Exception();
				}

				return versionElement.Value;
			}
		}

		private XElement GetMetadataRootFromNuspec(IConfiguration configuration, XDocument document)
		{
			if (document.Root == null)
			{
				configuration.LogAndThrow($"Invalid nuspec file, not an xml file format");
				throw new Exception();
			}

			if (document.Root.Name.LocalName != "package")
			{
				configuration.LogAndThrow($"Invalid nuspec file, root is not package tag");
				throw new Exception();
			}

			XElement metadataRoot = document.Root.Descendants(XName.Get("metadata")).FirstOrDefault();
			if (metadataRoot == null)
			{
				configuration.LogAndThrow($"Invalid nuspec file, missing metadata tag");
				throw new Exception();
			}

			return metadataRoot;
		}

		private void UpdateOrCreateNode(XElement metadataRoot, string nodeName, string value)
		{
			XElement child = metadataRoot.Descendants(XName.Get(nodeName)).FirstOrDefault();
			if (child == null)
			{
				child = new XElement(XName.Get(nodeName));
				metadataRoot.Add(child);
			}

			child.SetValue(value);
		}

		private string Nuspec(IConfiguration configuration, DirectoryPath nugetContentPath)
		{
			if (!configuration.TryGetSimple(NuGetConstants.NUGET_NUSPEC_FILE_KEY, out string nuspecPath))
			{
				configuration.Context.CakeContext.LogAndThrow("Missing nuspec file path");
			}
			nuspecPath = configuration.AddRootDirectory(nuspecPath);
			configuration.FileExistsOrThrow(nuspecPath);

			XDocument nuspecDocument;
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecPath).OpenRead())
			{
				nuspecDocument = XDocument.Load(inputStream);
			}

			XElement metadataRoot = GetMetadataRootFromNuspec(configuration, nuspecDocument);
			string packageId = configuration.GetSimple<string>(NuGetConstants.NUGET_PACKAGE_ID_KEY);

			UpdateOrCreateNode(metadataRoot, "id", packageId);

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_AUTHOR_KEY, out string author))
			{
				UpdateOrCreateNode(metadataRoot, "authors", author);
			}

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_RELEASE_NOTES_FILE_KEY, out string releaseNoteFile))
			{
				releaseNoteFile = configuration.AddRootDirectory(releaseNoteFile);
				configuration.FileExistsOrThrow(releaseNoteFile);

				string releaseNoteContent = configuration.Context.CakeContext.FileSystem.GetFile(releaseNoteFile).ReadAll();

				UpdateOrCreateNode(metadataRoot, "releaseNotes", releaseNoteContent);
			}

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, out string version)
			    || configuration.TryGetSimple(ConfigurationConstants.VERSION_KEY, out version))
			{
				UpdateOrCreateNode(metadataRoot, "version", version);
			}

			FilePath nuspecOutputPath = nugetContentPath.CombineWithFilePath($"{packageId}.nuspec");

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecOutputPath).OpenWrite())
			{
				nuspecDocument.Save(outputStream);
			}

			return nuspecOutputPath.FullPath;
		}
	}
}