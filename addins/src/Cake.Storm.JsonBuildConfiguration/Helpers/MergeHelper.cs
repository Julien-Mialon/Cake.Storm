using System;
using System.Collections.Generic;

namespace Cake.Storm.JsonBuildConfiguration.Helpers
{
	public static class MergeHelper
	{
		public static List<T> MergeList<T>(List<T> source, List<T> overwrite, Func<T, T, bool> areEqual, Func<T, T, T> merge)
		{
			if (overwrite == null)
			{
				return source;
			}
			if (source == null)
			{
				return overwrite;
			}

			List<T> results = new List<T>(source);

			foreach (T overwriteItem in overwrite)
			{
				int index = -1;
				for (int i = 0; i < results.Count; ++i)
				{
					if (areEqual(results[i], overwriteItem))
					{
						index = i;
						break;
					}
				}

				if (index < 0)
				{
					results.Add(overwriteItem);
				}
				else
				{
					results[index] = merge(results[index], overwriteItem);
				}
			}

			return results;
		}
	}
}
