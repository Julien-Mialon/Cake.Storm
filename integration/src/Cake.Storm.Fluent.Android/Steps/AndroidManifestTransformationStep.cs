using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PreBuildStep]
	internal class AndroidManifestTransformationStep : IStep
	{
		private readonly string _manifestFile;
		private readonly IAndroidManifestTransformationAction _transformation;

		public AndroidManifestTransformationStep(string manifestFile, IAndroidManifestTransformationAction transformation)
		{
			_manifestFile = manifestFile;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration)
		{
			_transformation.Execute(_manifestFile, configuration);
		}
	}
}