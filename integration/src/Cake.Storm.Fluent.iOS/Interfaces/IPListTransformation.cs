using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.iOS.Interfaces
{
    public interface IPListTransformation
    {
	    IPListTransformation UseVersion(string version);

	    IPListTransformation UseVersionFromParameter();

	    IPListTransformation UseBundleId(string bundleId);

	    IPListTransformation UseBundleIdFromParameter();

	    void Execute(FilePath filePath, IConfiguration configuration);
    }
}
