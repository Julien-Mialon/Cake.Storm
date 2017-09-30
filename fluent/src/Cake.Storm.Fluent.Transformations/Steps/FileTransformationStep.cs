using System.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	internal class FileTransformationStep : BaseMultipleStep
	{
		private readonly string _file;
		private readonly IFileTransformationAction _transformation;

		public FileTransformationStep(string file, IFileTransformationAction transformation, StepType onStep) : base(onStep)
		{
			_file = file;
			_transformation = transformation;
		}

		protected override void Execute(IConfiguration configuration)
		{
			string file = configuration.AddRootDirectory(_file);
			configuration.FileExistsOrThrow(file);

			string content = File.ReadAllText(file);
			string updatedContent = _transformation.Execute(content);
			
			File.WriteAllText(file, updatedContent);
		}
	}
}