using System.ComponentModel;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	internal class EnvironmentValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly string _variableName;

		public EnvironmentValueResolver(string variableName)
		{
			_variableName = variableName;
		}

		public TValue Resolve(IConfiguration configuration)
		{
			var context = configuration.Context.CakeContext;
			string value = context.Environment.GetEnvironmentVariable(_variableName);
			if (value == null)
			{
				context.LogAndThrow($"Missing environment variable {_variableName}");
			}

			//In order to convert the value, we are using the same method than Cake.Argument<> method
			return (TValue) TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromInvariantString(value);
		}
	}
}