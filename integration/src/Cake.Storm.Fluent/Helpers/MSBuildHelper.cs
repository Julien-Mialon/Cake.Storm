using System.Collections.Generic;
using System.Linq;

namespace Cake.Storm.Fluent.Helpers
{
	public static class MSBuildHelper
    {
	    public static string PropertyValue(string value)
	    {
			value = value.Replace(",", "%2c");
		    if (value.Any(char.IsWhiteSpace))
		    {
			    value = $"'{value}'";
		    }
		    return value;
		}

		public static string PropertyValue(List<string> values)
	    {
		    return string.Join(",", values.Select(PropertyValue));
	    }
	}
}
