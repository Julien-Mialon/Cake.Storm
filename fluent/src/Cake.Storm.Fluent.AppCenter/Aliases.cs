using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;

[assembly: CakeNamespaceImport("Cake.Storm.Fluent.AppCenter.Extensions")]
[assembly: CakeNamespaceImport("Cake.Storm.Fluent.AppCenter.Interfaces")]

namespace Cake.Storm.Fluent.AppCenter
{
	public static class Aliases
	{
		//This method is only for namespace import in cake script
		[CakeMethodAlias]
		public static void ImportAppCenter(this ICakeContext context)
		{
			context.Log.Information("Fluent: import appcenter tooling...");
		}
	}
}