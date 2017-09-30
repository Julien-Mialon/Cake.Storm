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
			string projectFile;
			if (_projectFile == null)
			{
				projectFile = configuration.GetProjectPath();
			}
			else
			{
				projectFile = configuration.AddRootDirectory(_projectFile);
			}
			
			
			configuration.FileExistsOrThrow(projectFile);
			
			_transformation.Execute(projectFile, configuration);
		}
	}
}