using Cake.Common;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	internal class ArgumentValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly string _argumentName;

		public ArgumentValueResolver(string argumentName)
		{
			_argumentName = argumentName;
		}

		public TValue Resolve(IConfiguration configuration)
		{
			var context = configuration.Context.CakeContext;
			if (context.HasArgument(_argumentName))
			{
				return context.Argument<TValue>(_argumentName);
			}
			context.LogAndThrow($"Missing argument {_argumentName}");
			return default(TValue);
		}
	}
}