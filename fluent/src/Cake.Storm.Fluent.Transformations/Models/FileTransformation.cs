using System.Collections.Generic;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Models
{
	internal class FileTransformation : IFileTransformationAction
	{
		private readonly List<Replacement> _replacements = new List<Replacement>();

		public IFileTransformation Replace(string source, string target)
		{
			_replacements.Add(new Replacement(source, target));
			return this;
		}

		public string Execute(string content)
		{
			foreach (var replacement in _replacements)
			{
				content = content.Replace(replacement.Source, replacement.Target);
			}
			return content;
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