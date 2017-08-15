using Cake.Common.IO;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;

namespace Cake.Storm.Fluent.iOS.Steps
{
	[PreBuildStep]
    internal class PListTransformationStep : IStep
	{
		private readonly string _sourceFile;
		private readonly IPListTransformationAction _transformation;

		public PListTransformationStep(string sourceFile, IPListTransformationAction transformation)
		{
			_sourceFile = sourceFile;
			_transformation = transformation;
		}

		public void Execute(IConfiguration configuration)
		{
			string plistFile = configuration.AddRootDirectory(_sourceFile);
			configuration.FileExistsOrThrow(plistFile);
			
			_transformation.Execute(plistFile, configuration);
		}
	}
}
