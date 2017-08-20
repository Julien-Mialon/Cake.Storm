using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	public class SimpleConfigurationItem<TValue> : IConfigurationItem
	{
		public TValue Value { get; set; }

		public SimpleConfigurationItem() { }

		public SimpleConfigurationItem(TValue value) => Value = value;

		public IConfigurationItem Merge(IConfigurationItem other)
		{
			if (other is SimpleConfigurationItem<TValue>)
			{
				return other;
			}

			throw new CakeException($"IConfigurationItem.Merge {other.GetType().FullName} into {GetType().Name} is not possible");
		}

		public override string ToString()
		{
			return $"\t Value: {Value} ({nameof(SimpleConfigurationItem<TValue>)}<{typeof(TValue).Name}>)";
		}
	}
}