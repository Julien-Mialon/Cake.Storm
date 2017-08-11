using Cake.Common.IO;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.iOS.Interfaces;

namespace Cake.Storm.Fluent.iOS.Steps
{
	[PreBuildStep]
    internal class PListTransformationStep : IStep
	{
		private readonly string _sourceFile;
		private readonly IPListTransformation _transformation;

		public PListTransformationStep(string sourceFile, IPListTransformation transformation)
		{
			_sourceFile = sourceFile;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration)
		{
			if (!configuration.Context.CakeContext.FileExists(_sourceFile))
			{
				configuration.Context.CakeContext.LogAndThrow($"PList file {_sourceFile} does not exists");
			}

			_transformation.Execute(_sourceFile, configuration);
		}
	}
}
