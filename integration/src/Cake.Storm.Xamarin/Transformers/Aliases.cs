using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Storm.Xamarin.Transformers.Models;

namespace Cake.Storm.Xamarin.Transformers
{
	[CakeAliasCategory("Cake.Storm.Xamarin")]
	[CakeNamespaceImport("Cake.Storm.Xamarin.Transformers.Models")]
    public static class Aliases
    {
	    [CakeMethodAlias]
	    public static CSharpCodeFile TransformCSharpFile(this ICakeContext context, FilePath filePath)
	    {
		    if (!context.FileExists(filePath))
		    {
			    context.LogAndThrow($"File {filePath} does not exists");
		    }

			return new CSharpCodeFile(context, filePath);
	    }
    }
}
