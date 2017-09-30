using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	[PreBuildStep]
	[MultiStep]
	internal class FilesTransformationStep : IStep
	{
		private readonly IFilesTransformationAction _transformation;

		public FilesTransformationStep(IFilesTransformationAction transformation)
		{
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration)
		{
			_transformation.Execute(configuration);
		}
	}
}