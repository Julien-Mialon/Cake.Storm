using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.NuGet.Extensions")]

namespace Cake.Storm.Fluent.NuGet
{
	public static class Aliases
	{
		public static void ImportNuGet(this ICakeContext context)
		{
			context.Log.Information("Fluent: import NuGet tooling...");
		}
	}
}