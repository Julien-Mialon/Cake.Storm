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

			UpdateNuspecDependencies(metadataRoot, DependenciesFromProjectFiles(configuration));

			FilePath nuspecOutputPath = nugetContentPath.CombineWithFilePath($"{packageId}.nuspec");

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(nuspecOutputPath).OpenWrite())
			{
				nuspecDocument.Save(outputStream);
			}

			return nuspecOutputPath.FullPath;
		}

		private void UpdateNuspecDependencies(XElement metadataRoot, List<NugetDependency> dependencies)
		{
			const string DEPENDENCIES_NODE = "dependencies";
			const string GROUP_NODE = "group";
			const string DEPENDENCY_NODE = "dependency";
			const string ID_ATTRIBUTE = "id";
			const string VERSION_ATTRIBUTE = "version";

			XElement dependenciesNode = metadataRoot.Descendants(XName.Get(DEPENDENCIES_NODE)).FirstOrDefault();
			if (dependenciesNode == null)
			{
				dependenciesNode = new XElement(XName.Get(DEPENDENCIES_NODE));
				metadataRoot.Add(dependenciesNode);
			}

			List<XElement> groups = dependenciesNode.Descendants(XName.Get(GROUP_NODE)).ToList();
			if (groups.Count > 0)
			{
				foreach (XElement group in groups)
				{
					AddDependencies(group, dependencies);
				}
			}
			else
			{
				AddDependencies(dependenciesNode, dependencies);
			}

			static void AddDependencies(XElement root, List<NugetDependency> dependencies)
			{
				List<XElement> dependenciesNodes = root.Descendants(XName.Get(DEPENDENCY_NODE)).ToList();
				Dictionary<string, NugetDependency> dependenciesToHave = dependencies.ToDictionary(x => x.PackageId);

				root.RemoveNodes();

				foreach (XElement dependencyNode in dependenciesNodes)
				{
					string id = dependencyNode.Attributes().Single(x => x.Name.LocalName == ID_ATTRIBUTE).Value;

					if (dependenciesToHave.TryGetValue(id, out NugetDependency dependency))
					{
						dependenciesToHave.Remove(id);
						dependencyNode.Attributes().Single(x => x.Name.LocalName == VERSION_ATTRIBUTE).Value = dependency.Version;
					}

					root.Add(dependencyNode);
				}

				foreach (NugetDependency dependency in dependenciesToHave.Values)
				{
					XElement newNode = new(XName.Get(DEPENDENCY_NODE), new XAttribute(XName.Get(ID_ATTRIBUTE), dependency.PackageId), new XAttribute(XName.Get(VERSION_ATTRIBUTE), dependency.Version));
					root.Add(newNode);
				}
			}
		}

		private List<NugetDependency> DependenciesFromProjectFiles(IConfiguration configuration)
		{
			if (configuration.TryGet(NuGetConstants.NUGET_DEPENDENCIES_KEY, out ListConfigurationItem<string> files))
			{
				List<(string packageId, string version)> references = new();
				List<string> filePaths = new(files.Values);
				for (int i = filePaths.Count - 1; i >= 0; i--)
				{
					if (filePaths[i] == NuGetConstants.NUGET_DEPENDENCIES_FROM_PROJECT_VALUE)
					{
						filePaths.RemoveAt(i);
						filePaths.AddRange(configuration.GetProjectsPath());
						continue;
					}

					filePaths[i] = configuration.AddRootDirectory(filePaths[i]);
				}

				foreach (string projectFile in filePaths)
				{
					configuration.FileExistsOrThrow(projectFile);

					using Stream input = configuration.Context.CakeContext.FileSystem.GetFile(projectFile).OpenRead();
					XDocument document = XDocument.Load(input);
					if (document.Root is null)
					{
						continue;
					}

					foreach (XElement child in document.Root.Descendants())
					{
						if (child.Name.LocalName == "PackageReference")
						{
							string packageId = null;
							string version = null;
							foreach (XAttribute attribute in child.Attributes())
							{
								if (attribute.Name.LocalName == "Include")
								{
									packageId = attribute.Value;
								}
								else if (attribute.Name.LocalName == "Version")
								{
									version = attribute.Value;
								}
							}

							if (string.IsNullOrEmpty(packageId) || string.IsNullOrEmpty(version))
							{
								continue;
							}

							references.Add((packageId, version));
						}
					}
				}

				return references.GroupBy(x => x.packageId)
					.Select(x => (packageId: x.Key, version: x.OrderByDescending(y => y).FirstOrDefault().version))
					.Select(x => new NugetDependency
					{
						PackageId = x.packageId,
						Version = x.version,
					})
					.ToList();
			}

			return new();
		}

		private class NugetDependency
		{
			public string PackageId { get; set; }

			public string Version { get; set; }
		}
	}
}