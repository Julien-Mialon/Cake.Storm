using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cake.Storm.JsonBuildConfiguration.Models
{
	[JsonConverter(typeof(StringEnumConverter))]
	internal enum PlatformType
	{
		Undefined,
		Android,
		iOS,
		Dotnet
	}
}
