using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	public class DictionaryOfListConfigurationItem<TKey, TValue> : IConfigurationItem
	{
		public Dictionary<TKey, List<TValue>> Values { get; }

		public DictionaryOfListConfigurationItem() => Values = new Dictionary<TKey, List<TValue>>();

		public DictionaryOfListConfigurationItem(TKey key, TValue value) : this() => Values.Add(key, new List<TValue> { value });

		public DictionaryOfListConfigurationItem(Dictionary<TKey, List<TValue>> values) => Values = values;

		public IConfigurationItem Merge(IConfigurationItem other)
		{
			if (other is DictionaryOfListConfigurationItem<TKey, TValue> otherDictionary)
			{
				Dictionary<TKey, List<TValue>> result = new Dictionary<TKey, List<TValue>>(Values);
				foreach (var item in otherDictionary.Values)
				{
					if (result.TryGetValue(item.Key, out var currentValues))
					{
						currentValues.AddRange(item.Value);
					}
					else
					{
						result.Add(item.Key, item.Value);
					}
				}
				return new DictionaryOfListConfigurationItem<TKey, TValue>(result);
			}

			throw new CakeException($"IConfigurationItem.Merge {other.GetType().FullName} into {GetType().Name} is not possible");
		}
		
		public override string ToString()
		{
			if (Values.Count > 0)
			{
				StringBuilder builder = new StringBuilder();

				builder.AppendLine($"\t Values: [");
				foreach (var item in Values)
				{
					builder.AppendLine($"\t\t {item.Key} = [{string.Join(", ", item.Value.Select(x => x.ToString()))}]");
				}
				builder.AppendLine($"\t ] ({nameof(DictionaryOfListConfigurationItem<TKey, TValue>)}<{typeof(TKey).Name}, {typeof(TValue).Name}>)");
				return builder.ToString();
			}
			return $"\t Values: [] ({nameof(DictionaryOfListConfigurationItem<TKey, TValue>)}<{typeof(TKey).Name}, {typeof(TValue).Name}>)";
		}
	}
}