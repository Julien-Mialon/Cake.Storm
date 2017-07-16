using System.IO;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	[PreBuildStep]
    public class CodeTransformationStep : IStep
	{
		private readonly string _filePath;
		private readonly ICodeTransformation _transformation;

		public CodeTransformationStep(string filePath, ICodeTransformation transformation)
		{
			_filePath = filePath;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration)
		{
			string path = configuration.AddRootDirectory(_filePath);
			if (!configuration.Context.CakeContext.FileExists(path))
			{
				configuration.Context.CakeContext.LogAndThrow($"File {path} does not exists");
			}

			string fileContent;
			using (Stream inputStream = configuration.Context.CakeContext.FileSystem.GetFile(path).OpenRead())
			{
				using (StreamReader streamReader = new StreamReader(inputStream))
				{
					fileContent = streamReader.ReadToEnd();
				}
			}

			SyntaxNode node = CSharpSyntaxTree.ParseText(fileContent).GetRoot();
			node = _transformation.Execute(node);

			fileContent = node.ToFullString();

			using (Stream outputStream = configuration.Context.CakeContext.FileSystem.GetFile(path).OpenWrite())
			{
				using (StreamWriter streamWriter = new StreamWriter(outputStream))
				{
					streamWriter.Write(fileContent);
				}
			}
		}
    }
}
