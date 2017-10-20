namespace Cake.Storm.Fluent.Resolvers
{
	public static class ValueResolver
	{
		public static IValueResolver<TValue> FromConstant<TValue>(TValue value)
		{
			return new ConstantValueResolver<TValue>(value);
		}

		public static IValueResolver<TValue> FromArgument<TValue>(string argumentName)
		{
			return new ArgumentValueResolver<TValue>(argumentName);
		}

		public static IValueResolver<TValue> FromArgument<TValue>(string argumentName, TValue defaultValue)
		{
			return new ArgumentWithDefaultValueResolver<TValue>(argumentName, defaultValue);
		}

		public static IValueResolver<TValue> FromEnvironment<TValue>(string variableName)
		{
			return new EnvironmentValueResolver<TValue>(variableName);
		}
		
		public static IValueResolver<TValue> FromEnvironment<TValue>(string variableName, TValue defaultValue)
		{
			return new EnvironmentWithDefaultValueResolver<TValue>(variableName, defaultValue);
		}
	}
}