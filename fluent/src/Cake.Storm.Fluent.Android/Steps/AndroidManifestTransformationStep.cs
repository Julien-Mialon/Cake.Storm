using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PreBuildStep]
	[MultiStep]
	internal class AndroidManifestTransformationStep : IStep
	{
		private readonly string _manifestFile;
		private readonly IAndroidManifestTransformationAction _transformation;

		public AndroidManifestTransformationStep(string manifestFile, IAndroidManifestTransformationAction transformation)
		{
			_manifestFile = manifestFile;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			_transformation.Execute(configuration.AddRootDirectory(_manifestFile), configuration);
		}
	}
}