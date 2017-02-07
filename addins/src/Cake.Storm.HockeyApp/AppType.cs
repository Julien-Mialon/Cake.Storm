using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cake.Storm.HockeyApp
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AppType
	{
		Android,
		iOS
	}
}
