using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.NuGet.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.NuGet.Interfaces")]

namespace Cake.Storm.Fluent.NuGet
{
	public static class Aliases
	{
		//This method is only for namespace import in cake script
		[CakeMethodAlias]
		public static void ImportNuget(this ICakeContext context)
		{
			context.Log.Information("Fluent: import NuGet tooling...");
		}
	}
}