namespace Cake.Storm.Fluent.Transformations.Interfaces
{
	public interface IFileTransformation
	{
		IFileTransformation Replace(string source, string target);
	}

	internal interface IFileTransformationAction : IFileTransformation
	{
		string Execute(string content);
	}
}