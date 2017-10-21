using Cake.Common;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	internal class ArgumentValueResolver<TValue> : IValueResolver<TValue>
	{
		private readonly string _argumentName;
		private readonly TValue _defaultValue;
		private readonly bool _hasDefaultValue;

		public ArgumentValueResolver(string argumentName)
		{
			_argumentName = argumentName;
		}
		
		public ArgumentValueResolver(string argumentName, TValue defaultValue)
		{
			_argumentName = argumentName;
			_defaultValue = defaultValue;
			_hasDefaultValue = true;
		}

		public TValue Resolve(IConfiguration configuration)
		{
			var context = configuration.Context.CakeContext;
			if (context.HasArgument(_argumentName))
			{
				return context.Argument<TValue>(_argumentName);
			}

			if (_hasDefaultValue)
			{
				return _defaultValue;
			}
			
			context.LogAndThrow($"Missing argument {_argumentName}");
			return default(TValue);
		}
	}
}