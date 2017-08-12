using System;
using Cake.Core;
using Cake.Core.Annotations;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Interfaces")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.iOS.Models")]

namespace Cake.Storm.Fluent.iOS
{
    public static class Aliases
    {
	    //only for namespace import to work correctly
	    [CakeMethodAlias]
	    public static void ImportiOS(this ICakeContext context)
	    {

	    }
	}
}
