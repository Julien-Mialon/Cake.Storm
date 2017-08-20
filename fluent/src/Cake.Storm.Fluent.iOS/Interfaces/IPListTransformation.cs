using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.iOS.Interfaces
{
	public interface IPListTransformation
	{
		IPListTransformation WithVersion(string version);

		IPListTransformation WithVersionFromParameter();

		IPListTransformation WithBundleId(string bundleId);

		IPListTransformation WithBundleIdFromParameter();
	}

	internal interface IPListTransformationAction : IPListTransformation
	{
		void Execute(FilePath filePath, IConfiguration configuration);
	}
}