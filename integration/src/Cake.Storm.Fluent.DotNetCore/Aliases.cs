using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.DotNetCore.Extensions")]

namespace Cake.Storm.Fluent.DotNetCore
{
	public static class Aliases
    {
	    //This method is only for namespace import in cake script
		[CakeMethodAlias]
	    public static void ImportDotNetCore(this ICakeContext context)
	    {
		    context.Log.Information("Fluent: import dotnetcore tooling...");
	    }
    }
}
