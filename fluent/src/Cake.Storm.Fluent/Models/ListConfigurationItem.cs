using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	public class ListConfigurationItem<TValue> : IConfigurationItem
	{
		public List<TValue> Values { get; }

		public ListConfigurationItem() => Values = new List<TValue>();

		public ListConfigurationItem(params TValue[] values) : this(values.AsEnumerable()) { }

		public ListConfigurationItem(IEnumerable<TValue> values) => Values = new List<TValue>(values);

		public IConfigurationItem Merge(IConfigurationItem other)
		{
			if (other is ListConfigurationItem<TValue> otherList)
			{
				return new ListConfigurationItem<TValue>(Enumerable.Concat(Values, otherList.Values));
			}

			throw new CakeException($"IConfigurationItem.Merge {other.GetType().FullName} into {GetType().Name} is not possible");
		}

		public override string ToString()
		{
			if (Values.Count > 0)
			{
				StringBuilder builder = new StringBuilder();

				builder.AppendLine($"\t Values: [");
				foreach (var value in Values)
				{
					builder.AppendLine($"\t\t {value}");
				}
				builder.AppendLine($"\t ] ({nameof(ListConfigurationItem<TValue>)}<{typeof(TValue).Name}>)");
				return builder.ToString();
			}
			return $"\t Values: [] ({nameof(ListConfigurationItem<TValue>)}<{typeof(TValue).Name}>)";
		}
	}
}