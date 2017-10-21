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
	internal class SimpleFileNugetFileResolver : IValueResolver<IEnumerable<NugetFile>>
	{
		private readonly string _filePath;
		private readonly string _nugetRelativePath;
		private readonly Func<string, IConfiguration, string> _obtainFileFullPath;

		public SimpleFileNugetFileResolver(string filePath, string nugetRelativePath, Func<string, IConfiguration, string> obtainFileFullPath)
		{
			_filePath = filePath;
			_nugetRelativePath = nugetRelativePath;
			_obtainFileFullPath = obtainFileFullPath;
		}

		public IEnumerable<NugetFile> Resolve(IConfiguration configuration)
		{
			ICakeContext context = configuration.Context.CakeContext;

			FilePath filePath = context.File(_obtainFileFullPath(_filePath, configuration));
			filePath = filePath.MakeAbsolute(context.Environment);
			if (!context.FileExists(filePath))
			{
				context.LogAndThrow($"File {filePath} does not exists");
			}

			string relativePath = NugetFileHelper.GetRelativePath(_nugetRelativePath, filePath.GetDirectory(), filePath);
			yield return new NugetFile(filePath.FullPath, relativePath);
		}
	}
}