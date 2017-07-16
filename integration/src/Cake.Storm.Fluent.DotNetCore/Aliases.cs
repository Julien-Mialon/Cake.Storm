using Cake.Core;
using Cake.Core.Annotations;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.DotNetCore.Extensions")]

namespace Cake.Storm.Fluent.DotNetCore
{
	public static class Aliases
    {
		//only for namespace import to work correctly
		[CakeMethodAlias]
	    public static void ImportDotNetCore(this ICakeContext context)
	    {
		    
	    }
    }
}
