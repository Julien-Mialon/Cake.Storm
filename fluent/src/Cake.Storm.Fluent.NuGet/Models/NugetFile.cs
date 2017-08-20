namespace Cake.Storm.Fluent.NuGet.Models
{
	internal class NugetFile
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