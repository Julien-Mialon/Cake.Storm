namespace Cake.Storm.Fluent.Resolvers
{
	internal class ConstantValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly TValue _value;

		public ConstantValueResolver(TValue value)
		{
			_value = value;
		}

		public TValue Resolve()
		{
			return _value;
		}
	}
}