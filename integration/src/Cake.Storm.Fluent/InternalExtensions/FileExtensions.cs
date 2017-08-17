using System.IO;
using Cake.Core.IO;

namespace Cake.Storm.Fluent.InternalExtensions
{
	public static class FileExtensions
	{
		public static string ReadAll(this IFile file)
		{
			using (Stream inputStream = file.OpenRead())
			{
				using (TextReader inputReader = new StreamReader(inputStream))
				{
					return inputReader.ReadToEnd();
				}
			}
		}
	}
}