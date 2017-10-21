using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.NuGet.Models;
using Cake.Storm.Fluent.Resolvers;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.NuGet.Resolvers
{
	internal class AllFilesFromDirectoryWithPatternNugetFileResolver : IValueResolver<IEnumerable<NugetFile>>
	{
		private readonly string _directory;
		private readonly string _pattern;
		private readonly string _nugetRelativePath;
		private readonly Func<string, IConfiguration, string> _obtainDirectoryFullPath;

		public AllFilesFromDirectoryWithPatternNugetFileResolver(string directory, string pattern, string nugetRelativePath, Func<string, IConfiguration, string> obtainDirectoryFullPath)
		{
			_directory = directory;
			_pattern = pattern;
			_nugetRelativePath = nugetRelativePath ?? string.Empty;
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

			string pattern = rootDirectoryPath.FullPath + Path.DirectorySeparatorChar + _pattern;

			foreach (FilePath file in context.Globber.GetFiles(pattern))
			{
				string relativePath = NugetFileHelper.GetRelativePath(_nugetRelativePath, rootDirectoryPath, file);
				yield return new NugetFile(file.FullPath, relativePath);
			}
		}
	}
}