using System.Collections.Generic;
using System.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Models
{
	internal class FilesTransformation : IFilesTransformationAction
	{
		private readonly List<Replacement> _replacements = new List<Replacement>();
		private readonly List<string> _files = new List<string>();

		public IFilesTransformation OnFile(string file)
		{
			_files.Add(file);
			return this;
		}

		public IFilesTransformation Replace(string source, string target)
		{
			_replacements.Add(new Replacement(source, target));
			return this;
		}

		public void Execute(IConfiguration configuration)
		{
			foreach (string relativeFilePath in _files)
			{
				string file = configuration.AddRootDirectory(relativeFilePath);
				configuration.FileExistsOrThrow(file);

				string content = File.ReadAllText(file);
				foreach (var replacement in _replacements)
				{
					content = content.Replace(replacement.Source, replacement.Target);
				}

				File.WriteAllText(file, content);
			}
		}

		private struct Replacement
		{
			public Replacement(string source, string target)
			{
				Source = source;
				Target = target;
			}

			public string Source { get; }

			public string Target { get; }
		}
	}
}