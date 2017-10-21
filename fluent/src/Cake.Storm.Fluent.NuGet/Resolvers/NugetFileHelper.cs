using System;
using Cake.Core.IO;
using Path = System.IO.Path;

namespace Cake.Storm.Fluent.NuGet.Resolvers
{
	internal static class NugetFileHelper
	{
		public static string GetRelativePath(string nugetRelativePath, DirectoryPath directory, FilePath file)
		{
			string directoryPath = directory.FullPath;
			string fileContainerPath = file.GetDirectory().FullPath;

			if (!fileContainerPath.StartsWith(directoryPath))
			{
				throw new ArgumentException("File and directory are not related", nameof(file));
			}

			if (fileContainerPath == directoryPath)
			{
				return nugetRelativePath;
			}
			
			string subPath = fileContainerPath.Substring(directoryPath.Length + 1); //remove path separator char
			if (nugetRelativePath == null)
			{
				return subPath;
			}
			return Path.Combine(nugetRelativePath, subPath);
		}
	}
}