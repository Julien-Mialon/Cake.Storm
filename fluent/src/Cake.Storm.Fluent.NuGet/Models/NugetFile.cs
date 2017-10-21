namespace Cake.Storm.Fluent.NuGet.Models
{
	public class NugetFile
	{
		public NugetFile(string filePath, string nugetRelativePath)
		{
			FilePath = filePath;
			NugetRelativePath = nugetRelativePath;
		}

		public string FilePath { get; }
		
		public string NugetRelativePath { get; }
	}
}