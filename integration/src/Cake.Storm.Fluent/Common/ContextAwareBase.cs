using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Common
{
	internal abstract class ContextAwareBase : IContextAware
	{
		public IFluentContext Context { get; }

		protected ContextAwareBase(IFluentContext context)
		{
			Context = context;
		}
	}
}