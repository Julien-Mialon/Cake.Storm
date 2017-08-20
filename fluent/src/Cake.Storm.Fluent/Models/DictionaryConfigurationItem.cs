using System.Collections.Generic;
using System.Text;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	public class DictionaryConfigurationItem<TKey, TValue> : IConfigurationItem
	{
		public Dictionary<TKey, TValue> Values { get; }

		public DictionaryConfigurationItem() => Values = new Dictionary<TKey, TValue>();

		public DictionaryConfigurationItem(TKey key, TValue value) : this() => Values.Add(key, value);

		public DictionaryConfigurationItem(Dictionary<TKey, TValue> values) => Values = values;

		public IConfigurationItem Merge(IConfigurationItem other)
		{
			if (other is DictionaryConfigurationItem<TKey, TValue> otherDictionary)
			{
				Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(Values);
				foreach (var item in otherDictionary.Values)
				{
					result[item.Key] = item.Value;
				}
				return new DictionaryConfigurationItem<TKey, TValue>(result);
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
					builder.AppendLine($"\t\t {item.Key} = {item.Value}");
				}
				builder.AppendLine($"\t ] ({nameof(DictionaryConfigurationItem<TKey, TValue>)}<{typeof(TKey).Name}, {typeof(TValue).Name}>)");
				return builder.ToString();
			}
			return $"\t Values: [] ({nameof(DictionaryConfigurationItem<TKey, TValue>)}<{typeof(TKey).Name}, {typeof(TValue).Name}>)";
		}
	}
}