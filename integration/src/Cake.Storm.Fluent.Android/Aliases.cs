using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Android.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.Android.Interfaces")]

namespace Cake.Storm.Fluent.Android
{
	public static class Aliases
	{
		//This method is only for namespace import in cake script
		[CakeMethodAlias]
		public static void ImportAndroid(this ICakeContext context)
		{
			context.Log.Information("Fluent: import android tooling...");
		}
	}
}