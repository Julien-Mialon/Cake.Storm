using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.NuGet.Steps
{
	public class NuGetPackStep : IStep
	{
		public void Execute(IConfiguration configuration)
		{
			//get artifacts output directory
			DirectoryPath artifactsPath = configuration.GetArtifactsPath();

			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, out string packageId))
			{
				if (configuration.TryGetSimple(NuGetConstants.NUSPEC_FILE_KEY, out string nuspecPath))
				{
					packageId = ReadPackageIdFromNuspec(configuration, configuration.AddRootDirectory(nuspecPath));
					configuration.AddSimple(NuGetConstants.NUGET_PACKAGE_ID_KEY, packageId);
				}
				else
				{
					configuration.Context.CakeContext.LogAndThrow("Can not determine nuget package id using either the PackageId configuration or the id in the Nuspec file");
				}
			}

			IDirectory artifactsDirectory = configuration.Context.CakeContext.FileSystem.GetDirectory(artifactsPath);
			List<FilePath> inputFiles = artifactsDirectory.GetFiles("*", SearchScope.Current).Select(x => x.Path).ToList();
			List<DirectoryPath> inputDirectories = artifactsDirectory.GetDirectories("*", SearchScope.Current).Select(x => x.Path).ToList();

			//create nuget output directory
			DirectoryPath nugetContentPath = artifactsPath.Combine(packageId);
			configuration.Context.CakeContext.EnsureDirectoryExists(nugetContentPath);

			//copy existing file in nuget content directory
			foreach (DirectoryPath inputDirectory in inputDirectories)
			{
				configuration.Context.CakeContext.CopyDirectory(inputDirectory, nugetContentPath);
			}
			foreach (FilePath inputFile in inputFiles)
			{
				configuration.Context.CakeContext.CopyFileToDirectory(inputFile, nugetContentPath);
			}

			//copy nuspec with parameters
			Nuspec(configuration, nugetContentPath);

			//TODO: handle additional files
		}

		private string ReadPackageIdFromNuspec(IConfiguration configuration, string nuspecPath)
		{
			configuration.FileExistsOrThrow(nuspecPath);
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecPath).OpenRead())
			{
				XDocument document = XDocument.Load(inputStream);

				if (document.Root == null)
				{
					configuration.Context.CakeContext.LogAndThrow($"Invalid nuspec file, not an xml file format");
					throw new Exception();
				}

				//TODO: parse with children.Children to be sure to get the right node
				foreach (var idNode in document.Root.Descendants(XName.Get("id")))
				{
					return idNode.Value;
				}
			}

			return null;
		}

		private void Nuspec(IConfiguration configuration, DirectoryPath nugetContentPath)
		{
			if (!configuration.TryGetSimple(NuGetConstants.NUSPEC_FILE_KEY, out string nuspecPath))
			{
				configuration.Context.CakeContext.LogAndThrow("Missing nuspec file path");
			}
			nuspecPath = configuration.AddRootDirectory(nuspecPath);
			configuration.FileExistsOrThrow(nuspecPath);

			string fileContent = configuration.Context.CakeContext.FileSystem.GetFile(nuspecPath).ReadAll();

			string packageId = configuration.GetSimple<string>(NuGetConstants.NUGET_PACKAGE_ID_KEY);
			fileContent = fileContent.Replace("{id}", packageId);

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_AUTHOR_KEY, out string author))
			{
				fileContent = fileContent.Replace("{author}", author);
			}

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_RELEASE_NOTES_FILE_KEY, out string releaseNoteFile))
			{
				releaseNoteFile = configuration.AddRootDirectory(releaseNoteFile);
				configuration.FileExistsOrThrow(releaseNoteFile);

				//TODO: see how to improve the fact that release notes can contains xml characters...
				fileContent = fileContent.Replace("{release_notes}", configuration.Context.CakeContext.FileSystem.GetFile(releaseNoteFile).ReadAll());
			}

			if (configuration.TryGetSimple(NuGetConstants.NUGET_PACKAGE_VERSION_KEY, out string version)
				|| configuration.TryGetSimple(ConfigurationConstants.VERSION_KEY, out version))
			{
				fileContent = fileContent.Replace("{version}", version);
			}

			FilePath nuspecOutputPath = nugetContentPath.CombineWithFilePath($"{packageId}.nuspec");

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecOutputPath).OpenWrite())
			{
				using (TextWriter outputWriter = new StreamWriter(outputStream, Encoding.UTF8))
				{
					outputWriter.Write(fileContent);
				}
			}
		}
	}
}