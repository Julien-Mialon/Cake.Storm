using Cake.Core;
using Cake.Core.Annotations;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Transformations.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Transformations.Interfaces")]

namespace Cake.Storm.Fluent.Transformations
{
    public static class Aliases
    {
	    [CakeMethodAlias]
	    public static void ImportTransformations(this ICakeContext context)
	    {
		    
	    }
    }
}
