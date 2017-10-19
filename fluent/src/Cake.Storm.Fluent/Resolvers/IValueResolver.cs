using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Resolvers
{
	public interface IValueResolver<out TValue>
	{
		TValue Resolve(IConfiguration configuration);
	}
}