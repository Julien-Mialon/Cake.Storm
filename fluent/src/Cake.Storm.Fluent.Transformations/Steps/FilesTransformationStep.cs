using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	internal class FilesTransformationStep : BaseMultipleStep
	{
		private readonly IFilesTransformationAction _transformation;

		public FilesTransformationStep(IFilesTransformationAction transformation, StepType onStep) : base(onStep)
		{
			_transformation = transformation;
		}

		protected override void Execute(IConfiguration configuration)
		{
			_transformation.Execute(configuration);
		}
	}
}