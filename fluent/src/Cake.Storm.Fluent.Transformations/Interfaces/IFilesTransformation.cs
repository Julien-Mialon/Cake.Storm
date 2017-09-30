using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Transformations.Interfaces
{
	public interface IFilesTransformation
	{
		IFilesTransformation OnFile(string file);
		
		IFilesTransformation Replace(string source, string target);
	}

	internal interface IFilesTransformationAction : IFilesTransformation
	{
		void Execute(IConfiguration configuration);
	}
}