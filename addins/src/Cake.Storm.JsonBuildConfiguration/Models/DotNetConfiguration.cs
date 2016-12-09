using Newtonsoft.Json;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	internal class DotNetConfiguration
	{
		public static DotNetConfiguration Merge(DotNetConfiguration source, DotNetConfiguration overwrite)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			return new DotNetConfiguration
			{
				
			};
		}
	}
}
