using System.ComponentModel;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	internal class EnvironmentWithDefaultValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly string _variableName;
		private readonly TValue _defaultValue;

		public EnvironmentWithDefaultValueResolver(string variableName, TValue defaultValue)
		{
			_variableName = variableName;
			_defaultValue = defaultValue;
		}

		public TValue Resolve(IConfiguration configuration)
		{
			var context = configuration.Context.CakeContext;
			string value = context.Environment.GetEnvironmentVariable(_variableName);
			if (value == null)
			{
				return _defaultValue;
			}

			//In order to convert the value, we are using the same method than Cake.Argument<> method
			return (TValue) TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromInvariantString(value);
		}
	}
}