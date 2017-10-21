using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.NuGet.Models;
using Cake.Storm.Fluent.Resolvers;

namespace Cake.Storm.Fluent.NuGet.Resolvers
{
	internal class AllFilesFromDirectoryNugetFileResolver : IValueResolver<IEnumerable<NugetFile>>
	{
		private readonly string _directory;
		private readonly string _nugetRelativePath;
		private readonly Func<string, IConfiguration, string> _obtainDirectoryFullPath;

		public AllFilesFromDirectoryNugetFileResolver(string directory, string nugetRelativePath, Func<string, IConfiguration, string> obtainDirectoryFullPath)
		{
			_directory = directory;
			_nugetRelativePath = nugetRelativePath;
			_obtainDirectoryFullPath = obtainDirectoryFullPath;
		}

		public IEnumerable<NugetFile> Resolve(IConfiguration configuration)
		{
			ICakeContext context = configuration.Context.CakeContext;

			DirectoryPath rootDirectoryPath = context.Directory(_obtainDirectoryFullPath(_directory, configuration));
			rootDirectoryPath = rootDirectoryPath.MakeAbsolute(context.Environment);
			if (!context.DirectoryExists(rootDirectoryPath))
			{
				context.LogAndThrow($"Directory {rootDirectoryPath} does not exists");
			}

			IDirectory rootDirectory = context.FileSystem.GetDirectory(rootDirectoryPath);
			foreach (IFile file in rootDirectory.GetFiles("*", SearchScope.Recursive))
			{
				string relativePath = NugetFileHelper.GetRelativePath(_nugetRelativePath, rootDirectory.Path, file.Path);
				yield return new NugetFile(file.Path.FullPath, relativePath);
			}
		}
	}
}