using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Interfaces")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Models")]

namespace Cake.Storm.Fluent.iOS
{
    public static class Aliases
    {
	    //This method is only for namespace import in cake script
	    [CakeMethodAlias]
	    public static void ImportiOS(this ICakeContext context)
	    {
		    context.Log.Information("Fluent: import iOS tooling...");
	    }
	}
}
