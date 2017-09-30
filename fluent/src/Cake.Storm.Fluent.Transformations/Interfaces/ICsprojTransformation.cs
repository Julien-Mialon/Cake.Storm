using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Interfaces
{
	public interface ICsprojTransformation
	{
		ICsprojTransformation UpdatePackageVersion(string newVersion);

		ICsprojTransformation UpdatePackageVersionFromParameter();
	}

	internal interface ICsprojTransformationAction : ICsprojTransformation
	{
		void Execute(FilePath projectFile, IConfiguration configuration);
	}
}