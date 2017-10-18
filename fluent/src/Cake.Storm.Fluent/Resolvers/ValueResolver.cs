namespace Cake.Storm.Fluent.Resolvers
{
	public static class ValueResolver
	{
		public static IValueResolver<TValue> FromConstant<TValue>(TValue value)
		{
			return new ConstantValueResolver<TValue>(value);
		}
	}
}