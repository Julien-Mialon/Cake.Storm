using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Android.Interfaces
{
	public interface IAndroidManifestTransformation
	{
		IAndroidManifestTransformation WithPackage(string package);

		IAndroidManifestTransformation WithVersionName(string versionName);

		IAndroidManifestTransformation WithVersionCode(int versionCode);

		IAndroidManifestTransformation WithPackageFromParameter();

		IAndroidManifestTransformation WithVersionNameFromParameter();

		IAndroidManifestTransformation WithVersionCodeFromParameter();
	}

	internal interface IAndroidManifestTransformationAction : IAndroidManifestTransformation
	{
		void Execute(FilePath manifestFile, IConfiguration configuration);
	}
}