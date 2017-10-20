using Cake.Common;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	internal class ArgumentWithDefaultValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly string _argumentName;
		private readonly TValue _defaultValue;

		public ArgumentWithDefaultValueResolver(string argumentName, TValue defaultValue)
		{
			_argumentName = argumentName;
			_defaultValue = defaultValue;
		}

		public TValue Resolve(IConfiguration configuration)
		{
			var context = configuration.Context.CakeContext;
			if (context.HasArgument(_argumentName))
			{
				return context.Argument<TValue>(_argumentName);
			}

			return _defaultValue;
		}
	}
}