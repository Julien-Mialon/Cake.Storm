using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Transformations.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Transformations.Interfaces")]

namespace Cake.Storm.Fluent.Transformations
{
    public static class Aliases
    {
	    //This method is only for namespace import in cake script
	    [CakeMethodAlias]
	    public static void ImportTransformations(this ICakeContext context)
	    {
		    context.Log.Information("Fluent: import transformations tooling...");
	    }
    }
}
