using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.Transformations.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Steps
{
	[PreBuildStep]
	[MultiStep]
	internal class CsprojTransformationStep : IStep
	{
		private readonly string _projectFile;
		private readonly ICsprojTransformationAction _transformation;

		public CsprojTransformationStep(string projectFile, ICsprojTransformationAction transformation)
		{
			_projectFile = projectFile;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (_projectFile != null)
			{
				Transform(configuration.AddRootDirectory(_projectFile));
			}
			else
			{
				foreach (string file in configuration.GetProjectsPath())
				{
					Transform(file);
				}
			}

			void Transform(string file)
			{
				configuration.FileExistsOrThrow(file);

				_transformation.Execute(file, configuration);
			}
		}
	}
}